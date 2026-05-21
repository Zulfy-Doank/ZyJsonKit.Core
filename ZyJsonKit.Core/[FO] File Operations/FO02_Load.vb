Imports System.IO
Imports System.Text
Imports Newtonsoft.Json

''' <summary>
''' FO02_Load
''' Memuat string JSON dari file
''' [PF03] Exception-safe - Handle error file
''' [PF04] Null-safe - Validasi input
''' </summary>
Public NotInheritable Class FO02_Load

    ''' <summary>
    ''' Membaca file JSON menjadi string
    ''' </summary>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>String JSON, Nothing jika gagal</returns>
    Public Shared Function Muat(pathFile As String) As String
        If String.IsNullOrWhiteSpace(pathFile) Then Return Nothing
        If Not File.Exists(pathFile) Then Return Nothing

        Try
            Return File.ReadAllText(pathFile, Encoding.UTF8)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON dan deserialize ke object
    ''' </summary>
    ''' <typeparam name="T">Tipe object target</typeparam>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>Object hasil deserialisasi, Nothing jika gagal</returns>
    Public Shared Function MuatObject(Of T)(pathFile As String) As T
        Dim jsonString = Muat(pathFile)
        If jsonString Is Nothing Then Return Nothing

        Try
            Return JsonConvert.DeserializeObject(Of T)(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON dengan error handling
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>String JSON, Nothing jika gagal</returns>
    Public Shared Function MuatAman(
        pathFile As String,
        ByRef pesanError As String) As String

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(pathFile) Then
            pesanError = "Path file tidak boleh kosong"
            Return Nothing
        End If

        If Not File.Exists(pathFile) Then
            pesanError = $"File tidak ditemukan: {pathFile}"
            Return Nothing
        End If

        ' Cek ekstensi file
        Dim ekstensi = Path.GetExtension(pathFile).ToLowerInvariant()
        If ekstensi <> ".json" AndAlso ekstensi <> ".txt" Then
            pesanError = $"Ekstensi file tidak didukung: {ekstensi}. Gunakan .json atau .txt"
            ' Tidak return false, tetap coba baca
        End If

        Try
            Dim fileInfo As New FileInfo(pathFile)
            If fileInfo.Length > 100 * 1024 * 1024 Then ' 100 MB
                pesanError = "Warning: File sangat besar (>100MB), mungkin memakan memory banyak"
            End If

            Return File.ReadAllText(pathFile, Encoding.UTF8)
        Catch ex As UnauthorizedAccessException
            pesanError = $"Akses ditolak: {ex.Message}"
            Return Nothing
        Catch ex As FileNotFoundException
            pesanError = $"File tidak ditemukan: {ex.Message}"
            Return Nothing
        Catch ex As IOException
            pesanError = $"Error membaca file: {ex.Message}"
            Return Nothing
        Catch ex As Exception
            pesanError = $"Error: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON dengan fallback encoding
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="encoding">Encoding (default UTF8)</param>
    ''' <returns>String JSON</returns>
    Public Shared Function MuatDenganEncoding(
        pathFile As String,
        Optional encoding As Encoding = Nothing) As String

        If String.IsNullOrWhiteSpace(pathFile) Then Return Nothing
        If Not File.Exists(pathFile) Then Return Nothing

        Dim enc = If(encoding, Encoding.UTF8)

        Try
            Return File.ReadAllText(pathFile, enc)
        Catch
            Try
                Return File.ReadAllText(pathFile, Encoding.UTF8)
            Catch
                Return Nothing
            End Try
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON menjadi JToken
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <returns>JToken, Nothing jika gagal</returns>
    Public Shared Function MuatJToken(
        pathFile As String) As Newtonsoft.Json.Linq.JToken

        Dim jsonString = Muat(pathFile)
        If jsonString Is Nothing Then Return Nothing

        Try
            Return Newtonsoft.Json.Linq.JToken.Parse(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

End Class