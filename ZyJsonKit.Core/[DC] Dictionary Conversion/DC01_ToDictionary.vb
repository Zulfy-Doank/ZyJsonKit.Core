Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' DC01_ToDictionary
''' Konversi JSON string atau JObject ke Dictionary
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DC01_ToDictionary

    ''' <summary>
    ''' Konversi JSON string ke Dictionary(Of String, Object)
    ''' </summary>
    ''' <param name="jsonString">String JSON object</param>
    ''' <returns>Dictionary(Of String, Object), tidak pernah null</returns>
    Public Shared Function DariJson(jsonString As String) As Dictionary(Of String, Object)
        If String.IsNullOrWhiteSpace(jsonString) Then
            Return New Dictionary(Of String, Object)()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonString)
            Return If(hasil, New Dictionary(Of String, Object)())
        Catch
            Return New Dictionary(Of String, Object)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JObject ke Dictionary(Of String, Object)
    ''' </summary>
    ''' <param name="jObject">JObject</param>
    ''' <returns>Dictionary</returns>
    Public Shared Function DariJObject(jObject As JObject) As Dictionary(Of String, Object)
        If jObject Is Nothing Then Return New Dictionary(Of String, Object)()

        Try
            Return jObject.ToObject(Of Dictionary(Of String, Object))()
        Catch
            Return New Dictionary(Of String, Object)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi Dictionary ke JSON string
    ''' </summary>
    ''' <param name="dictionary">Dictionary</param>
    ''' <param name="formatRapi">Gunakan formatting rapi</param>
    ''' <returns>String JSON</returns>
    Public Shared Function KeJson(
        dictionary As Dictionary(Of String, Object),
        Optional formatRapi As Boolean = True) As String

        If dictionary Is Nothing OrElse dictionary.Count = 0 Then Return "{}"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Return JsonConvert.SerializeObject(dictionary, jsonFormatting)
        Catch
            Return "{}"
        End Try
    End Function

    ''' <summary>
    ''' Konversi Dictionary ke JObject
    ''' </summary>
    ''' <param name="dictionary">Dictionary</param>
    ''' <returns>JObject</returns>
    Public Shared Function KeJObject(
        dictionary As Dictionary(Of String, Object)) As JObject

        If dictionary Is Nothing Then Return New JObject()

        Try
            Return JObject.FromObject(dictionary)
        Catch
            Return New JObject()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON string ke Dictionary(Of String, String)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Dictionary(Of String, String)</returns>
    Public Shared Function DariJsonString(jsonString As String) As Dictionary(Of String, String)
        If String.IsNullOrWhiteSpace(jsonString) Then
            Return New Dictionary(Of String, String)()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(jsonString)
            Return If(hasil, New Dictionary(Of String, String)())
        Catch
            Return New Dictionary(Of String, String)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON string ke Dictionary dengan tipe value spesifik
    ''' </summary>
    ''' <typeparam name="TValue">Tipe value</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Dictionary(Of String, TValue)</returns>
    Public Shared Function DariJsonTyped(Of TValue)(
        jsonString As String) As Dictionary(Of String, TValue)

        If String.IsNullOrWhiteSpace(jsonString) Then
            Return New Dictionary(Of String, TValue)()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of Dictionary(Of String, TValue))(jsonString)
            Return If(hasil, New Dictionary(Of String, TValue)())
        Catch
            Return New Dictionary(Of String, TValue)()
        End Try
    End Function

    ''' <summary>
    ''' Mengecek apakah Dictionary kosong atau null
    ''' </summary>
    ''' <param name="dictionary">Dictionary yang dicek</param>
    ''' <returns>True jika null atau kosong</returns>
    Public Shared Function ApakahKosong(
        dictionary As Dictionary(Of String, Object)) As Boolean

        Return dictionary Is Nothing OrElse dictionary.Count = 0
    End Function

    ''' <summary>
    ''' Mendapatkan value dari Dictionary dengan aman
    ''' </summary>
    ''' <param name="dictionary">Dictionary sumber</param>
    ''' <param name="key">Key yang dicari</param>
    ''' <param name="nilaiDefault">Nilai default jika tidak ditemukan</param>
    ''' <returns>Value atau nilaiDefault</returns>
    Public Shared Function AmbilValue(
        dictionary As Dictionary(Of String, Object),
        key As String,
        Optional nilaiDefault As Object = Nothing) As Object

        If dictionary Is Nothing OrElse String.IsNullOrWhiteSpace(key) Then
            Return nilaiDefault
        End If

        If dictionary.ContainsKey(key) Then
            Return If(dictionary(key), nilaiDefault)
        End If

        Return nilaiDefault
    End Function

End Class