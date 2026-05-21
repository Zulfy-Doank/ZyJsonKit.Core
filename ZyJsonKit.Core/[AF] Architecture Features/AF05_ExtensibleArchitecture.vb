Imports Newtonsoft.Json

''' <summary>
''' AF05_Extensible architecture
''' Arsitektur yang mudah dikembangkan (extensible)
''' Base class dan interface untuk custom implementation
''' </summary>
Public NotInheritable Class AF05_ExtensibleArchitecture
    ''' <summary>
    ''' Interface untuk custom serializer
    ''' </summary>
    Public Interface ICustomSerializer
        Function Serialize(obj As Object) As String
        Function Deserialize(Of T)(jsonString As String) As T
    End Interface

    ''' <summary>
    ''' Base class untuk custom serializer
    ''' </summary>
    Public MustInherit Class CustomSerializerBase
        Implements ICustomSerializer

        Public MustOverride Function Serialize(obj As Object) As String Implements ICustomSerializer.Serialize
        Public MustOverride Function Deserialize(Of T)(jsonString As String) As T Implements ICustomSerializer.Deserialize
    End Class

    ''' <summary>
    ''' Default serializer implementation
    ''' </summary>
    Public Class DefaultSerializer
        Inherits CustomSerializerBase

        Public Overrides Function Serialize(obj As Object) As String
            If obj Is Nothing Then Return "null"
            Try
                Return JsonConvert.SerializeObject(obj, Formatting.Indented)
            Catch
                Return "null"
            End Try
        End Function

        Public Overrides Function Deserialize(Of T)(jsonString As String) As T
            If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing
            Try
                Return JsonConvert.DeserializeObject(Of T)(jsonString)
            Catch
                Return Nothing
            End Try
        End Function
    End Class

    ''' <summary>
    ''' Factory untuk membuat serializer
    ''' </summary>
    Public Shared Function BuatSerializer(Optional tipe As Type = Nothing) As ICustomSerializer
        If tipe IsNot Nothing AndAlso GetType(ICustomSerializer).IsAssignableFrom(tipe) Then
            Try
                Return DirectCast(Activator.CreateInstance(tipe), ICustomSerializer)
            Catch
            End Try
        End If

        Return New DefaultSerializer()
    End Function

    ''' <summary>
    ''' Registrasi custom serializer dengan generic constraint interface
    ''' </summary>
    Public Shared Function RegistrasiSerializer(Of T As ICustomSerializer)() As ICustomSerializer
        Try
            Return DirectCast(Activator.CreateInstance(GetType(T)), ICustomSerializer)
        Catch
            Return New DefaultSerializer()
        End Try
    End Function
End Class