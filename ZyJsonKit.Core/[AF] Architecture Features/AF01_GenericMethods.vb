Imports Newtonsoft.Json

''' <summary>
''' AF01_Generic methods
''' Method generic reusable untuk berbagai tipe
''' </summary>
Public NotInheritable Class AF01_GenericMethods
    ''' <summary>
    ''' Generic serialize dengan constraint Class
    ''' </summary>
    Public Shared Function Serialize(Of T As Class)(obj As T, Optional formatRapi As Boolean = True) As String
        If obj Is Nothing Then Return "null"

        Try
            Return JsonConvert.SerializeObject(obj, If(formatRapi, Formatting.Indented, Formatting.None))
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Generic deserialize dengan constraint Class
    ''' </summary>
    Public Shared Function Deserialize(Of T As Class)(jsonString As String) As T
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JsonConvert.DeserializeObject(Of T)(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Generic list serialize
    ''' </summary>
    Public Shared Function SerializeList(Of T)(list As List(Of T), Optional formatRapi As Boolean = True) As String
        If list Is Nothing OrElse list.Count = 0 Then Return "[]"

        Try
            Return JsonConvert.SerializeObject(list, If(formatRapi, Formatting.Indented, Formatting.None))
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Generic list deserialize
    ''' </summary>
    Public Shared Function DeserializeList(Of T)(jsonArray As String) As List(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return New List(Of T)()

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            Return If(hasil, New List(Of T)())
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Generic dictionary serialize
    ''' </summary>
    Public Shared Function SerializeDictionary(Of TKey, TValue)(dict As Dictionary(Of TKey, TValue), Optional formatRapi As Boolean = True) As String
        If dict Is Nothing OrElse dict.Count = 0 Then Return "{}"

        Try
            Return JsonConvert.SerializeObject(dict, If(formatRapi, Formatting.Indented, Formatting.None))
        Catch
            Return "{}"
        End Try
    End Function
End Class