Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

''' <summary>
''' FO04_ReadAsync
''' Membaca file JSON secara asynchronous
''' [PF05] Async-ready - Full async support
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class FO04_ReadAsync

    ''' <summary>
    ''' Membaca file JSON async
    ''' </summary>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>Task(Of String) - String JSON, Nothing jika gagal</returns>
    Public Shared Async Function BacaAsync(pathFile As String) As Task(Of String)
        If String.IsNullOrWhiteSpace(pathFile) Then Return Nothing
        If Not File.Exists(pathFile) Then Return Nothing

        Try
            Using reader As New StreamReader(pathFile, Encoding.UTF8)
                Return Await reader.ReadToEndAsync()
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON dan deserialize async
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="pathFile">Path file</param>
    ''' <returns>Task(Of T) - Object hasil</returns>
    Public Shared Async Function BacaObjectAsync(Of T)(pathFile As String) As Task(Of T)
        Dim jsonString = Await BacaAsync(pathFile)
        If jsonString Is Nothing Then Return Nothing

        Try
            Return Await Task.Run(
                Function() JsonConvert.DeserializeObject(Of T)(jsonString))
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON async dengan error handling
    ''' Tuple karena ByRef tidak bisa di async
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <returns>Task(Of (Hasil As String, PesanError As String))</returns>
    Public Shared Async Function BacaAmanAsync(
        pathFile As String) As Task(Of (Hasil As String, PesanError As String))

        If String.IsNullOrWhiteSpace(pathFile) Then
            Return (Nothing, "Path file tidak boleh kosong")
        End If

        If Not File.Exists(pathFile) Then
            Return (Nothing, $"File tidak ditemukan: {pathFile}")
        End If

        Try
            Using reader As New StreamReader(pathFile, Encoding.UTF8)
                Dim hasil = Await reader.ReadToEndAsync()
                Return (hasil, String.Empty)
            End Using
        Catch ex As FileNotFoundException
            Return (Nothing, $"File tidak ditemukan: {ex.Message}")
        Catch ex As UnauthorizedAccessException
            Return (Nothing, $"Akses ditolak: {ex.Message}")
        Catch ex As IOException
            Return (Nothing, $"Error membaca file: {ex.Message}")
        Catch ex As Exception
            Return (Nothing, $"Error: {ex.Message}")
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON async dengan callback
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="onSuccess">Callback menerima hasil</param>
    ''' <param name="onError">Callback menerima pesan error</param>
    ''' <returns>Task(Of Boolean)</returns>
    Public Shared Async Function BacaAmanDenganCallback(
        pathFile As String,
        onSuccess As Action(Of String),
        onError As Action(Of String)) As Task(Of Boolean)

        If String.IsNullOrWhiteSpace(pathFile) Then
            onError?.Invoke("Path file tidak boleh kosong")
            Return False
        End If

        If Not File.Exists(pathFile) Then
            onError?.Invoke($"File tidak ditemukan: {pathFile}")
            Return False
        End If

        Try
            Using reader As New StreamReader(pathFile, Encoding.UTF8)
                Dim hasil = Await reader.ReadToEndAsync()
                onSuccess?.Invoke(hasil)
                Return True
            End Using
        Catch ex As Exception
            onError?.Invoke(ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON per baris async (untuk file besar)
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <returns>Task(Of String) - String JSON gabungan</returns>
    Public Shared Async Function BacaPerBarisAsync(pathFile As String) As Task(Of String)
        If String.IsNullOrWhiteSpace(pathFile) Then Return Nothing
        If Not File.Exists(pathFile) Then Return Nothing

        Try
            Dim sb As New StringBuilder()
            Using reader As New StreamReader(pathFile, Encoding.UTF8)
                Dim baris As String = Await reader.ReadLineAsync()
                While baris IsNot Nothing
                    sb.AppendLine(baris)
                    baris = Await reader.ReadLineAsync()
                End While
            End Using
            Return sb.ToString()
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membaca banyak file JSON sekaligus async
    ''' </summary>
    ''' <param name="daftarPath">Daftar path file</param>
    ''' <returns>Task(Of Dictionary(Of String, String))</returns>
    Public Shared Async Function BacaBanyakAsync(
        daftarPath As IEnumerable(Of String)) As Task(Of Dictionary(Of String, String))

        Dim hasil As New Dictionary(Of String, String)()

        If daftarPath Is Nothing Then Return hasil

        Dim tasks As New List(Of Task(Of String))()
        Dim paths As New List(Of String)()

        For Each path In daftarPath
            paths.Add(path)
            tasks.Add(BacaAsync(path))
        Next

        Dim jsonStrings = Await Task.WhenAll(tasks)

        For i As Integer = 0 To paths.Count - 1
            If jsonStrings(i) IsNot Nothing Then
                hasil(paths(i)) = jsonStrings(i)
            End If
        Next

        Return hasil
    End Function

End Class