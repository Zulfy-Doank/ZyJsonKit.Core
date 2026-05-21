Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' JP02_ParseJObject
''' Mengkonversi string JSON menjadi JObject
''' [OPT01] Eliminasi triple-parse → single parse
''' SEBELUM: Valid[parse#1] → IsObject[parse#2] → JObject.Parse[parse#3]
''' SESUDAH: JObject.Parse[parse#1] → exception sebagai validator
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class JP02_ParseJObject

    ''' <summary>
    ''' Parse string JSON ke JObject
    ''' </summary>
    ''' <param name="jsonString">String JSON object</param>
    ''' <returns>JObject hasil parse, Nothing jika gagal</returns>
    Public Shared Function Parse(jsonString As String) As JObject
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JObject.Parse(jsonString)
        Catch ex As JsonReaderException
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' [OPT01] Parse dengan error handling - ELIMINASI triple parse
    ''' SESUDAH: 1x JObject.Parse + exception handling
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>JObject hasil parse, Nothing jika gagal</returns>
    Public Shared Function ParseAman(
        jsonString As String,
        ByRef pesanError As String) As JObject

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string kosong"
            Return Nothing
        End If

        Try
            Return JObject.Parse(jsonString)
        Catch ex As JsonReaderException
            pesanError = $"Format JSON tidak valid atau bukan object: {ex.Message}"
            Return Nothing
        Catch ex As Exception
            pesanError = $"Gagal parse JObject: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Parse dari file path
    ''' </summary>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>JObject, Nothing jika gagal</returns>
    Public Shared Function ParseDariFile(pathFile As String) As JObject
        If Not IO.File.Exists(pathFile) Then Return Nothing

        Try
            Dim jsonString = IO.File.ReadAllText(pathFile, Text.Encoding.UTF8)
            Return Parse(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Mencoba parse dengan Try pattern
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="hasil">Output JObject</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function CobaParse(
        jsonString As String,
        ByRef hasil As JObject) As Boolean

        hasil = Parse(jsonString)
        Return hasil IsNot Nothing
    End Function

End Class