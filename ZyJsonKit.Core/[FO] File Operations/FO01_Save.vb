Imports System.IO
Imports System.Text
Imports Newtonsoft.Json

''' <summary>
''' FO01_Save
''' Menyimpan string JSON ke file
''' [PF03] Exception-safe - Handle semua error file
''' [PF04] Null-safe - Validasi input
''' [AF02] Reusable design
''' </summary>
Public NotInheritable Class FO01_Save

    ''' <summary>
    ''' Menyimpan string JSON ke file
    ''' </summary>
    ''' <param name="pathFile">Path file tujuan</param>
    ''' <param name="jsonString">String JSON yang akan disimpan</param>
    ''' <returns>True jika berhasil, False jika gagal</returns>
    Public Shared Function Simpan(pathFile As String, jsonString As String) As Boolean
        If String.IsNullOrWhiteSpace(pathFile) Then Return False
        If jsonString Is Nothing Then Return False

        Try
            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso
               Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            File.WriteAllText(pathFile, jsonString, Encoding.UTF8)
            Return True
        Catch ex As UnauthorizedAccessException
            Return False
        Catch ex As IOException
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan object ke file JSON (auto serialize)
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="pathFile">Path file tujuan</param>
    ''' <param name="obj">Object yang akan disimpan</param>
    ''' <param name="formatRapi">Gunakan formatting rapi</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function SimpanObject(Of T)(
        pathFile As String,
        obj As T,
        Optional formatRapi As Boolean = True) As Boolean

        If obj Is Nothing Then Return False

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim jsonString = JsonConvert.SerializeObject(obj, jsonFormatting)
            Return Simpan(pathFile, jsonString)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan JSON dengan error handling detail
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function SimpanAman(
        pathFile As String,
        jsonString As String,
        ByRef pesanError As String) As Boolean

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(pathFile) Then
            pesanError = "Path file tidak boleh kosong"
            Return False
        End If

        If jsonString Is Nothing Then
            pesanError = "JSON string tidak boleh null"
            Return False
        End If

        Try
            Dim invalidChars = Path.GetInvalidPathChars()
            If pathFile.IndexOfAny(invalidChars) >= 0 Then
                pesanError = "Path mengandung karakter tidak valid"
                Return False
            End If

            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso
               Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            File.WriteAllText(pathFile, jsonString, Encoding.UTF8)
            Return True
        Catch ex As UnauthorizedAccessException
            pesanError = $"Akses ditolak: {ex.Message}"
            Return False
        Catch ex As PathTooLongException
            pesanError = "Path file terlalu panjang"
            Return False
        Catch ex As IOException
            pesanError = $"Error IO: {ex.Message}"
            Return False
        Catch ex As Exception
            pesanError = $"Error: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan JSON dengan opsi overwrite
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="timpaFile">True untuk timpa file yang sudah ada</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function SimpanDenganOpsi(
        pathFile As String,
        jsonString As String,
        Optional timpaFile As Boolean = True) As Boolean

        If String.IsNullOrWhiteSpace(pathFile) Then Return False

        If File.Exists(pathFile) AndAlso Not timpaFile Then
            Return False
        End If

        Return Simpan(pathFile, jsonString)
    End Function

    ''' <summary>
    ''' Menyimpan JSON ke file temporary
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Path file temporary, Nothing jika gagal</returns>
    Public Shared Function SimpanKeTemp(jsonString As String) As String
        If jsonString Is Nothing Then Return Nothing

        Try
            Dim tempPath = Path.GetTempFileName()
            File.WriteAllText(tempPath, jsonString, Encoding.UTF8)
            Return tempPath
        Catch
            Return Nothing
        End Try
    End Function

End Class