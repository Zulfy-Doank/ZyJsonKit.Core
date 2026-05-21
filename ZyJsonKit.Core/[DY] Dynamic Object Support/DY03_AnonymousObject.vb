Imports Newtonsoft.Json
Imports System.Reflection

''' <summary>
''' DY03_Anonymous object handling
''' Handle anonymous object: serialisasi, deserialisasi, property extraction
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DY03_AnonymousObject
    ''' <summary>
    ''' Serialisasi anonymous object ke JSON
    ''' </summary>
    ''' <param name="obj">Anonymous object</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJson(obj As Object, Optional formatRapi As Boolean = True) As String
        If obj Is Nothing Then Return "null"

        Try
            Return JsonConvert.SerializeObject(obj, If(formatRapi, Formatting.Indented, Formatting.None))
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Ekstrak properti dari anonymous object ke Dictionary
    ''' </summary>
    ''' <param name="obj">Anonymous object</param>
    ''' <returns>Dictionary properti dan value</returns>
    Public Shared Function KeDictionary(obj As Object) As Dictionary(Of String, Object)
        If obj Is Nothing Then Return New Dictionary(Of String, Object)()

        Try
            ' Gunakan JSON sebagai perantara
            Dim json = JsonConvert.SerializeObject(obj)
            Return JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(json)
        Catch
            Return New Dictionary(Of String, Object)()
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan value properti dari anonymous object via reflection
    ''' </summary>
    ''' <param name="obj">Anonymous object</param>
    ''' <param name="namaProperti">Nama properti</param>
    ''' <returns>Value properti, Nothing jika tidak ada</returns>
    Public Shared Function AmbilProperti(obj As Object, namaProperti As String) As Object
        If obj Is Nothing OrElse String.IsNullOrWhiteSpace(namaProperti) Then Return Nothing

        Try
            Dim tipe = obj.GetType()
            Dim prop = tipe.GetProperty(namaProperti)
            If prop IsNot Nothing Then
                Return prop.GetValue(obj, Nothing)
            End If
        Catch
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' Mendapatkan semua nama properti dari anonymous object
    ''' </summary>
    ''' <param name="obj">Anonymous object</param>
    ''' <returns>List nama properti</returns>
    Public Shared Function DaftarProperti(obj As Object) As List(Of String)
        If obj Is Nothing Then Return New List(Of String)()

        Try
            Return obj.GetType().GetProperties().Select(Function(p) p.Name).ToList()
        Catch
            Return New List(Of String)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi anonymous object ke tipe spesifik
    ''' </summary>
    ''' <typeparam name="T">Tipe target</typeparam>
    ''' <param name="obj">Anonymous object</param>
    ''' <returns>Object dengan tipe T</returns>
    Public Shared Function KonversiKe(Of T)(obj As Object) As T
        If obj Is Nothing Then Return Nothing

        Try
            Dim json = JsonConvert.SerializeObject(obj)
            Return JsonConvert.DeserializeObject(Of T)(json)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Merge dua anonymous object menjadi satu
    ''' </summary>
    ''' <param name="obj1">Object pertama</param>
    ''' <param name="obj2">Object kedua</param>
    ''' <returns>Dictionary hasil merge</returns>
    Public Shared Function Gabungkan(obj1 As Object, obj2 As Object) As Dictionary(Of String, Object)
        Dim hasil As New Dictionary(Of String, Object)()

        If obj1 IsNot Nothing Then
            Dim dict1 = KeDictionary(obj1)
            For Each kvp In dict1
                hasil(kvp.Key) = kvp.Value
            Next
        End If

        If obj2 IsNot Nothing Then
            Dim dict2 = KeDictionary(obj2)
            For Each kvp In dict2
                hasil(kvp.Key) = kvp.Value
            Next
        End If

        Return hasil
    End Function
End Class