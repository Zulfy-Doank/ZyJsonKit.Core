Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports System.Reflection

''' <summary>
''' LA01_SerializeList
''' Serialisasi List(Of T) ke JSON array
''' [PF02] Generic-safe
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class LA01_SerializeList

    ''' <summary>
    ''' Serialisasi List(Of T) ke JSON array
    ''' </summary>
    ''' <typeparam name="T">Tipe item dalam list</typeparam>
    ''' <param name="list">List yang akan diserialisasi</param>
    ''' <param name="formatRapi">Gunakan formatting indent</param>
    ''' <returns>String JSON array, "[]" jika list kosong/null</returns>
    Public Shared Function ToJson(Of T)(
        list As List(Of T),
        Optional formatRapi As Boolean = True) As String

        If list Is Nothing OrElse list.Count = 0 Then Return "[]"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim pengaturan = New JsonSerializerSettings With {
                .Formatting = jsonFormatting,
                .NullValueHandling = NullValueHandling.Ignore,
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            Return JsonConvert.SerializeObject(list, pengaturan)
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi List ke JSON compact
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="list">List</param>
    ''' <returns>JSON array compact</returns>
    Public Shared Function ToJsonCompact(Of T)(list As List(Of T)) As String
        Return ToJson(list, False)
    End Function

    ''' <summary>
    ''' Serialisasi List dengan camelCase
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="list">List</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>JSON array camelCase</returns>
    Public Shared Function ToJsonCamelCase(Of T)(
        list As List(Of T),
        Optional formatRapi As Boolean = True) As String

        If list Is Nothing OrElse list.Count = 0 Then Return "[]"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver = New CamelCasePropertyNamesContractResolver(),
                .Formatting = jsonFormatting,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(list, pengaturan)
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi List dengan exclude properti tertentu
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="list">List</param>
    ''' <param name="daftarPropertiDikecualikan">Properti yang tidak disertakan</param>
    ''' <returns>JSON array</returns>
    Public Shared Function ToJsonTanpaProperti(Of T)(
        list As List(Of T),
        ParamArray daftarPropertiDikecualikan As String()) As String

        If list Is Nothing OrElse list.Count = 0 Then Return "[]"

        Try
            Dim resolver = New ExcludePropertiesResolver(daftarPropertiDikecualikan)
            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver = resolver,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(list, pengaturan)
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi List dengan error handling
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="list">List</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>JSON array atau Nothing jika error</returns>
    Public Shared Function ToJsonAman(Of T)(
        list As List(Of T),
        ByRef pesanError As String) As String

        pesanError = String.Empty

        If list Is Nothing Then
            pesanError = "List tidak boleh null"
            Return "[]"
        End If

        Try
            Return JsonConvert.SerializeObject(list, Formatting.Indented)
        Catch ex As Exception
            pesanError = $"Gagal serialisasi list: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi List ke file JSON
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="list">List</param>
    ''' <param name="pathFile">Path file output</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function SimpanKeFile(Of T)(
        list As List(Of T),
        pathFile As String,
        Optional formatRapi As Boolean = True) As Boolean

        Try
            Dim json = ToJson(list, formatRapi)
            ' ✅ Fix: F01_Save → FO01_Save
            Return FO01_Save.Simpan(pathFile, json)
        Catch
            Return False
        End Try
    End Function

#Region "Private Resolver"

    ''' <summary>
    ''' Resolver untuk exclude properti tertentu
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

#End Region

End Class