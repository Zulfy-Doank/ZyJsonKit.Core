Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' JP01_Validate
''' Memvalidasi apakah string adalah JSON yang valid
''' [OPT01] Parse SEKALI - tidak double-parse
''' [OPT02] TryCast langsung dari JToken
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class JP01_Validate

    ''' <summary>
    ''' Memeriksa validitas string JSON
    ''' </summary>
    ''' <param name="jsonString">String JSON yang akan divalidasi</param>
    ''' <returns>True jika valid, False jika tidak</returns>
    Public Shared Function ApakahValid(jsonString As String) As Boolean
        If String.IsNullOrWhiteSpace(jsonString) Then Return False

        Try
            JToken.Parse(jsonString)
            Return True
        Catch ex As JsonReaderException
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Memvalidasi dan memberikan pesan error jika tidak valid
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>True jika valid</returns>
    Public Shared Function ValidasiDenganError(
        jsonString As String,
        ByRef pesanError As String) As Boolean

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string tidak boleh kosong"
            Return False
        End If

        Try
            JToken.Parse(jsonString)
            Return True
        Catch ex As JsonReaderException
            pesanError = $"Format JSON tidak valid: {ex.Message}"
            Return False
        Catch ex As Exception
            pesanError = $"Error validasi: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>
    ''' [OPT01] Parse SEKALI → cek tipe via TryCast
    ''' SEBELUM: 2x parse | SESUDAH: 1x parse + TryCast O(1)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>True jika JSON object</returns>
    Public Shared Function ApakahObject(jsonString As String) As Boolean
        If String.IsNullOrWhiteSpace(jsonString) Then Return False

        Try
            Dim token = JToken.Parse(jsonString)
            Return TryCast(token, JObject) IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' [OPT01] Parse SEKALI → cek tipe via TryCast
    ''' SEBELUM: 2x parse | SESUDAH: 1x parse + TryCast O(1)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>True jika JSON array</returns>
    Public Shared Function ApakahArray(jsonString As String) As Boolean
        If String.IsNullOrWhiteSpace(jsonString) Then Return False

        Try
            Dim token = JToken.Parse(jsonString)
            Return TryCast(token, JArray) IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan tipe JSON
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JTokenType, Nothing jika invalid</returns>
    Public Shared Function DapatkanTipe(jsonString As String) As JTokenType?
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JToken.Parse(jsonString).Type
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' [OPT02] Validasi + Parse dalam SATU operasi untuk JObject
    ''' Menghindari parse berulang di caller
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="hasil">Output JObject jika valid</param>
    ''' <returns>True jika valid dan berupa object</returns>
    Public Shared Function CobaParseObject(
        jsonString As String,
        ByRef hasil As JObject) As Boolean

        hasil = Nothing
        If String.IsNullOrWhiteSpace(jsonString) Then Return False

        Try
            hasil = TryCast(JToken.Parse(jsonString), JObject)
            Return hasil IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' [OPT02] Validasi + Parse dalam SATU operasi untuk JArray
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="hasil">Output JArray jika valid</param>
    ''' <returns>True jika valid dan berupa array</returns>
    Public Shared Function CobaParseArray(
        jsonString As String,
        ByRef hasil As JArray) As Boolean

        hasil = Nothing
        If String.IsNullOrWhiteSpace(jsonString) Then Return False

        Try
            hasil = TryCast(JToken.Parse(jsonString), JArray)
            Return hasil IsNot Nothing
        Catch
            Return False
        End Try
    End Function

End Class