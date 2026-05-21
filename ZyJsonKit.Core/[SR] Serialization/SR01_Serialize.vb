Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization

''' <summary>
''' SR01_Serialize
''' Serialisasi object ke string JSON standar
''' [PF02] Generic-safe - Aman untuk generic typing
''' [AF01] Generic methods - Method generic reusable
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class SR01_Serialize

    ' ✅ Pre-built settings untuk ToJsonRapi - reusable, tidak dibuat ulang setiap call
    Private Shared ReadOnly _rapiSettings As New JsonSerializerSettings With {
        .Formatting = Formatting.Indented,
        .NullValueHandling = NullValueHandling.Ignore
    }

    ' ✅ Pre-built settings untuk ToJsonCamelCase - reusable
    Private Shared ReadOnly _camelCaseSettings As New JsonSerializerSettings With {
        .ContractResolver = New CamelCasePropertyNamesContractResolver(),
        .Formatting = Formatting.Indented,
        .NullValueHandling = NullValueHandling.Ignore
    }

    ''' <summary>
    ''' Serialisasi object generic ke JSON
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object yang akan diserialisasi</param>
    ''' <param name="settings">Pengaturan serializer (opsional)</param>
    ''' <returns>String JSON hasil serialisasi</returns>
    Public Shared Function ToJson(Of T)(
        obj As T,
        Optional settings As JsonSerializerSettings = Nothing) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim pengaturan = If(settings, DF03_Global.DapatkanTerkini())
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch ex As Exception
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi object dengan formatting rapi
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <returns>JSON formatted indented</returns>
    Public Shared Function ToJsonRapi(Of T)(obj As T) As String
        If obj Is Nothing Then Return "null"

        Try
            ' ✅ Gunakan cached settings, tidak buat New setiap call
            Return JsonConvert.SerializeObject(obj, _rapiSettings)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi object dengan camelCase
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <returns>JSON camelCase</returns>
    Public Shared Function ToJsonCamelCase(Of T)(obj As T) As String
        If obj Is Nothing Then Return "null"

        Try
            ' ✅ Fix: Hapus Newtonsoft.Json.Serialization prefix (sudah di-import)
            ' ✅ Gunakan cached settings, tidak buat New setiap call
            Return JsonConvert.SerializeObject(obj, _camelCaseSettings)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi object dengan error handling detail
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>JSON atau Nothing jika error</returns>
    Public Shared Function ToJsonAman(Of T)(
        obj As T,
        ByRef pesanError As String) As String

        pesanError = String.Empty

        If obj Is Nothing Then
            pesanError = "Object tidak boleh null"
            Return "null"
        End If

        Try
            Return JsonConvert.SerializeObject(obj, Formatting.Indented)
        Catch ex As Exception
            pesanError = $"Gagal serialisasi: {ex.Message}"
            Return Nothing
        End Try
    End Function

End Class