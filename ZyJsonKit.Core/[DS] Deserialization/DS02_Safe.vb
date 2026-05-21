Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' DS02_Safe
''' Deserialisasi aman dengan error handling lengkap
''' [PF03] Exception-safe - Tangani semua exception
''' [PF04] Null-safe - Validasi null menyeluruh
''' </summary>
Public NotInheritable Class DS02_Safe

    ''' <summary>
    ''' Deserialisasi aman dengan pesan error detail
    ''' </summary>
    ''' <typeparam name="T">Tipe object target</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>Object hasil deserialisasi, Nothing jika gagal</returns>
    Public Shared Function FromJson(Of T)(
        jsonString As String,
        ByRef pesanError As String) As T

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string tidak boleh kosong"
            Return Nothing
        End If

        If Not JP01_Validate.ApakahValid(jsonString) Then
            pesanError = "Format JSON tidak valid"
            Return Nothing
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of T)(jsonString)
            If hasil Is Nothing Then
                pesanError = "Hasil deserialisasi null (mungkin struktur JSON tidak cocok)"
            End If
            Return hasil
        Catch ex As JsonSerializationException
            pesanError = $"Error serialisasi: {ex.Message}"
            Return Nothing
        Catch ex As JsonReaderException
            pesanError = $"Error membaca JSON: {ex.Message}"
            Return Nothing
        Catch ex As Exception
            pesanError = $"Error tidak terduga: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi aman dengan logging error
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="logError">Action untuk mencatat error</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonDenganLog(Of T)(
        jsonString As String,
        logError As Action(Of String)) As T

        Dim pesanError As String = String.Empty
        Dim hasil = FromJson(Of T)(jsonString, pesanError)

        If Not String.IsNullOrEmpty(pesanError) AndAlso logError IsNot Nothing Then
            logError(pesanError)
        End If

        Return hasil
    End Function

    ''' <summary>
    ''' Deserialisasi aman dengan pengaturan kustom
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="settings">Pengaturan serializer</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonDenganSettings(Of T)(
        jsonString As String,
        settings As JsonSerializerSettings,
        ByRef pesanError As String) As T

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string kosong"
            Return Nothing
        End If

        If settings Is Nothing Then
            pesanError = "Settings tidak boleh null"
            Return Nothing
        End If

        Try
            Return JsonConvert.DeserializeObject(Of T)(jsonString, settings)
        Catch ex As Exception
            pesanError = $"Gagal deserialisasi: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi aman dengan validasi struktur
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="daftarError">Output list error</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonDenganValidasi(Of T)(
        jsonString As String,
        ByRef daftarError As List(Of String)) As T

        daftarError = New List(Of String)()

        If String.IsNullOrWhiteSpace(jsonString) Then
            daftarError.Add("JSON string kosong")
            Return Nothing
        End If

        If Not JP01_Validate.ApakahValid(jsonString) Then
            daftarError.Add("Format JSON tidak valid")
            Return Nothing
        End If

        Try
            Dim token = JToken.Parse(jsonString)
            If token.Type = JTokenType.Object Then
                Dim jObj = DirectCast(token, JObject)
                If jObj.Count = 0 Then
                    daftarError.Add("JSON object kosong (tidak memiliki properti)")
                End If
            End If
        Catch ex As Exception
            daftarError.Add($"Error validasi struktur: {ex.Message}")
            Return Nothing
        End Try

        Try
            Return JsonConvert.DeserializeObject(Of T)(jsonString)
        Catch ex As Exception
            daftarError.Add($"Gagal deserialisasi: {ex.Message}")
            Return Nothing
        End Try
    End Function

End Class