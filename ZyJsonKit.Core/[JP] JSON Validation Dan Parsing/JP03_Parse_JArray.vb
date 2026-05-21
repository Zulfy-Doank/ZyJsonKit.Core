Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' JP03_ParseJArray
''' Mengkonversi string JSON menjadi JArray
''' [OPT01] Eliminasi triple-parse → single parse
''' SEBELUM: Valid[parse#1] → IsArray[parse#2] → JArray.Parse[parse#3]
''' SESUDAH: JArray.Parse[parse#1] → exception sebagai validator
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class JP03_ParseJArray

    ''' <summary>
    ''' Parse string JSON ke JArray
    ''' </summary>
    ''' <param name="jsonString">String JSON array</param>
    ''' <returns>JArray hasil parse, Nothing jika gagal</returns>
    Public Shared Function Parse(jsonString As String) As JArray
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JArray.Parse(jsonString)
        Catch ex As JsonReaderException
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' [OPT01] Parse dengan error handling - ELIMINASI triple parse
    ''' SESUDAH: 1x JArray.Parse + exception handling
    ''' </summary>
    ''' <param name="jsonString">String JSON array</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>JArray, Nothing jika gagal</returns>
    Public Shared Function ParseAman(
        jsonString As String,
        ByRef pesanError As String) As JArray

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string kosong"
            Return Nothing
        End If

        Try
            Return JArray.Parse(jsonString)
        Catch ex As JsonReaderException
            pesanError = $"Format JSON tidak valid atau bukan array: {ex.Message}"
            Return Nothing
        Catch ex As Exception
            pesanError = $"Gagal parse JArray: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Parse dari file path
    ''' </summary>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>JArray, Nothing jika gagal</returns>
    Public Shared Function ParseDariFile(pathFile As String) As JArray
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
    ''' <param name="hasil">Output JArray</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function CobaParse(
        jsonString As String,
        ByRef hasil As JArray) As Boolean

        hasil = Parse(jsonString)
        Return hasil IsNot Nothing
    End Function

    ''' <summary>
    ''' Parse dan hitung jumlah item
    ''' </summary>
    ''' <param name="jsonString">String JSON array</param>
    ''' <returns>Jumlah item, -1 jika gagal</returns>
    Public Shared Function HitungItem(jsonString As String) As Integer
        Dim arr = Parse(jsonString)
        If arr Is Nothing Then Return -1
        Return arr.Count
    End Function

End Class