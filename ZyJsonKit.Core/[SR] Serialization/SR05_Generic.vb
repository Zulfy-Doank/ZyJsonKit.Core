Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports System.Text

''' <summary>
''' SR05_Generic
''' Serialisasi generic object dengan berbagai opsi lengkap
''' [PF02] Generic-safe - Aman untuk generic typing
''' [AF01] Generic methods - Method generic reusable
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class SR05_Generic

    ''' <summary>
    ''' Serialisasi generic dengan opsi lengkap
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object yang akan diserialisasi</param>
    ''' <param name="formatRapi">Gunakan formatting rapi</param>
    ''' <param name="abaikanNull">Abaikan properti null</param>
    ''' <param name="gunakanCamelCase">Gunakan camelCase</param>
    ''' <param name="dateFormat">Format tanggal</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJson(Of T)(
        obj As T,
        Optional formatRapi As Boolean = True,
        Optional abaikanNull As Boolean = True,
        Optional gunakanCamelCase As Boolean = False,
        Optional dateFormat As String = "yyyy-MM-dd HH:mm:ss") As String

        If obj Is Nothing Then Return "null"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim nullHandling As NullValueHandling = If(
                abaikanNull,
                NullValueHandling.Ignore,
                NullValueHandling.Include)

            Dim pengaturan = New JsonSerializerSettings With {
                .Formatting = jsonFormatting,
                .NullValueHandling = nullHandling,
                .MissingMemberHandling = MissingMemberHandling.Ignore,
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                .DateFormatString = dateFormat
            }

            If gunakanCamelCase Then
                pengaturan.ContractResolver =
                    New CamelCasePropertyNamesContractResolver()
            End If

            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan pengaturan kustom via Action
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="konfigurasi">Action untuk konfigurasi settings</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonCustom(Of T)(
        obj As T,
        konfigurasi As Action(Of JsonSerializerSettings)) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim settings = New JsonSerializerSettings()

            If konfigurasi IsNot Nothing Then
                konfigurasi.Invoke(settings)
            End If

            Return JsonConvert.SerializeObject(obj, settings)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan TypeNameHandling untuk polymorphic
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="sertakanTipeInfo">Sertakan $type info</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonDenganTipe(Of T)(
        obj As T,
        Optional sertakanTipeInfo As Boolean = False) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim typeHandling As TypeNameHandling = If(
                sertakanTipeInfo,
                TypeNameHandling.All,
                TypeNameHandling.None)

            Dim pengaturan = New JsonSerializerSettings With {
                .TypeNameHandling = typeHandling,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan MaxDepth untuk batasi kedalaman
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="maxDepth">Kedalaman maksimum</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonDenganDepth(Of T)(
        obj As T,
        Optional maxDepth As Integer = 64) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .MaxDepth = maxDepth,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi ke StringBuilder untuk performa tinggi
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <returns>StringBuilder berisi JSON</returns>
    Public Shared Function ToJsonStringBuilder(Of T)(obj As T) As StringBuilder
        Dim sb As New StringBuilder()

        If obj Is Nothing Then
            sb.Append("null")
            Return sb
        End If

        Try
            Using writer As New IO.StringWriter(sb)
                Dim serializer = JsonSerializer.Create(
                    New JsonSerializerSettings With {
                        .Formatting = Formatting.Indented,
                        .NullValueHandling = NullValueHandling.Ignore
                    })
                serializer.Serialize(writer, obj)
            End Using
            Return sb
        Catch
            sb.Clear()
            sb.Append("null")
            Return sb
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan ReferenceLoopHandling kustom
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="loopHandling">Cara menangani circular reference</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonDenganLoopHandling(Of T)(
        obj As T,
        Optional loopHandling As ReferenceLoopHandling = ReferenceLoopHandling.Ignore) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .ReferenceLoopHandling = loopHandling,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan PreserveReferencesHandling
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="preserveReferences">True untuk preserve $id/$ref</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonDenganPreserveRef(Of T)(
        obj As T,
        Optional preserveReferences As Boolean = False) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim preserveHandling As PreserveReferencesHandling = If(
                preserveReferences,
                PreserveReferencesHandling.Objects,
                PreserveReferencesHandling.None)

            Dim pengaturan = New JsonSerializerSettings With {
                .PreserveReferencesHandling = preserveHandling,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan NullValueHandling kustom
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="nullHandling">Penanganan nilai null</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonDenganNullHandling(Of T)(
        obj As T,
        Optional nullHandling As NullValueHandling = NullValueHandling.Ignore) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .NullValueHandling = nullHandling,
                .Formatting = Formatting.Indented
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan MissingMemberHandling kustom
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="missingMemberHandling">Penanganan missing member</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonDenganMissingMember(Of T)(
        obj As T,
        Optional missingMemberHandling As MissingMemberHandling = MissingMemberHandling.Ignore) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .MissingMemberHandling = missingMemberHandling,
                .Formatting = Formatting.Indented,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi dengan semua opsi dalam satu method
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="formatRapi">Format rapi</param>
    ''' <param name="abaikanNull">Abaikan null</param>
    ''' <param name="gunakanCamelCase">CamelCase</param>
    ''' <param name="dateFormat">Format tanggal</param>
    ''' <param name="maxDepth">Kedalaman maks</param>
    ''' <param name="loopHandling">Reference loop handling</param>
    ''' <param name="sertakanTipeInfo">Sertakan $type</param>
    ''' <returns>String JSON</returns>
    Public Shared Function ToJsonLengkap(Of T)(
        obj As T,
        Optional formatRapi As Boolean = True,
        Optional abaikanNull As Boolean = True,
        Optional gunakanCamelCase As Boolean = False,
        Optional dateFormat As String = "yyyy-MM-dd HH:mm:ss",
        Optional maxDepth As Integer? = Nothing,
        Optional loopHandling As ReferenceLoopHandling? = Nothing,
        Optional sertakanTipeInfo As Boolean = False) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim jsonFormatting As Formatting = If(
                formatRapi,
                Formatting.Indented,
                Formatting.None)

            Dim nullHandling As NullValueHandling = If(
                abaikanNull,
                NullValueHandling.Ignore,
                NullValueHandling.Include)

            Dim pengaturan = New JsonSerializerSettings With {
                .Formatting = jsonFormatting,
                .NullValueHandling = nullHandling,
                .MissingMemberHandling = MissingMemberHandling.Ignore,
                .DateFormatString = dateFormat
            }

            If gunakanCamelCase Then
                pengaturan.ContractResolver =
                    New CamelCasePropertyNamesContractResolver()
            End If

            If maxDepth.HasValue Then
                pengaturan.MaxDepth = maxDepth.Value
            End If

            If loopHandling.HasValue Then
                pengaturan.ReferenceLoopHandling = loopHandling.Value
            End If

            If sertakanTipeInfo Then
                pengaturan.TypeNameHandling = TypeNameHandling.All
            End If

            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

End Class