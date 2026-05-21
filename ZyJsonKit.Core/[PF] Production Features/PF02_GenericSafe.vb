Imports Newtonsoft.Json

''' <summary>
''' PF02_Generic-safe
''' Method generic yang aman dengan constraint yang tepat
''' </summary>
Public NotInheritable Class PF02_GenericSafe

    ''' <summary>
    ''' Serialize generic dengan constraint Class
    ''' </summary>
    Public Shared Function Serialize(Of T As Class)(
        obj As T,
        Optional formatRapi As Boolean = True) As String

        If obj Is Nothing Then Return "null"

        Try
            ' ✅ Fix BC30980: Ganti nama variabel agar tidak konflik dengan Formatting enum
            Dim jsonFormatting As Formatting = If(formatRapi, Formatting.Indented, Formatting.None)
            Return JsonConvert.SerializeObject(obj, jsonFormatting)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Deserialize generic dengan constraint Class
    ''' </summary>
    Public Shared Function Deserialize(Of T As Class)(
        jsonString As String) As T

        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Return JsonConvert.DeserializeObject(Of T)(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialize generic dengan constraint New
    ''' </summary>
    Public Shared Function DeserializeDenganDefault(Of T As {Class, New})(
        jsonString As String) As T

        If String.IsNullOrWhiteSpace(jsonString) Then Return New T()

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of T)(jsonString)

            ' ✅ IDE0029 Fix: Null check disederhanakan
            Return If(hasil, New T())
        Catch
            Return New T()
        End Try
    End Function

    ''' <summary>
    ''' Clone generic dengan constraint Class
    ''' </summary>
    Public Shared Function Clone(Of T As Class)(obj As T) As T
        If obj Is Nothing Then Return Nothing

        Try
            Dim json = JsonConvert.SerializeObject(obj)
            Return JsonConvert.DeserializeObject(Of T)(json)
        Catch
            Return Nothing
        End Try
    End Function

End Class