Imports Newtonsoft.Json
Imports System.Threading.Tasks

''' <summary>
''' PF05_Async-ready
''' Method yang siap untuk operasi async/await
''' </summary>
Public NotInheritable Class PF05_AsyncReady
    ''' <summary>
    ''' Serialisasi async
    ''' </summary>
    Public Shared Async Function SerializeAsync(Of T)(obj As T, Optional formatRapi As Boolean = True) As Task(Of String)
        If obj Is Nothing Then Return "null"

        Return Await Task.Run(Function()
                                  Try
                                      Return JsonConvert.SerializeObject(obj, If(formatRapi, Formatting.Indented, Formatting.None))
                                  Catch
                                      Return "null"
                                  End Try
                              End Function)
    End Function

    ''' <summary>
    ''' Deserialisasi async
    ''' </summary>
    Public Shared Async Function DeserializeAsync(Of T)(jsonString As String) As Task(Of T)
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Return Await Task.Run(Function()
                                  Try
                                      Return JsonConvert.DeserializeObject(Of T)(jsonString)
                                  Catch
                                      Return Nothing
                                  End Try
                              End Function)
    End Function

    ''' <summary>
    ''' Deep clone async
    ''' </summary>
    Public Shared Async Function CloneAsync(Of T)(obj As T) As Task(Of T)
        If obj Is Nothing Then Return Nothing

        Dim json = Await SerializeAsync(obj, False)
        Return Await DeserializeAsync(Of T)(json)
    End Function

    ''' <summary>
    ''' Batch serialize async
    ''' </summary>
    Public Shared Async Function SerializeBatchAsync(Of T)(daftarObj As IEnumerable(Of T)) As Task(Of List(Of String))
        Dim hasil As New List(Of String)()

        If daftarObj Is Nothing Then Return hasil

        Dim tasks = daftarObj.Select(Function(x) SerializeAsync(x, False)).ToList()
        Dim jsons = Await Task.WhenAll(tasks)
        hasil.AddRange(jsons)

        Return hasil
    End Function
End Class