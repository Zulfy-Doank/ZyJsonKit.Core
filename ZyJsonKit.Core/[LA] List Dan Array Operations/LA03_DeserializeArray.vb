Imports Newtonsoft.Json

''' <summary>
''' LA03_DeserializeArray
''' Deserialisasi JSON array ke Array T()
''' [PF02] Generic-safe
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class LA03_DeserializeArray

    ''' <summary>
    ''' Deserialisasi JSON array ke Array T()
    ''' </summary>
    ''' <typeparam name="T">Tipe item dalam array</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="settings">Pengaturan serializer (opsional)</param>
    ''' <returns>Array T(), array kosong jika gagal</returns>
    Public Shared Function FromJson(Of T)(
        jsonArray As String,
        Optional settings As JsonSerializerSettings = Nothing) As T()

        If String.IsNullOrWhiteSpace(jsonArray) Then
            Return Array.Empty(Of T)()
        End If

        Try
            Dim pengaturan = If(settings, New JsonSerializerSettings With {
                .MissingMemberHandling = MissingMemberHandling.Ignore,
                .NullValueHandling = NullValueHandling.Ignore
            })

            Dim list = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray, pengaturan)
            Return If(list IsNot Nothing, list.ToArray(), Array.Empty(Of T)())
        Catch
            Return Array.Empty(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi JSON array ke array dengan tipe spesifik (runtime type)
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="tipeItem">Tipe item dalam array</param>
    ''' <returns>Array sebagai Object, Nothing jika gagal</returns>
    Public Shared Function FromJsonTyped(
        jsonArray As String,
        tipeItem As Type) As Array

        If String.IsNullOrWhiteSpace(jsonArray) OrElse tipeItem Is Nothing Then
            Return Nothing
        End If

        Try
            Dim tipeList = GetType(List(Of )).MakeGenericType(tipeItem)
            Dim list = DirectCast(
                JsonConvert.DeserializeObject(jsonArray, tipeList),
                System.Collections.IList)

            If list Is Nothing Then Return Nothing

            Dim arr = Array.CreateInstance(tipeItem, list.Count)
            list.CopyTo(arr, 0)
            Return arr
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dari file JSON ke Array
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>Array T()</returns>
    Public Shared Function FromFile(Of T)(pathFile As String) As T()
        If Not IO.File.Exists(pathFile) Then Return Array.Empty(Of T)()

        Try
            Dim jsonString = IO.File.ReadAllText(pathFile, Text.Encoding.UTF8)
            Return FromJson(Of T)(jsonString)
        Catch
            Return Array.Empty(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan batas jumlah item
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="maxItem">Maksimum item yang diambil</param>
    ''' <returns>Array T()</returns>
    Public Shared Function FromJsonDenganBatas(Of T)(
        jsonArray As String,
        maxItem As Integer) As T()

        Dim arr = FromJson(Of T)(jsonArray)

        If arr.Length <= maxItem Then Return arr

        Dim hasil(maxItem - 1) As T
        Array.Copy(arr, hasil, maxItem)
        Return hasil
    End Function

    ''' <summary>
    ''' Deserialisasi dan konversi ke array 2D
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array of arrays</param>
    ''' <returns>Array 2D T(,), Nothing jika gagal</returns>
    Public Shared Function FromJson2D(Of T)(jsonArray As String) As T(,)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return Nothing

        Try
            Dim listOfLists = JsonConvert.DeserializeObject(Of List(Of List(Of T)))(jsonArray)

            If listOfLists Is Nothing OrElse listOfLists.Count = 0 Then
                Return Nothing
            End If

            Dim baris = listOfLists.Count
            Dim kolom = listOfLists(0).Count
            Dim hasil(baris - 1, kolom - 1) As T

            For i As Integer = 0 To baris - 1
                For j As Integer = 0 To Math.Min(kolom, listOfLists(i).Count) - 1
                    hasil(i, j) = listOfLists(i)(j)
                Next
            Next

            Return hasil
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi array ke JSON
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="arr">Array</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>String JSON array</returns>
    Public Shared Function ToJson(Of T)(
        arr As T(),
        Optional formatRapi As Boolean = True) As String

        If arr Is Nothing OrElse arr.Length = 0 Then Return "[]"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Return JsonConvert.SerializeObject(arr, jsonFormatting)
        Catch
            Return "[]"
        End Try
    End Function

End Class