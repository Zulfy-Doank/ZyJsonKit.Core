Imports Newtonsoft.Json.Linq

''' <summary>
''' JO05_JSON Path support
''' Support untuk JSONPath expression (SelectToken/SelectTokens)
''' Mendukung filter, wildcard, dan recursive descent
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class JT05_PathSupport
    ''' <summary>
    ''' Mencari satu token dengan JSONPath expression
    ''' </summary>
    ''' <param name="root">Root JToken</param>
    ''' <param name="jsonPath">JSONPath expression</param>
    ''' <returns>JToken pertama yang cocok, Nothing jika tidak ada</returns>
    Public Shared Function CariSatu(root As JToken, jsonPath As String) As JToken
        If root Is Nothing OrElse String.IsNullOrWhiteSpace(jsonPath) Then
            Return Nothing
        End If

        Try
            Return root.SelectToken(jsonPath)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Mencari semua token yang cocok dengan JSONPath
    ''' </summary>
    ''' <param name="root">Root JToken</param>
    ''' <param name="jsonPath">JSONPath expression</param>
    ''' <returns>IEnumerable of JToken</returns>
    Public Shared Function CariSemua(root As JToken, jsonPath As String) As IEnumerable(Of JToken)
        If root Is Nothing OrElse String.IsNullOrWhiteSpace(jsonPath) Then
            Return Enumerable.Empty(Of JToken)()
        End If

        Try
            Return root.SelectTokens(jsonPath)
        Catch
            Return Enumerable.Empty(Of JToken)()
        End Try
    End Function

    ''' <summary>
    ''' Mengecek apakah path exists
    ''' </summary>
    Public Shared Function PathAda(root As JToken, jsonPath As String) As Boolean
        Dim token = CariSatu(root, jsonPath)
        Return token IsNot Nothing
    End Function

    ''' <summary>
    ''' Menghitung jumlah token yang cocok
    ''' </summary>
    Public Shared Function HitungToken(root As JToken, jsonPath As String) As Integer
        Dim tokens = CariSemua(root, jsonPath)
        Return tokens.Count()
    End Function

    ''' <summary>
    ''' Mendapatkan semua property names dari JObject
    ''' </summary>
    ''' <param name="jObject">JObject</param>
    ''' <returns>List nama properti</returns>
    Public Shared Function DaftarProperti(jObject As JObject) As List(Of String)
        If jObject Is Nothing Then Return New List(Of String)()

        Try
            Return jObject.Properties().Select(Function(p) p.Name).ToList()
        Catch
            Return New List(Of String)()
        End Try
    End Function

    ''' <summary>
    ''' Mengecek tipe token pada path
    ''' </summary>
    Public Shared Function TipeToken(root As JToken, jsonPath As String) As JTokenType?
        Dim token = CariSatu(root, jsonPath)
        If token Is Nothing Then Return Nothing
        Return token.Type
    End Function

    ''' <summary>
    ''' Mendapatkan parent dari token pada path
    ''' </summary>
    Public Shared Function ParentToken(root As JToken, jsonPath As String) As JToken
        Dim token = CariSatu(root, jsonPath)
        If token Is Nothing Then Return Nothing
        Return token.Parent
    End Function

    ''' <summary>
    ''' Mendapatkan root ancestor
    ''' </summary>
    Public Shared Function RootToken(token As JToken) As JToken
        If token Is Nothing Then Return Nothing

        Try
            Return token.Root
        Catch
            Return Nothing
        End Try
    End Function

#Region "JSONPath Examples Helper"

    ''' <summary>
    ''' Mendapatkan contoh JSONPath expression
    ''' </summary>
    Public Shared Function ContohPath() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"$.store.book[0].title", "Buku pertama di store"},
            {"$.store.book[*].author", "Semua author buku"},
            {"$..author", "Semua author di mana pun"},
            {"$.store.book[?(@.price < 10)]", "Buku dengan harga < 10"},
            {"$.store.book[?(@.category == 'fiction')]", "Buku kategori fiction"},
            {"$.store.book[-1:]", "Buku terakhir"},
            {"$.store.book[:2]", "Dua buku pertama"},
            {"$..*", "Semua properti dan value"},
            {"$.store.book[0:2].title", "Title buku index 0 sampai 2"},
            {"$.store.book[?(@.isbn)]", "Buku yang memiliki ISBN"}
        }
    End Function

#End Region

End Class