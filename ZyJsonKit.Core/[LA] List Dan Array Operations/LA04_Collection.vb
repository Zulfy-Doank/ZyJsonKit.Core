Imports Newtonsoft.Json
Imports System.Collections

''' <summary>
''' LA04_Collection
''' Support untuk IEnumerable dan berbagai tipe collection
''' [PF02] Generic-safe
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' [AF01] Generic methods - Reusable
''' </summary>
Public NotInheritable Class LA04_Collection

    ''' <summary>
    ''' Serialisasi IEnumerable ke JSON array
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="koleksi">IEnumerable(Of T)</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>String JSON array</returns>
    Public Shared Function ToJson(Of T)(
        koleksi As IEnumerable(Of T),
        Optional formatRapi As Boolean = True) As String

        If koleksi Is Nothing Then Return "[]"
        If Not koleksi.Any() Then Return "[]"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim pengaturan = New JsonSerializerSettings With {
                .Formatting = jsonFormatting,
                .NullValueHandling = NullValueHandling.Ignore,
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            Return JsonConvert.SerializeObject(koleksi, pengaturan)
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi non-generic IEnumerable ke JSON
    ''' </summary>
    ''' <param name="koleksi">IEnumerable (non-generic)</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>String JSON array</returns>
    Public Shared Function ToJsonNonGeneric(
        koleksi As IEnumerable,
        Optional formatRapi As Boolean = True) As String

        If koleksi Is Nothing Then Return "[]"

        Try
            Dim list As New List(Of Object)()
            For Each item In koleksi
                list.Add(item)
            Next

            If list.Count = 0 Then Return "[]"

            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Return JsonConvert.SerializeObject(list, jsonFormatting)
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi JSON ke berbagai tipe collection
    ''' </summary>
    ''' <typeparam name="TCollection">Tipe collection target</typeparam>
    ''' <typeparam name="TItem">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>Collection dengan tipe yang diminta</returns>
    Public Shared Function FromJson(Of TCollection As {New, ICollection(Of TItem)}, TItem)(
        jsonArray As String) As TCollection

        If String.IsNullOrWhiteSpace(jsonArray) Then Return New TCollection()

        Try
            Dim list = JsonConvert.DeserializeObject(Of List(Of TItem))(jsonArray)

            If list Is Nothing Then Return New TCollection()

            Dim collection As New TCollection()
            For Each item In list
                collection.Add(item)
            Next
            Return collection
        Catch
            Return New TCollection()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON array ke HashSet(Of T)
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>HashSet(Of T)</returns>
    Public Shared Function ToHashSet(Of T)(jsonArray As String) As HashSet(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return New HashSet(Of T)()

        Try
            Dim list = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            Return If(list IsNot Nothing, New HashSet(Of T)(list), New HashSet(Of T)())
        Catch
            Return New HashSet(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON array ke Queue(Of T)
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>Queue(Of T)</returns>
    Public Shared Function ToQueue(Of T)(jsonArray As String) As Queue(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return New Queue(Of T)()

        Try
            Dim list = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            Return If(list IsNot Nothing, New Queue(Of T)(list), New Queue(Of T)())
        Catch
            Return New Queue(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON array ke Stack(Of T)
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>Stack(Of T)</returns>
    Public Shared Function ToStack(Of T)(jsonArray As String) As Stack(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return New Stack(Of T)()

        Try
            Dim list = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            Return If(list IsNot Nothing, New Stack(Of T)(list), New Stack(Of T)())
        Catch
            Return New Stack(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON array ke LinkedList(Of T)
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>LinkedList(Of T)</returns>
    Public Shared Function ToLinkedList(Of T)(jsonArray As String) As LinkedList(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return New LinkedList(Of T)()

        Try
            Dim list = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            Return If(list IsNot Nothing, New LinkedList(Of T)(list), New LinkedList(Of T)())
        Catch
            Return New LinkedList(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi collection ke DataTable
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="koleksi">IEnumerable(Of T)</param>
    ''' <returns>DataTable</returns>
    Public Shared Function ToDataTable(Of T)(
        koleksi As IEnumerable(Of T)) As System.Data.DataTable

        Dim dt As New System.Data.DataTable()

        If koleksi Is Nothing Then Return dt

        Try
            Dim tipe = GetType(T)
            Dim properti = tipe.GetProperties()

            ' Buat kolom
            For Each prop In properti
                Dim columnType = If(
                    Nullable.GetUnderlyingType(prop.PropertyType),
                    prop.PropertyType)
                dt.Columns.Add(prop.Name, columnType)
            Next

            ' Isi baris
            For Each item In koleksi
                Dim row = dt.NewRow()
                For Each prop In properti
                    Dim value = prop.GetValue(item, Nothing)
                    row(prop.Name) = If(value, DBNull.Value)
                Next
                dt.Rows.Add(row)
            Next
        Catch
        End Try

        Return dt
    End Function

    ''' <summary>
    ''' Mengecek apakah collection null atau kosong
    ''' </summary>
    ''' <param name="koleksi">IEnumerable yang dicek</param>
    ''' <returns>True jika null atau kosong</returns>
    Public Shared Function ApakahKosong(koleksi As IEnumerable) As Boolean
        If koleksi Is Nothing Then Return True

        Try
            Return Not koleksi.GetEnumerator().MoveNext()
        Catch
            Return True
        End Try
    End Function

    ''' <summary>
    ''' Menghitung jumlah item dengan aman
    ''' </summary>
    ''' <param name="koleksi">IEnumerable yang dihitung</param>
    ''' <returns>Jumlah item, 0 jika null atau error</returns>
    Public Shared Function HitungItem(koleksi As IEnumerable) As Integer
        If koleksi Is Nothing Then Return 0

        Try
            Dim count As Integer = 0
            For Each item In koleksi
                count += 1
            Next
            Return count
        Catch
            Return 0
        End Try
    End Function

End Class