Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' JP04_ParseJToken
''' Mengkonversi string JSON menjadi JToken (fleksibel: object/array/value)
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class JP04_ParseJToken

    ''' <summary>
    ''' Parse string JSON ke JToken (otomatis deteksi tipe)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JToken hasil parse, Nothing jika gagal</returns>
    Public Shared Function Parse(jsonString As String) As JToken
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JToken.Parse(jsonString)
        Catch ex As JsonReaderException
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Parse dengan error handling detail
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>JToken, Nothing jika gagal</returns>
    Public Shared Function ParseAman(
        jsonString As String,
        ByRef pesanError As String) As JToken

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string kosong"
            Return Nothing
        End If

        Try
            Return JToken.Parse(jsonString)
        Catch ex As JsonReaderException
            pesanError = $"Format JSON tidak valid: {ex.Message}"
            Return Nothing
        Catch ex As Exception
            pesanError = $"Error: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Parse dari file
    ''' </summary>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>JToken, Nothing jika gagal</returns>
    Public Shared Function ParseDariFile(pathFile As String) As JToken
        If Not IO.File.Exists(pathFile) Then Return Nothing

        Try
            Dim jsonString = IO.File.ReadAllText(pathFile, Text.Encoding.UTF8)
            Return Parse(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Parse langsung ke tipe spesifik
    ''' Gunakan ini daripada Parse() + TryCast di caller
    ''' </summary>
    ''' <typeparam name="T">Tipe JToken turunan (JObject/JArray/JValue)</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JToken dengan tipe T, Nothing jika gagal/salah tipe</returns>
    Public Shared Function ParseTyped(Of T As JToken)(jsonString As String) As T
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Dim token = JToken.Parse(jsonString)
            Return TryCast(token, T)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deteksi tipe token - perlu full parse, tidak ada shortcut
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JTokenType, Nothing jika invalid</returns>
    Public Shared Function DeteksiTipe(jsonString As String) As JTokenType?
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JToken.Parse(jsonString).Type
        Catch
            Return Nothing
        End Try
    End Function

End Class