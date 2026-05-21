Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization

''' <summary>
''' SR03_CustomResolver
''' Serialisasi dengan custom contract resolver
''' Support camelCase, snake_case, lowercase, dan custom resolver
''' [CR] Contract Resolver System
''' [AF05] Extensible - Mudah ditambah resolver baru
''' </summary>
Public NotInheritable Class SR03_CustomResolver

    ''' <summary>
    ''' Serialisasi dengan custom contract resolver
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="resolver">Custom contract resolver</param>
    ''' <param name="formatRapi">Format rapi atau compact</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJson(Of T)(
        obj As T,
        resolver As IContractResolver,
        Optional formatRapi As Boolean = True) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver = resolver,
                .Formatting = jsonFormatting,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan CamelCase resolver
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>JSON camelCase</returns>
    Public Shared Function ToJsonCamelCase(Of T)(
        obj As T,
        Optional formatRapi As Boolean = True) As String

        Dim resolver = New CamelCasePropertyNamesContractResolver()
        Return ToJson(obj, resolver, formatRapi)
    End Function

    ''' <summary>
    ''' Serialisasi dengan SnakeCase resolver
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>JSON snake_case</returns>
    Public Shared Function ToJsonSnakeCase(Of T)(
        obj As T,
        Optional formatRapi As Boolean = True) As String

        Dim resolver = New DefaultContractResolver With {
            .NamingStrategy = New SnakeCaseNamingStrategy()
        }
        Return ToJson(obj, resolver, formatRapi)
    End Function

    ''' <summary>
    ''' Serialisasi dengan Default resolver (nama properti asli)
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>JSON default</returns>
    Public Shared Function ToJsonDefault(Of T)(
        obj As T,
        Optional formatRapi As Boolean = True) As String

        Dim resolver = New DefaultContractResolver()
        Return ToJson(obj, resolver, formatRapi)
    End Function

    ''' <summary>
    ''' Serialisasi dengan LowerCase resolver
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>JSON lowercase</returns>
    Public Shared Function ToJsonLowerCase(Of T)(
        obj As T,
        Optional formatRapi As Boolean = True) As String

        Dim resolver = New LowerCaseContractResolver()
        Return ToJson(obj, resolver, formatRapi)
    End Function

    ''' <summary>
    ''' Custom LowerCase Contract Resolver
    ''' Konversi semua nama properti ke lowercase
    ''' </summary>
    Private Class LowerCaseContractResolver
        Inherits DefaultContractResolver

        Protected Overrides Function ResolvePropertyName(
            propertyName As String) As String

            Return propertyName.ToLowerInvariant()
        End Function

    End Class

End Class