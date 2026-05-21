Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json.Linq
Imports System.Dynamic

''' <summary>
''' DY01_Dynamic deserialize
''' Deserialisasi JSON ke dynamic object (ExpandoObject)
''' Berguna saat struktur JSON tidak diketahui di compile time
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DY01_DynamicDeserialize
    ''' <summary>
    ''' Deserialisasi JSON ke dynamic object
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>ExpandoObject dynamic, Nothing jika gagal</returns>
    Public Shared Function FromJson(jsonString As String) As ExpandoObject
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JsonConvert.DeserializeObject(Of ExpandoObject)(jsonString, New ExpandoObjectConverter())
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi JSON array ke List of ExpandoObject
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>List of ExpandoObject</returns>
    Public Shared Function FromJsonArray(jsonArray As String) As List(Of ExpandoObject)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return New List(Of ExpandoObject)()

        Try
            Return JsonConvert.DeserializeObject(Of List(Of ExpandoObject))(jsonArray, New ExpandoObjectConverter())
        Catch
            Return New List(Of ExpandoObject)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi JSON ke JToken (dynamic JSON)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JToken, Nothing jika gagal</returns>
    Public Shared Function FromJsonKeJToken(jsonString As String) As JToken
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JToken.Parse(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Akses properti dynamic dengan aman
    ''' </summary>
    ''' <typeparam name="T">Tipe value</typeparam>
    ''' <param name="expando">ExpandoObject</param>
    ''' <param name="namaProperti">Nama properti</param>
    ''' <param name="nilaiDefault">Nilai default</param>
    ''' <returns>Value atau default</returns>
    Public Shared Function AmbilValue(Of T)(expando As ExpandoObject, namaProperti As String, Optional nilaiDefault As T = Nothing) As T
        If expando Is Nothing OrElse String.IsNullOrWhiteSpace(namaProperti) Then Return nilaiDefault

        Try
            Dim dict = DirectCast(expando, IDictionary(Of String, Object))
            If dict.ContainsKey(namaProperti) Then
                Dim value = dict(namaProperti)
                If value IsNot Nothing Then
                    Return DirectCast(Convert.ChangeType(value, GetType(T)), T)
                End If
            End If
        Catch
        End Try

        Return nilaiDefault
    End Function
End Class