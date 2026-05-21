Imports Newtonsoft.Json

''' <summary>
''' DS04_Try
''' Deserialisasi dengan pattern Try (return success flag)
''' Tidak throw exception, cocok untuk conditional flow
''' [PF03] Exception-safe - Tidak pernah throw
''' [PF04] Null-safe - Handle null dengan aman
''' </summary>
Public NotInheritable Class DS04_Try

    ''' <summary>
    ''' Mencoba deserialisasi dengan pattern Try
    ''' </summary>
    ''' <typeparam name="T">Tipe object target</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="hasil">Output object hasil (Nothing jika gagal)</param>
    ''' <returns>True jika berhasil deserialisasi</returns>
    Public Shared Function TryDeserialize(Of T)(
        jsonString As String,
        ByRef hasil As T) As Boolean

        hasil = Nothing
        If String.IsNullOrWhiteSpace(jsonString) Then Return False

        Try
            hasil = JsonConvert.DeserializeObject(Of T)(jsonString)
            Return hasil IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Try deserialize dengan output error message
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="hasil">Output object hasil</param>
    ''' <param name="pesanError">Output pesan error jika gagal</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function TryDeserializeDenganError(Of T)(
        jsonString As String,
        ByRef hasil As T,
        ByRef pesanError As String) As Boolean

        hasil = Nothing
        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string kosong"
            Return False
        End If

        Try
            hasil = JsonConvert.DeserializeObject(Of T)(jsonString)
            If hasil IsNot Nothing Then
                Return True
            Else
                pesanError = "Hasil deserialisasi null"
                Return False
            End If
        Catch ex As Exception
            pesanError = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Try deserialize dengan pengaturan kustom
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="settings">Pengaturan serializer</param>
    ''' <param name="hasil">Output object hasil</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function TryDeserializeDenganSettings(Of T)(
        jsonString As String,
        settings As JsonSerializerSettings,
        ByRef hasil As T) As Boolean

        hasil = Nothing
        If String.IsNullOrWhiteSpace(jsonString) Then Return False
        If settings Is Nothing Then Return False

        Try
            hasil = JsonConvert.DeserializeObject(Of T)(jsonString, settings)
            Return hasil IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Try deserialize dari file
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <param name="hasil">Output object hasil</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function TryDeserializeDariFile(Of T)(
        pathFile As String,
        ByRef hasil As T) As Boolean

        hasil = Nothing
        If Not IO.File.Exists(pathFile) Then Return False

        Try
            Dim jsonString = IO.File.ReadAllText(pathFile, Text.Encoding.UTF8)
            Return TryDeserialize(Of T)(jsonString, hasil)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Try deserialize multiple attempts dengan fallback types
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="hasil">Output object hasil</param>
    ''' <param name="daftarTipe">Daftar tipe yang dicoba berurutan</param>
    ''' <returns>True jika berhasil dengan salah satu tipe</returns>
    Public Shared Function TryDeserializeMultiTipe(
        jsonString As String,
        ByRef hasil As Object,
        ParamArray daftarTipe As Type()) As Boolean

        hasil = Nothing
        If String.IsNullOrWhiteSpace(jsonString) Then Return False
        If daftarTipe Is Nothing Then Return False

        For Each tipe As Type In daftarTipe
            Try
                hasil = JsonConvert.DeserializeObject(jsonString, tipe)
                If hasil IsNot Nothing Then Return True
            Catch
                ' Lanjut ke tipe berikutnya
            End Try
        Next

        Return False
    End Function

End Class