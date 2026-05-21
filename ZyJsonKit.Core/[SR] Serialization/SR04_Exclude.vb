Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports System.Reflection

''' <summary>
''' SR04_Exclude
''' Serialisasi dengan mengecualikan properti tertentu
''' [CR01] Exclude property dynamically
''' [CR03] Reflection-based filtering
''' [PF02] Generic-safe
''' </summary>
Public NotInheritable Class SR04_Exclude

    ''' <summary>
    ''' Serialisasi object tanpa properti tertentu (blacklist)
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="daftarPropertiDikecualikan">Nama properti yang dikecualikan</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJson(Of T)(
        obj As T,
        ParamArray daftarPropertiDikecualikan As String()) As String

        If obj Is Nothing Then Return "null"

        If daftarPropertiDikecualikan Is Nothing OrElse
           daftarPropertiDikecualikan.Length = 0 Then
            Return SR01_Serialize.ToJson(obj)
        End If

        Try
            Dim resolver = New ExcludePropertiesResolver(daftarPropertiDikecualikan)
            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver = resolver,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi object hanya dengan properti tertentu (whitelist)
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="daftarPropertiDiizinkan">Properti yang diizinkan</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonHanya(Of T)(
        obj As T,
        ParamArray daftarPropertiDiizinkan As String()) As String

        If obj Is Nothing Then Return "null"

        If daftarPropertiDiizinkan Is Nothing OrElse
           daftarPropertiDiizinkan.Length = 0 Then
            Return "{}"
        End If

        Try
            Dim resolver = New IncludeOnlyPropertiesResolver(daftarPropertiDiizinkan)
            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver = resolver,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "{}"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi object tanpa properti dengan tipe tertentu
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="tipeDikecualikan">Tipe properti yang dikecualikan</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonTanpaTipe(Of T)(
        obj As T,
        ParamArray tipeDikecualikan As Type()) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim resolver = New ExcludeTypeResolver(tipeDikecualikan)
            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver = resolver,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

#Region "Custom Resolvers"

    ''' <summary>
    ''' Resolver untuk exclude properti tertentu (blacklist)
    ''' </summary>
    Private Class ExcludePropertiesResolver
        Inherits DefaultContractResolver

        Private ReadOnly _propertiDikecualikan As HashSet(Of String)

        Public Sub New(daftarProperti As String())
            _propertiDikecualikan = New HashSet(Of String)(
                daftarProperti,
                StringComparer.OrdinalIgnoreCase)
        End Sub

        Protected Overrides Function CreateProperty(
            memberInfo As MemberInfo,
            memberSerialization As MemberSerialization) As JsonProperty

            Dim properti = MyBase.CreateProperty(memberInfo, memberSerialization)

            If _propertiDikecualikan.Contains(properti.PropertyName) Then
                properti.ShouldSerialize = Function(instance) False
            End If

            Return properti
        End Function

    End Class

    ''' <summary>
    ''' Resolver untuk include properti tertentu saja (whitelist)
    ''' </summary>
    Private Class IncludeOnlyPropertiesResolver
        Inherits DefaultContractResolver

        Private ReadOnly _propertiDiizinkan As HashSet(Of String)

        Public Sub New(daftarProperti As String())
            _propertiDiizinkan = New HashSet(Of String)(
                daftarProperti,
                StringComparer.OrdinalIgnoreCase)
        End Sub

        Protected Overrides Function CreateProperty(
            memberInfo As MemberInfo,
            memberSerialization As MemberSerialization) As JsonProperty

            Dim properti = MyBase.CreateProperty(memberInfo, memberSerialization)

            If Not _propertiDiizinkan.Contains(properti.PropertyName) Then
                properti.ShouldSerialize = Function(instance) False
            End If

            Return properti
        End Function

    End Class

    ''' <summary>
    ''' Resolver untuk exclude properti berdasarkan tipe
    ''' </summary>
    Private Class ExcludeTypeResolver
        Inherits DefaultContractResolver

        Private ReadOnly _tipeDikecualikan As HashSet(Of Type)

        Public Sub New(tipe As Type())
            _tipeDikecualikan = New HashSet(Of Type)(tipe)
        End Sub

        Protected Overrides Function CreateProperty(
            memberInfo As MemberInfo,
            memberSerialization As MemberSerialization) As JsonProperty

            Dim properti = MyBase.CreateProperty(memberInfo, memberSerialization)

            If _tipeDikecualikan.Contains(properti.PropertyType) Then
                properti.ShouldSerialize = Function(instance) False
            End If

            Return properti
        End Function

    End Class

#End Region

End Class