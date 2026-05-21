Imports Newtonsoft.Json.Linq

''' <summary>
''' JO01_GetValue(path)
''' Mengambil value dari JSON menggunakan path expression
''' Support dot notation dan bracket notation
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class JT01_GetValue
    ''' <summary>
    ''' Mendapatkan value dari JObject menggunakan path
    ''' </summary>
    ''' <param name="jObject">JObject sumber</param>
    ''' <param name="path">Path properti (contoh: "data.nama" atau "items[0].id")</param>
    ''' <returns>JToken value, Nothing jika tidak ditemukan</returns>
    Public Shared Function Ambil(jObject As JObject, path As String) As JToken
        ' Validasi input
        If jObject Is Nothing Then Return Nothing
        If String.IsNullOrWhiteSpace(path) Then Return Nothing

        Try
            Return jObject.SelectToken(path)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan value sebagai string
    ''' </summary>
    ''' <param name="jObject">JObject sumber</param>
    ''' <param name="path">Path properti</param>
    ''' <param name="nilaiDefault">Nilai default jika tidak ditemukan</param>
    ''' <returns>String value</returns>
    Public Shared Function AmbilString(jObject As JObject, path As String, Optional nilaiDefault As String = "") As String
        Dim token = Ambil(jObject, path)
        If token Is Nothing Then Return nilaiDefault

        Try
            Return token.ToString()
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan value sebagai Integer
    ''' </summary>
    ''' <param name="jObject">JObject sumber</param>
    ''' <param name="path">Path properti</param>
    ''' <param name="nilaiDefault">Nilai default</param>
    ''' <returns>Integer value</returns>
    Public Shared Function AmbilInteger(jObject As JObject, path As String, Optional nilaiDefault As Integer = 0) As Integer
        Dim token = Ambil(jObject, path)
        If token Is Nothing Then Return nilaiDefault

        Try
            Return token.Value(Of Integer)()
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan value sebagai Boolean
    ''' </summary>
    Public Shared Function AmbilBoolean(jObject As JObject, path As String, Optional nilaiDefault As Boolean = False) As Boolean
        Dim token = Ambil(jObject, path)
        If token Is Nothing Then Return nilaiDefault

        Try
            Return token.Value(Of Boolean)()
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan value sebagai DateTime
    ''' </summary>
    Public Shared Function AmbilTanggal(jObject As JObject, path As String, Optional nilaiDefault As DateTime = Nothing) As DateTime
        Dim token = Ambil(jObject, path)
        If token Is Nothing Then Return nilaiDefault

        Try
            Return token.Value(Of DateTime)()
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan value sebagai Double/Decimal
    ''' </summary>
    Public Shared Function AmbilDouble(jObject As JObject, path As String, Optional nilaiDefault As Double = 0.0) As Double
        Dim token = Ambil(jObject, path)
        If token Is Nothing Then Return nilaiDefault

        Try
            Return token.Value(Of Double)()
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan nested JObject
    ''' </summary>
    Public Shared Function AmbilObject(jObject As JObject, path As String) As JObject
        Dim token = Ambil(jObject, path)
        If token Is Nothing Then Return Nothing

        Try
            Return DirectCast(token, JObject)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan nested JArray
    ''' </summary>
    Public Shared Function AmbilArray(jObject As JObject, path As String) As JArray
        Dim token = Ambil(jObject, path)
        If token Is Nothing Then Return Nothing

        Try
            Return DirectCast(token, JArray)
        Catch
            Return Nothing
        End Try
    End Function
End Class