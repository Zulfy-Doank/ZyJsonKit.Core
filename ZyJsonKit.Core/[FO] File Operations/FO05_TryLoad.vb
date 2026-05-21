Imports System.IO
Imports System.Text

''' <summary>
''' FO05_TryLoad
''' Safe file loading dengan pattern Try (return success flag)
''' Tidak throw exception, cocok untuk validasi file
''' [PF03] Exception-safe - Tidak pernah throw
''' [PF04] Null-safe - Handle semua null
''' </summary>
Public NotInheritable Class FO05_TryLoad

    ''' <summary>
    ''' Mencoba memuat file JSON dengan Try pattern
    ''' </summary>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <param name="hasil">Output string JSON</param>
    ''' <returns>True jika berhasil memuat</returns>
    Public Shared Function CobaMuat(
        pathFile As String,
        ByRef hasil As String) As Boolean

        hasil = Nothing

        If String.IsNullOrWhiteSpace(pathFile) Then Return False
        If Not File.Exists(pathFile) Then Return False

        Try
            hasil = File.ReadAllText(pathFile, Encoding.UTF8)
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Mencoba memuat file JSON dengan error detail
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="hasil">Output string JSON</param>
    ''' <param name="pesanError">Output pesan error jika gagal</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function CobaMuatDenganError(
        pathFile As String,
        ByRef hasil As String,
        ByRef pesanError As String) As Boolean

        hasil = Nothing
        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(pathFile) Then
            pesanError = "Path file tidak boleh kosong"
            Return False
        End If

        If Not File.Exists(pathFile) Then
            pesanError = $"File tidak ditemukan: {pathFile}"
            Return False
        End If

        Dim ekstensi = Path.GetExtension(pathFile).ToLowerInvariant()
        If ekstensi <> ".json" AndAlso
           ekstensi <> ".txt" AndAlso
           ekstensi <> ".js" Then
            pesanError = $"Ekstensi file mencurigakan: {ekstensi}"
            ' Tetap coba baca
        End If

        Try
            Dim fileInfo As New FileInfo(pathFile)

            If fileInfo.Length = 0 Then
                pesanError = "File kosong (0 bytes)"
                Return False
            End If

            If fileInfo.Length > 500 * 1024 * 1024 Then ' 500 MB
                pesanError = $"File terlalu besar: {fileInfo.Length / (1024 * 1024):N0} MB (maks 500 MB)"
                Return False
            End If
        Catch ex As Exception
            pesanError = $"Tidak bisa membaca info file: {ex.Message}"
            Return False
        End Try

        Try
            hasil = File.ReadAllText(pathFile, Encoding.UTF8)
            Return True
        Catch ex As UnauthorizedAccessException
            pesanError = $"Akses ditolak: {ex.Message}"
            Return False
        Catch ex As DirectoryNotFoundException
            pesanError = $"Direktori tidak ditemukan: {ex.Message}"
            Return False
        Catch ex As PathTooLongException
            pesanError = "Path file terlalu panjang"
            Return False
        Catch ex As IOException
            pesanError = $"Error IO: {ex.Message}"
            Return False
        Catch ex As Exception
            pesanError = $"Error tidak terduga: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Mencoba memuat dan validasi sebagai JSON
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="hasil">Output string JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>True jika file valid JSON</returns>
    Public Shared Function CobaMuatDanValidasi(
        pathFile As String,
        ByRef hasil As String,
        ByRef pesanError As String) As Boolean

        If Not CobaMuatDenganError(pathFile, hasil, pesanError) Then
            Return False
        End If

        ' ✅ Fix: J01_Validate → JP01_Validate
        If Not JP01_Validate.ApakahValid(hasil) Then
            pesanError = "File bukan JSON yang valid"
            hasil = Nothing
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Mencoba memuat file dengan multiple encoding fallback
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="hasil">Output string</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function CobaMuatMultiEncoding(
        pathFile As String,
        ByRef hasil As String) As Boolean

        hasil = Nothing

        If String.IsNullOrWhiteSpace(pathFile) OrElse
           Not File.Exists(pathFile) Then
            Return False
        End If

        Dim daftarEncoding As Encoding() = {
            Encoding.UTF8,
            Encoding.Default,
            Encoding.ASCII,
            Encoding.Unicode,
            Encoding.BigEndianUnicode,
            Encoding.UTF32
        }

        For Each enc In daftarEncoding
            Try
                hasil = File.ReadAllText(pathFile, enc)
                If Not String.IsNullOrWhiteSpace(hasil) Then
                    If Not hasil.Contains("▯") Then
                        Return True
                    End If
                End If
            Catch
                ' Lanjut encoding berikutnya
            End Try
        Next

        Return False
    End Function

    ''' <summary>
    ''' Mencoba memuat file JSON dengan retry
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="hasil">Output string</param>
    ''' <param name="maxRetry">Maksimum percobaan</param>
    ''' <param name="delayMs">Delay antar percobaan (ms)</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function CobaMuatDenganRetry(
        pathFile As String,
        ByRef hasil As String,
        Optional maxRetry As Integer = 3,
        Optional delayMs As Integer = 100) As Boolean

        hasil = Nothing

        For i As Integer = 1 To maxRetry
            If CobaMuat(pathFile, hasil) Then
                Return True
            End If

            If i < maxRetry Then
                System.Threading.Thread.Sleep(delayMs)
            End If
        Next

        Return False
    End Function

End Class