Imports Newtonsoft.Json.Linq

''' <summary>
''' JO02_GetValue(Of T)
''' Mengambil typed value dari JSON dengan generic method
''' Support konversi otomatis ke tipe yang diinginkan
''' [PF02] Generic-safe
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class JT02_GetValueTyped
    ''' <summary>
    ''' Mendapatkan typed value dari JObject
    ''' </summary>
    ''' <typeparam name="T">Tipe value yang diinginkan</typeparam>
    ''' <param name="jObject">JObject sumber</param>
    ''' <param name="path">Path properti</param>
    ''' <param name="nilaiDefault">Nilai default jika tidak ditemukan</param>
    ''' <returns>Value dengan tipe T</returns>
    Public Shared Function Ambil(Of T)(jObject As JObject, path As String, Optional nilaiDefault As T = Nothing) As T
        ' Validasi
        If jObject Is Nothing Then Return nilaiDefault
        If String.IsNullOrWhiteSpace(path) Then Return nilaiDefault

        Try
            Dim token = jObject.SelectToken(path)
            If token Is Nothing Then Return nilaiDefault

            ' Konversi ke tipe yang diminta
            Return token.Value(Of T)()
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan typed value dengan konversi kustom
    ''' </summary>
    ''' <typeparam name="T">Tipe target</typeparam>
    ''' <param name="jObject">JObject</param>
    ''' <param name="path">Path</param>
    ''' <param name="konverter">Function konversi kustom</param>
    ''' <param name="nilaiDefault">Nilai default</param>
    ''' <returns>Value hasil konversi</returns>
    Public Shared Function AmbilDenganKonversi(Of T)(
        jObject As JObject,
        path As String,
        konverter As Func(Of JToken, T),
        Optional nilaiDefault As T = Nothing) As T

        If jObject Is Nothing Then Return nilaiDefault
        If konverter Is Nothing Then Return nilaiDefault

        Try
            Dim token = jObject.SelectToken(path)
            If token Is Nothing Then Return nilaiDefault

            Return konverter(token)
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan List(Of T) dari JArray
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jObject">JObject</param>
    ''' <param name="path">Path ke JArray</param>
    ''' <returns>List(Of T), list kosong jika gagal</returns>
    Public Shared Function AmbilList(Of T)(jObject As JObject, path As String) As List(Of T)
        If jObject Is Nothing Then Return New List(Of T)()

        Try
            Dim token = jObject.SelectToken(path)
            If token Is Nothing OrElse token.Type <> JTokenType.Array Then
                Return New List(Of T)()
            End If

            Return token.ToObject(Of List(Of T))()
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan Dictionary dari JObject
    ''' </summary>
    ''' <typeparam name="TKey">Tipe key</typeparam>
    ''' <typeparam name="TValue">Tipe value</typeparam>
    ''' <param name="jObject">JObject</param>
    ''' <param name="path">Path ke nested object</param>
    ''' <returns>Dictionary</returns>
    Public Shared Function AmbilDictionary(Of TKey, TValue)(jObject As JObject, path As String) As Dictionary(Of TKey, TValue)
        If jObject Is Nothing Then Return New Dictionary(Of TKey, TValue)()

        Try
            Dim token = jObject.SelectToken(path)
            If token Is Nothing OrElse token.Type <> JTokenType.Object Then
                Return New Dictionary(Of TKey, TValue)()
            End If

            Return token.ToObject(Of Dictionary(Of TKey, TValue))()
        Catch
            Return New Dictionary(Of TKey, TValue)()
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan Enum value
    ''' </summary>
    ''' <typeparam name="T">Tipe Enum</typeparam>
    ''' <param name="jObject">JObject</param>
    ''' <param name="path">Path</param>
    ''' <param name="nilaiDefault">Default enum value</param>
    ''' <returns>Enum value</returns>
    Public Shared Function AmbilEnum(Of T As Structure)(jObject As JObject, path As String, Optional nilaiDefault As T = Nothing) As T
        If jObject Is Nothing Then Return nilaiDefault

        Try
            Dim token = jObject.SelectToken(path)
            If token Is Nothing Then Return nilaiDefault

            Dim strValue = token.ToString()
            Return DirectCast([Enum].Parse(GetType(T), strValue, True), T)
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mengecek apakah path memiliki value (tidak null)
    ''' </summary>
    Public Shared Function PathAda(jObject As JObject, path As String) As Boolean
        If jObject Is Nothing Then Return False

        Try
            Dim token = jObject.SelectToken(path)
            Return token IsNot Nothing AndAlso token.Type <> JTokenType.Null
        Catch
            Return False
        End Try
    End Function
End Class