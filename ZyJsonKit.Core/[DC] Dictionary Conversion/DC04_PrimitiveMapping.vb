Imports Newtonsoft.Json.Linq

''' <summary>
''' DC04_PrimitiveMapping
''' Mapping dan konversi tipe primitif dari JSON value
''' Menangani konversi otomatis antar tipe dasar
''' [PF02] Generic-safe
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DC04_PrimitiveMapping

    ''' <summary>
    ''' Konversi JToken ke tipe primitif yang sesuai secara otomatis
    ''' </summary>
    ''' <param name="token">JToken</param>
    ''' <returns>Object primitif (String, Integer, Long, Double, Boolean, DateTime, Nothing)</returns>
    Public Shared Function KePrimitif(token As JToken) As Object
        If token Is Nothing Then Return Nothing

        Try
            Select Case token.Type
                Case JTokenType.Integer
                    Dim value = token.Value(Of Long)()
                    If value >= Integer.MinValue AndAlso value <= Integer.MaxValue Then
                        Return CInt(value)
                    End If
                    Return value

                Case JTokenType.Float
                    Return token.Value(Of Double)()

                Case JTokenType.Boolean
                    Return token.Value(Of Boolean)()

                Case JTokenType.Date
                    Return token.Value(Of DateTime)()

                Case JTokenType.String
                    Return token.Value(Of String)()

                Case JTokenType.Guid
                    Return token.Value(Of Guid)()

                Case JTokenType.TimeSpan
                    Return token.Value(Of TimeSpan)()

                Case JTokenType.Uri
                    Return token.Value(Of Uri)()

                Case JTokenType.Bytes
                    Return token.Value(Of Byte())()

                Case JTokenType.Null, JTokenType.None
                    Return Nothing

                Case Else
                    Return token.ToString()
            End Select
        Catch
            Return token.ToString()
        End Try
    End Function

    ''' <summary>
    ''' Konversi JToken ke tipe spesifik dengan safe casting
    ''' </summary>
    ''' <typeparam name="T">Tipe target</typeparam>
    ''' <param name="token">JToken</param>
    ''' <param name="nilaiDefault">Nilai default jika gagal</param>
    ''' <returns>Value dengan tipe T</returns>
    Public Shared Function KonversiKe(Of T)(
        token As JToken,
        Optional nilaiDefault As T = Nothing) As T

        If token Is Nothing Then Return nilaiDefault

        Try
            Dim primitif As Object = KePrimitif(token)
            If primitif Is Nothing Then Return nilaiDefault

            If TypeOf primitif Is T Then
                Return DirectCast(primitif, T)
            End If

            Dim targetType As Type = GetType(T)
            Return DirectCast(Convert.ChangeType(primitif, targetType), T)
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan tipe .NET dari JTokenType
    ''' </summary>
    ''' <param name="tokenType">JTokenType</param>
    ''' <returns>Type .NET yang sesuai</returns>
    Public Shared Function KeTipeDotNet(tokenType As JTokenType) As Type
        Select Case tokenType
            Case JTokenType.Integer : Return GetType(Long)
            Case JTokenType.Float : Return GetType(Double)
            Case JTokenType.String : Return GetType(String)
            Case JTokenType.Boolean : Return GetType(Boolean)
            Case JTokenType.Date : Return GetType(DateTime)
            Case JTokenType.Guid : Return GetType(Guid)
            Case JTokenType.TimeSpan : Return GetType(TimeSpan)
            Case JTokenType.Uri : Return GetType(Uri)
            Case JTokenType.Bytes : Return GetType(Byte())
            Case JTokenType.Null,
                 JTokenType.None : Return GetType(Object)
            Case Else : Return GetType(String)
        End Select
    End Function

    ''' <summary>
    ''' Konversi value Object ke JToken dengan tipe yang sesuai
    ''' </summary>
    ''' <param name="value">Object value</param>
    ''' <returns>JToken dengan tipe yang sesuai</returns>
    Public Shared Function KeJToken(value As Object) As JToken
        If value Is Nothing Then Return JValue.CreateNull()

        Try
            If TypeOf value Is String Then
                Return New JValue(DirectCast(value, String))
            ElseIf TypeOf value Is Integer Then
                Return New JValue(DirectCast(value, Integer))
            ElseIf TypeOf value Is Long Then
                Return New JValue(DirectCast(value, Long))
            ElseIf TypeOf value Is Double Then
                Return New JValue(DirectCast(value, Double))
            ElseIf TypeOf value Is Decimal Then
                Return New JValue(DirectCast(value, Decimal))
            ElseIf TypeOf value Is Boolean Then
                Return New JValue(DirectCast(value, Boolean))
            ElseIf TypeOf value Is DateTime Then
                Return New JValue(DirectCast(value, DateTime))
            ElseIf TypeOf value Is Guid Then
                Return New JValue(DirectCast(value, Guid))
            ElseIf TypeOf value Is TimeSpan Then
                Return New JValue(DirectCast(value, TimeSpan))
            ElseIf TypeOf value Is Byte() Then
                Return New JValue(DirectCast(value, Byte()))
            Else
                Return JToken.FromObject(value)
            End If
        Catch
            Return New JValue(value.ToString())
        End Try
    End Function

    ''' <summary>
    ''' Konversi string JSON ke tipe primitif
    ''' </summary>
    ''' <typeparam name="T">Tipe target</typeparam>
    ''' <param name="jsonValue">String JSON value</param>
    ''' <param name="nilaiDefault">Nilai default</param>
    ''' <returns>Value dengan tipe T</returns>
    Public Shared Function DariJsonValue(Of T)(
        jsonValue As String,
        Optional nilaiDefault As T = Nothing) As T

        If String.IsNullOrWhiteSpace(jsonValue) Then Return nilaiDefault

        Try
            Dim token = JToken.Parse(jsonValue)
            Return KonversiKe(Of T)(token, nilaiDefault)
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendeteksi JTokenType dari Object .NET
    ''' </summary>
    ''' <param name="value">Object value</param>
    ''' <returns>JTokenType yang sesuai</returns>
    Public Shared Function DeteksiJTokenType(value As Object) As JTokenType
        If value Is Nothing Then Return JTokenType.Null

        If TypeOf value Is String Then
            Return JTokenType.String
        ElseIf TypeOf value Is Integer OrElse
               TypeOf value Is Long OrElse
               TypeOf value Is Short OrElse
               TypeOf value Is Byte Then
            Return JTokenType.Integer
        ElseIf TypeOf value Is Double OrElse
               TypeOf value Is Single OrElse
               TypeOf value Is Decimal Then
            Return JTokenType.Float
        ElseIf TypeOf value Is Boolean Then
            Return JTokenType.Boolean
        ElseIf TypeOf value Is DateTime Then
            Return JTokenType.Date
        ElseIf TypeOf value Is Guid Then
            Return JTokenType.Guid
        ElseIf TypeOf value Is TimeSpan Then
            Return JTokenType.TimeSpan
        ElseIf TypeOf value Is Uri Then
            Return JTokenType.Uri
        ElseIf TypeOf value Is Byte() Then
            Return JTokenType.Bytes
        ElseIf TypeOf value Is JObject Then
            Return JTokenType.Object
        ElseIf TypeOf value Is JArray Then
            Return JTokenType.Array
        Else
            Return JTokenType.Object
        End If
    End Function

    ''' <summary>
    ''' Tabel mapping tipe primitif untuk referensi
    ''' </summary>
    ''' <returns>Dictionary JTokenType ke nama tipe .NET</returns>
    Public Shared Function TabelMapping() As Dictionary(Of JTokenType, String)
        Return New Dictionary(Of JTokenType, String) From {
            {JTokenType.Integer, "Long/Integer"},
            {JTokenType.Float, "Double"},
            {JTokenType.String, "String"},
            {JTokenType.Boolean, "Boolean"},
            {JTokenType.Date, "DateTime"},
            {JTokenType.Guid, "Guid"},
            {JTokenType.TimeSpan, "TimeSpan"},
            {JTokenType.Uri, "Uri"},
            {JTokenType.Bytes, "Byte()"},
            {JTokenType.Null, "Nothing"},
            {JTokenType.Object, "JObject"},
            {JTokenType.Array, "JArray"}
        }
    End Function

End Class