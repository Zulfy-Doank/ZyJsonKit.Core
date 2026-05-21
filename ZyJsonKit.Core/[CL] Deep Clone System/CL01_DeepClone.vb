Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' CL01_DeepClone
''' Clone object secara mendalam (deep copy) via JSON serialization
''' Menghasilkan object baru yang sepenuhnya terpisah dari sumber
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class CL01_DeepClone

    ' ✅ Pre-built settings untuk Clone() - reusable, tidak dibuat ulang setiap call
    Private Shared ReadOnly _cloneSettings As New JsonSerializerSettings With {
        .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        .PreserveReferencesHandling = PreserveReferencesHandling.Objects
    }

    ''' <summary>
    ''' Deep clone object via JSON serialization
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object yang akan diclone</param>
    ''' <returns>Object hasil clone yang sepenuhnya terpisah</returns>
    Public Shared Function Clone(Of T)(obj As T) As T
        If obj Is Nothing Then Return Nothing

        Try
            ' ✅ Fix: settings diteruskan ke KEDUA serialize DAN deserialize
            ' Tanpa ini, $id/$ref yang di-generate saat serialize
            ' tidak bisa diresolve saat deserialize
            Dim json = JsonConvert.SerializeObject(obj, _cloneSettings)
            Return JsonConvert.DeserializeObject(Of T)(json, _cloneSettings)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deep clone dengan pengaturan kustom
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object sumber</param>
    ''' <param name="settings">Pengaturan serializer kustom</param>
    ''' <returns>Object hasil clone</returns>
    Public Shared Function CloneDenganSettings(Of T)(
        obj As T,
        settings As JsonSerializerSettings) As T

        If obj Is Nothing Then Return Nothing
        If settings Is Nothing Then Return Clone(obj)

        Try
            ' settings konsisten untuk serialize dan deserialize
            Dim json = JsonConvert.SerializeObject(obj, settings)
            Return JsonConvert.DeserializeObject(Of T)(json, settings)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deep clone JToken
    ''' </summary>
    ''' <param name="token">JToken yang akan diclone</param>
    ''' <returns>JToken hasil clone</returns>
    Public Shared Function CloneToken(token As JToken) As JToken
        If token Is Nothing Then Return Nothing

        Try
            Return token.DeepClone()
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Membuat salinan list yang deep cloned
    ''' </summary>
    ''' <typeparam name="T">Tipe item list</typeparam>
    ''' <param name="list">List sumber</param>
    ''' <returns>List baru dengan semua item terclone</returns>
    Public Shared Function CloneList(Of T)(list As List(Of T)) As List(Of T)
        If list Is Nothing OrElse list.Count = 0 Then
            Return New List(Of T)()
        End If

        Try
            Dim json = JsonConvert.SerializeObject(list)
            Dim hasil = JsonConvert.DeserializeObject(Of List(Of T))(json)
            Return If(hasil, New List(Of T)())
        Catch
            Return New List(Of T)()
        End Try
    End Function

End Class