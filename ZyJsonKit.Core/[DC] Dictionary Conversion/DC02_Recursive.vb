Imports Newtonsoft.Json.Linq

''' <summary>
''' DC02_Recursive
''' Konversi rekursif nested JSON ke nested Dictionary/List
''' Menangani struktur bertingkat (object dalam object, array dalam object, dll)
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DC02_Recursive

    ''' <summary>
    ''' Konversi rekursif JToken ke Object (Dictionary/List/value primitif)
    ''' </summary>
    ''' <param name="token">JToken (JObject, JArray, atau JValue)</param>
    ''' <returns>Object hasil konversi rekursif</returns>
    Public Shared Function Konversi(token As JToken) As Object
        If token Is Nothing Then Return Nothing

        Try
            Select Case token.Type
                Case JTokenType.Object
                    Return KonversiObject(DirectCast(token, JObject))
                Case JTokenType.Array
                    Return KonversiArray(DirectCast(token, JArray))
                Case JTokenType.Null
                    Return Nothing
                Case Else
                    Return DirectCast(token, JValue).Value
            End Select
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Konversi rekursif JObject ke Dictionary(Of String, Object)
    ''' </summary>
    ''' <param name="jObject">JObject</param>
    ''' <returns>Dictionary dengan value rekursif</returns>
    Public Shared Function KonversiObject(
        jObject As JObject) As Dictionary(Of String, Object)

        Dim hasil As New Dictionary(Of String, Object)()

        If jObject Is Nothing Then Return hasil

        Try
            For Each prop As JProperty In jObject.Properties()
                hasil(prop.Name) = Konversi(prop.Value)
            Next
        Catch
        End Try

        Return hasil
    End Function

    ''' <summary>
    ''' Konversi rekursif JArray ke List(Of Object)
    ''' </summary>
    ''' <param name="jArray">JArray</param>
    ''' <returns>List dengan item rekursif</returns>
    Public Shared Function KonversiArray(jArray As JArray) As List(Of Object)
        Dim hasil As New List(Of Object)()

        If jArray Is Nothing Then Return hasil

        Try
            For Each item As JToken In jArray
                hasil.Add(Konversi(item))
            Next
        Catch
        End Try

        Return hasil
    End Function

    ''' <summary>
    ''' Konversi JSON string ke struktur rekursif
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Object (Dictionary/List/value)</returns>
    Public Shared Function DariJson(jsonString As String) As Object
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Dim token = JToken.Parse(jsonString)
            Return Konversi(token)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan value dari struktur rekursif dengan path
    ''' </summary>
    ''' <param name="root">Object hasil konversi rekursif</param>
    ''' <param name="path">Path dipisah titik (contoh: "data.items.0.nama")</param>
    ''' <returns>Value pada path, Nothing jika tidak ditemukan</returns>
    Public Shared Function AmbilDariPath(root As Object, path As String) As Object
        If root Is Nothing OrElse String.IsNullOrWhiteSpace(path) Then Return Nothing

        Try
            Dim parts = path.Split("."c)
            Dim current = root

            For Each part In parts
                If current Is Nothing Then Return Nothing

                Dim index As Integer = -1
                If Integer.TryParse(part, index) Then
                    ' Akses sebagai List
                    Dim list = TryCast(current, List(Of Object))
                    If list IsNot Nothing AndAlso
                       index >= 0 AndAlso
                       index < list.Count Then
                        current = list(index)
                    Else
                        Return Nothing
                    End If
                Else
                    ' Akses sebagai Dictionary
                    Dim dict = TryCast(current, Dictionary(Of String, Object))
                    If dict IsNot Nothing AndAlso dict.ContainsKey(part) Then
                        current = dict(part)
                    Else
                        Return Nothing
                    End If
                End If
            Next

            Return current
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Flatten struktur rekursif menjadi Dictionary flat
    ''' </summary>
    ''' <param name="root">Object rekursif</param>
    ''' <param name="prefix">Prefix path (untuk recursive call)</param>
    ''' <returns>Dictionary flat</returns>
    Public Shared Function Flatten(
        root As Object,
        Optional prefix As String = "") As Dictionary(Of String, Object)

        Dim hasil As New Dictionary(Of String, Object)()

        If root Is Nothing Then Return hasil

        Try
            Dim dict = TryCast(root, Dictionary(Of String, Object))
            If dict IsNot Nothing Then
                For Each kvp In dict
                    Dim newPrefix = If(
                        String.IsNullOrEmpty(prefix),
                        kvp.Key,
                        $"{prefix}.{kvp.Key}")

                    Dim childFlat = Flatten(kvp.Value, newPrefix)

                    If childFlat.Count = 0 Then
                        hasil(newPrefix) = kvp.Value
                    Else
                        For Each childKvp In childFlat
                            hasil(childKvp.Key) = childKvp.Value
                        Next
                    End If
                Next
                Return hasil
            End If

            Dim list = TryCast(root, List(Of Object))
            If list IsNot Nothing Then
                For i As Integer = 0 To list.Count - 1
                    Dim newPrefix = $"{prefix}[{i}]"
                    Dim childFlat = Flatten(list(i), newPrefix)

                    If childFlat.Count = 0 Then
                        hasil(newPrefix) = list(i)
                    Else
                        For Each childKvp In childFlat
                            hasil(childKvp.Key) = childKvp.Value
                        Next
                    End If
                Next
                Return hasil
            End If

            ' Primitive value
            If Not String.IsNullOrEmpty(prefix) Then
                hasil(prefix) = root
            End If
        Catch
        End Try

        Return hasil
    End Function

End Class