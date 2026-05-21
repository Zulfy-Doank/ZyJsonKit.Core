Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' DC03_ArrayHandling
''' Support konversi JSON array ke berbagai tipe collection
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DC03_ArrayHandling

    ''' <summary>
    ''' Konversi JSON array ke List(Of Dictionary(Of String, Object))
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>List of Dictionary</returns>
    Public Shared Function KeListDictionary(
        jsonArray As String) As List(Of Dictionary(Of String, Object))

        If String.IsNullOrWhiteSpace(jsonArray) Then
            Return New List(Of Dictionary(Of String, Object))()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(
                Of List(Of Dictionary(Of String, Object)))(jsonArray)
            Return If(hasil, New List(Of Dictionary(Of String, Object))())
        Catch
            Return New List(Of Dictionary(Of String, Object))()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON array ke List(Of T)
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>List(Of T)</returns>
    Public Shared Function KeList(Of T)(jsonArray As String) As List(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then
            Return New List(Of T)()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            Return If(hasil, New List(Of T)())
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON array ke Array T()
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>Array T()</returns>
    Public Shared Function KeArray(Of T)(jsonArray As String) As T()
        If String.IsNullOrWhiteSpace(jsonArray) Then
            Return Array.Empty(Of T)()
        End If

        Try
            Dim list = KeList(Of T)(jsonArray)
            Return list.ToArray()
        Catch
            Return Array.Empty(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JArray ke List(Of Dictionary)
    ''' </summary>
    ''' <param name="jArray">JArray</param>
    ''' <returns>List of Dictionary</returns>
    Public Shared Function DariJArray(
        jArray As JArray) As List(Of Dictionary(Of String, Object))

        If jArray Is Nothing Then
            Return New List(Of Dictionary(Of String, Object))()
        End If

        Try
            Dim hasil = jArray.ToObject(Of List(Of Dictionary(Of String, Object)))()
            Return If(hasil, New List(Of Dictionary(Of String, Object))())
        Catch
            Return New List(Of Dictionary(Of String, Object))()
        End Try
    End Function

    ''' <summary>
    ''' Konversi List ke JSON array
    ''' </summary>
    ''' <param name="list">List atau IList</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>String JSON array</returns>
    Public Shared Function KeJson(
        list As System.Collections.IList,
        Optional formatRapi As Boolean = True) As String

        If list Is Nothing OrElse list.Count = 0 Then Return "[]"

        Try
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
    ''' Filter JSON array berdasarkan kondisi
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="kondisi">Callback filter</param>
    ''' <returns>JSON array terfilter</returns>
    Public Shared Function FilterArray(
        jsonArray As String,
        kondisi As Func(Of Dictionary(Of String, Object), Boolean)) As String

        Dim list = KeListDictionary(jsonArray)
        If list.Count = 0 Then Return "[]"

        Try
            Dim filtered = list.Where(kondisi).ToList()
            Return KeJson(filtered)
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Transform JSON array (map)
    ''' </summary>
    ''' <typeparam name="T">Tipe hasil transformasi</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="transform">Callback transformasi</param>
    ''' <returns>List hasil transformasi</returns>
    Public Shared Function TransformArray(Of T)(
        jsonArray As String,
        transform As Func(Of Dictionary(Of String, Object), T)) As List(Of T)

        Dim list = KeListDictionary(jsonArray)
        If list.Count = 0 Then Return New List(Of T)()

        Try
            Return list.Select(transform).ToList()
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Group JSON array by key
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="keySelector">Nama properti untuk grouping</param>
    ''' <returns>Dictionary dengan group</returns>
    Public Shared Function GroupArray(
        jsonArray As String,
        keySelector As String) As Dictionary(Of String, List(Of Dictionary(Of String, Object)))

        Dim hasil As New Dictionary(Of String, List(Of Dictionary(Of String, Object)))()
        Dim list = KeListDictionary(jsonArray)

        Try
            For Each item In list
                If item.ContainsKey(keySelector) Then
                    Dim valueObj As Object = item(keySelector)
                    Dim key As String = If(valueObj IsNot Nothing, valueObj.ToString(), "")

                    If Not hasil.ContainsKey(key) Then
                        hasil(key) = New List(Of Dictionary(Of String, Object))()
                    End If

                    hasil(key).Add(item)
                End If
            Next
        Catch
        End Try

        Return hasil
    End Function

    ''' <summary>
    ''' Sort JSON array by key
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="sortKey">Nama properti untuk sorting</param>
    ''' <param name="ascending">True untuk ascending</param>
    ''' <returns>JSON array terurut</returns>
    Public Shared Function SortArray(
        jsonArray As String,
        sortKey As String,
        Optional ascending As Boolean = True) As String

        Dim list = KeListDictionary(jsonArray)
        If list.Count = 0 Then Return "[]"

        Try
            Dim keyExtractor = Function(x As Dictionary(Of String, Object)) As String
                                   If x.ContainsKey(sortKey) AndAlso
                                      x(sortKey) IsNot Nothing Then
                                       Return x(sortKey).ToString()
                                   End If
                                   Return ""
                               End Function

            Dim sorted As List(Of Dictionary(Of String, Object))

            If ascending Then
                sorted = list.OrderBy(keyExtractor).ToList()
            Else
                sorted = list.OrderByDescending(keyExtractor).ToList()
            End If

            Return KeJson(sorted)
        Catch
            Return jsonArray
        End Try
    End Function

End Class