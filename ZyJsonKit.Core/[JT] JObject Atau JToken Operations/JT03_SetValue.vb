Imports Newtonsoft.Json.Linq

''' <summary>
''' JO03_SetValue(path)
''' Mengatur value pada JObject menggunakan path expression
''' Support nested path dan auto-create intermediate objects
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class JT03_SetValue
    ''' <summary>
    ''' Mengatur value JObject pada path tertentu
    ''' </summary>
    ''' <param name="jObject">JObject target</param>
    ''' <param name="path">Path properti (contoh: "data.nama" atau "items[0].id")</param>
    ''' <param name="value">Value yang akan diset (Nothing untuk null)</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function Atur(jObject As JObject, path As String, value As Object) As Boolean
        ' Validasi
        If jObject Is Nothing Then Return False
        If String.IsNullOrWhiteSpace(path) Then Return False

        Try
            Dim token = jObject.SelectToken(path)
            If token IsNot Nothing Then
                ' Path sudah ada, replace value
                token.Replace(If(value IsNot Nothing, JToken.FromObject(value), JValue.CreateNull()))
                Return True
            Else
                ' Path belum ada, buat baru
                Return BuatDanAtur(jObject, path, value)
            End If
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Mengatur value string
    ''' </summary>
    Public Shared Function AturString(jObject As JObject, path As String, value As String) As Boolean
        Return Atur(jObject, path, value)
    End Function

    ''' <summary>
    ''' Mengatur value integer
    ''' </summary>
    Public Shared Function AturInteger(jObject As JObject, path As String, value As Integer) As Boolean
        Return Atur(jObject, path, New JValue(value))
    End Function

    ''' <summary>
    ''' Mengatur value boolean
    ''' </summary>
    Public Shared Function AturBoolean(jObject As JObject, path As String, value As Boolean) As Boolean
        Return Atur(jObject, path, New JValue(value))
    End Function

    ''' <summary>
    ''' Mengatur value DateTime
    ''' </summary>
    Public Shared Function AturTanggal(jObject As JObject, path As String, value As DateTime) As Boolean
        Return Atur(jObject, path, New JValue(value))
    End Function

    ''' <summary>
    ''' Mengatur value JObject (nested object)
    ''' </summary>
    Public Shared Function AturObject(jObject As JObject, path As String, value As JObject) As Boolean
        Return Atur(jObject, path, value)
    End Function

    ''' <summary>
    ''' Mengatur value JArray
    ''' </summary>
    Public Shared Function AturArray(jObject As JObject, path As String, value As JArray) As Boolean
        Return Atur(jObject, path, value)
    End Function

    ''' <summary>
    ''' Menambah value ke JArray pada path tertentu
    ''' </summary>
    Public Shared Function TambahKeArray(jObject As JObject, path As String, value As Object) As Boolean
        If jObject Is Nothing Then Return False

        Try
            Dim token = jObject.SelectToken(path)
            If token Is Nothing Then
                ' Buat array baru jika belum ada
                Dim arr As New JArray From {
                    If(value IsNot Nothing, JToken.FromObject(value), JValue.CreateNull())
                }
                Return BuatDanAtur(jObject, path, arr)
            End If

            If token.Type = JTokenType.Array Then
                DirectCast(token, JArray).Add(If(value IsNot Nothing, JToken.FromObject(value), JValue.CreateNull()))
                Return True
            End If

            Return False
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menghapus value pada path tertentu
    ''' </summary>
    Public Shared Function Hapus(jObject As JObject, path As String) As Boolean
        If jObject Is Nothing OrElse String.IsNullOrWhiteSpace(path) Then Return False

        Try
            Dim token = jObject.SelectToken(path)
            If token Is Nothing Then Return False

            If token.Parent IsNot Nothing Then
                token.Parent.Remove()
                Return True
            End If

            Return False
        Catch
            Return False
        End Try
    End Function

#Region "Private Helper"

    ''' <summary>
    ''' Membuat intermediate path dan set value
    ''' </summary>
    Private Shared Function BuatDanAtur(jObject As JObject, path As String, value As Object) As Boolean
        Try
            Dim parts = path.Split("."c)
            Dim current As JToken = jObject

            For i As Integer = 0 To parts.Length - 1
                Dim part = parts(i)
                Dim isLast = (i = parts.Length - 1)

                ' Handle array index: "items[0]"
                Dim arrayIndex As Integer = -1
                Dim propName = part

                If part.Contains("[") AndAlso part.Contains("]") Then
                    Dim startBracket = part.IndexOf("["c)
                    Dim endBracket = part.IndexOf("]"c)
                    propName = part.Substring(0, startBracket)
                    Dim indexStr = part.Substring(startBracket + 1, endBracket - startBracket - 1)
                    Integer.TryParse(indexStr, arrayIndex)
                End If

                If TypeOf current Is JObject Then
                    Dim obj = DirectCast(current, JObject)
                    Dim prop = obj.Property(propName)

                    If prop Is Nothing Then
                        If isLast Then
                            ' Buat properti baru dengan value
                            obj.Add(propName, If(value IsNot Nothing, JToken.FromObject(value), JValue.CreateNull()))
                        Else
                            ' Buat intermediate object
                            Dim newObj As New JObject()
                            obj.Add(propName, newObj)
                            current = newObj
                        End If
                    Else
                        current = prop.Value
                    End If
                End If
            Next

            Return True
        Catch
            Return False
        End Try
    End Function

#End Region

End Class