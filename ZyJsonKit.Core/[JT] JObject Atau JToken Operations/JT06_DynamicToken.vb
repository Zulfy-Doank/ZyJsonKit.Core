Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' JO06_Dynamic token handling
''' Handle token JSON secara dinamis
''' Membuat, memodifikasi, dan mengkonversi JToken dengan fleksibel
''' [DY] Dynamic Object Support
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class JT06_DynamicToken
    ''' <summary>
    ''' Membuat JObject dari anonymous object
    ''' </summary>
    ''' <param name="obj">Anonymous object atau Dictionary</param>
    ''' <returns>JObject</returns>
    Public Shared Function BuatDariObject(obj As Object) As JObject
        If obj Is Nothing Then Return New JObject()

        Try
            Return JObject.FromObject(obj)
        Catch
            Return New JObject()
        End Try
    End Function

    ''' <summary>
    ''' Membuat JObject dari Dictionary
    ''' </summary>
    Public Shared Function BuatDariDictionary(dictionary As Dictionary(Of String, Object)) As JObject
        If dictionary Is Nothing Then Return New JObject()

        Try
            Return JObject.FromObject(dictionary)
        Catch
            Return New JObject()
        End Try
    End Function

    ''' <summary>
    ''' Membuat JArray dari List
    ''' </summary>
    Public Shared Function BuatDariList(list As System.Collections.IList) As JArray
        If list Is Nothing Then Return New JArray()

        Try
            Return JArray.FromObject(list)
        Catch
            Return New JArray()
        End Try
    End Function

    ''' <summary>
    ''' Membuat JObject kosong
    ''' </summary>
    Public Shared Function BuatObjectKosong() As JObject
        Return New JObject()
    End Function

    ''' <summary>
    ''' Membuat JArray kosong
    ''' </summary>
    Public Shared Function BuatArrayKosong() As JArray
        Return New JArray()
    End Function

    ''' <summary>
    ''' Menambah properti ke JObject secara dinamis
    ''' </summary>
    ''' <param name="jObject">JObject target</param>
    ''' <param name="nama">Nama properti</param>
    ''' <param name="value">Value (Object, String, Integer, dll)</param>
    Public Shared Sub TambahProperti(jObject As JObject, nama As String, value As Object)
        If jObject Is Nothing Then Return
        If String.IsNullOrWhiteSpace(nama) Then Return

        Try
            ' Cek apakah properti sudah ada
            Dim existing = jObject.Property(nama)
            If existing IsNot Nothing Then
                ' Update value jika sudah ada
                existing.Value.Replace(If(value IsNot Nothing, JToken.FromObject(value), JValue.CreateNull()))
            Else
                ' Tambah properti baru
                jObject.Add(nama, If(value IsNot Nothing, JToken.FromObject(value), JValue.CreateNull()))
            End If
        Catch
            ' Properti mungkin sudah ada dengan nama berbeda case
        End Try
    End Sub

    ''' <summary>
    ''' Menambah item ke JArray
    ''' </summary>
    Public Shared Sub TambahItem(jArray As JArray, value As Object)
        If jArray Is Nothing Then Return

        Try
            jArray.Add(If(value IsNot Nothing, JToken.FromObject(value), JValue.CreateNull()))
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Menghapus properti dari JObject
    ''' </summary>
    Public Shared Function HapusProperti(jObject As JObject, nama As String) As Boolean
        If jObject Is Nothing OrElse String.IsNullOrWhiteSpace(nama) Then Return False

        Try
            Return jObject.Remove(nama)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Mengecek apakah properti exists
    ''' </summary>
    Public Shared Function PropertiAda(jObject As JObject, nama As String) As Boolean
        If jObject Is Nothing Then Return False

        Try
            Return jObject.Property(nama) IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Konversi JToken ke Dictionary
    ''' </summary>
    Public Shared Function KeDictionary(jToken As JToken) As Dictionary(Of String, Object)
        If jToken Is Nothing Then Return New Dictionary(Of String, Object)()

        Try
            Return jToken.ToObject(Of Dictionary(Of String, Object))()
        Catch
            Return New Dictionary(Of String, Object)()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JToken ke ExpandoObject (dynamic)
    ''' </summary>
    Public Shared Function KeDynamic(jToken As JToken) As Object
        If jToken Is Nothing Then Return Nothing

        Try
            Return jToken.ToObject(Of System.Dynamic.ExpandoObject)()
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Konversi JToken ke JSON string
    ''' </summary>
    Public Shared Function KeJsonString(jToken As JToken, Optional formatRapi As Boolean = True) As String
        If jToken Is Nothing Then Return "null"

        Try
            Return jToken.ToString(If(formatRapi, Formatting.Indented, Formatting.None))
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Clone JToken (deep copy)
    ''' </summary>
    Public Shared Function Clone(jToken As JToken) As JToken
        If jToken Is Nothing Then Return Nothing

        Try
            Return jToken.DeepClone()
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membandingkan dua JToken
    ''' </summary>
    Public Shared Function ApakahSama(token1 As JToken, token2 As JToken) As Boolean
        If token1 Is Nothing AndAlso token2 Is Nothing Then Return True
        If token1 Is Nothing OrElse token2 Is Nothing Then Return False

        Try
            Return JToken.DeepEquals(token1, token2)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Replace value pada JToken
    ''' </summary>
    Public Shared Function GantiValue(token As JToken, valueBaru As Object) As Boolean
        If token Is Nothing Then Return False

        Try
            token.Replace(If(valueBaru IsNot Nothing, JToken.FromObject(valueBaru), JValue.CreateNull()))
            Return True
        Catch
            Return False
        End Try
    End Function
End Class