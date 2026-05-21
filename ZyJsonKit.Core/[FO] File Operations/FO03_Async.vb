Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

''' <summary>
''' FO03_Async
''' Operasi file JSON secara asynchronous
''' [PF05] Async-ready - Full async support
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class FO03_Async

    ''' <summary>
    ''' Menyimpan JSON ke file secara async
    ''' </summary>
    ''' <param name="pathFile">Path file tujuan</param>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Task(Of Boolean) - True jika berhasil</returns>
    Public Shared Async Function SimpanAsync(
        pathFile As String,
        jsonString As String) As Task(Of Boolean)

        If String.IsNullOrWhiteSpace(pathFile) Then Return False
        If jsonString Is Nothing Then Return False

        Try
            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso
               Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            Using writer As New StreamWriter(pathFile, False, Encoding.UTF8)
                Await writer.WriteAsync(jsonString)
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan object ke file JSON async
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="obj">Object</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>Task(Of Boolean)</returns>
    Public Shared Async Function SimpanObjectAsync(Of T)(
        pathFile As String,
        obj As T,
        Optional formatRapi As Boolean = True) As Task(Of Boolean)

        If obj Is Nothing Then Return False

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim jsonString = JsonConvert.SerializeObject(obj, jsonFormatting)
            Return Await SimpanAsync(pathFile, jsonString)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan JSON ke file async dengan error handling
    ''' Tuple karena ByRef tidak bisa di async
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Task(Of (Sukses As Boolean, PesanError As String))</returns>
    Public Shared Async Function SimpanAmanAsync(
        pathFile As String,
        jsonString As String) As Task(Of (Sukses As Boolean, PesanError As String))

        If String.IsNullOrWhiteSpace(pathFile) Then
            Return (False, "Path file tidak boleh kosong")
        End If

        If jsonString Is Nothing Then
            Return (False, "JSON string tidak boleh null")
        End If

        Try
            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso
               Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            Using writer As New StreamWriter(pathFile, False, Encoding.UTF8)
                Await writer.WriteAsync(jsonString)
            End Using
            Return (True, String.Empty)
        Catch ex As UnauthorizedAccessException
            Return (False, $"Akses ditolak: {ex.Message}")
        Catch ex As IOException
            Return (False, $"Error IO: {ex.Message}")
        Catch ex As Exception
            Return (False, $"Error: {ex.Message}")
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan JSON ke file async dengan callback
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="onError">Callback untuk menerima pesan error</param>
    ''' <returns>Task(Of Boolean)</returns>
    Public Shared Async Function SimpanAmanDenganCallback(
        pathFile As String,
        jsonString As String,
        onError As Action(Of String)) As Task(Of Boolean)

        If String.IsNullOrWhiteSpace(pathFile) Then
            onError?.Invoke("Path file tidak boleh kosong")
            Return False
        End If

        If jsonString Is Nothing Then
            onError?.Invoke("JSON string tidak boleh null")
            Return False
        End If

        Try
            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso
               Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            Using writer As New StreamWriter(pathFile, False, Encoding.UTF8)
                Await writer.WriteAsync(jsonString)
            End Using
            Return True
        Catch ex As Exception
            onError?.Invoke(ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan banyak file JSON sekaligus async
    ''' </summary>
    ''' <param name="daftarFile">Dictionary(path, jsonString)</param>
    ''' <returns>Task(Of Integer) - Jumlah file berhasil disimpan</returns>
    Public Shared Async Function SimpanBanyakAsync(
        daftarFile As Dictionary(Of String, String)) As Task(Of Integer)

        If daftarFile Is Nothing OrElse daftarFile.Count = 0 Then Return 0

        Dim tasks As New List(Of Task(Of Boolean))()

        For Each kvp In daftarFile
            tasks.Add(SimpanAsync(kvp.Key, kvp.Value))
        Next

        Dim hasil = Await Task.WhenAll(tasks)
        Return hasil.Count(Function(x) x)
    End Function

End Class