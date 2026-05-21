Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' PF01_Option Strict On compatible
''' Method yang kompatibel penuh dengan Option Strict On
''' Semua casting explicit, tidak ada late binding
''' </summary>
Public NotInheritable Class PF01_OptionStrictOn

    ''' <summary>
    ''' Parse JSON dengan Option Strict On
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JObject, tidak pernah null</returns>
    Public Shared Function ParseJSON(jsonString As String) As JObject
        If String.IsNullOrWhiteSpace(jsonString) Then
            Return New JObject()
        End If

        Dim hasil As JToken = JToken.Parse(jsonString)
        Dim jObject As JObject = TryCast(hasil, JObject)

        ' ✅ IDE0029 Fix: Sederhanakan null check dengan 2-parameter If()
        ' Aman karena JObject adalah reference type (Class)
        Return If(jObject, New JObject())
    End Function

    ''' <summary>
    ''' Serialize dengan Option Strict On (semua parameter typed)
    ''' </summary>
    Public Shared Function Serialize(Of T As Class)(
        obj As T,
        formatRapi As Boolean) As String

        If obj Is Nothing Then Return "null"

        Dim settings As New JsonSerializerSettings With {
            .Formatting = If(formatRapi, Formatting.Indented, Formatting.None),
            .NullValueHandling = NullValueHandling.Ignore
        }

        Return JsonConvert.SerializeObject(obj, settings)
    End Function

    ''' <summary>
    ''' Deserialize dengan Option Strict On
    ''' </summary>
    Public Shared Function Deserialize(Of T As Class)(
        jsonString As String) As T

        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Dim hasil As Object = JsonConvert.DeserializeObject(Of T)(jsonString)
        Return DirectCast(hasil, T)
    End Function

    ''' <summary>
    ''' Safe cast dengan Option Strict On
    ''' </summary>
    Public Shared Function SafeCast(Of T)(
        obj As Object,
        ByRef hasil As T) As Boolean

        hasil = Nothing

        If obj Is Nothing Then Return False

        ' ✅ IDE0084 Fix: Gunakan 'IsNot' expression
        ' Lebih idiomatik di VB.NET daripada 'Not TypeOf ... Is'
        If TypeOf obj IsNot T Then Return False

        hasil = DirectCast(obj, T)
        Return True
    End Function

End Class