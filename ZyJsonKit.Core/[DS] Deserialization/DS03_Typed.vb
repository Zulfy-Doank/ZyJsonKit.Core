Imports Newtonsoft.Json

''' <summary>
''' DS03_Typed
''' Deserialisasi menggunakan Type parameter (bukan generic)
''' Berguna saat tipe object diketahui saat runtime
''' [PF01] Option Strict On compatible
''' [PF03] Exception-safe
''' </summary>
Public NotInheritable Class DS03_Typed

    ''' <summary>
    ''' Deserialisasi dengan Type parameter
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="tipe">Type target deserialisasi</param>
    ''' <returns>Object hasil deserialisasi, Nothing jika gagal</returns>
    Public Shared Function FromJson(
        jsonString As String,
        tipe As Type) As Object

        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing
        If tipe Is Nothing Then Return Nothing

        Try
            Return JsonConvert.DeserializeObject(jsonString, tipe)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan Type dan pengaturan kustom
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="tipe">Type target</param>
    ''' <param name="settings">Pengaturan serializer</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonDenganSettings(
        jsonString As String,
        tipe As Type,
        settings As JsonSerializerSettings) As Object

        If String.IsNullOrWhiteSpace(jsonString) OrElse tipe Is Nothing Then
            Return Nothing
        End If

        Try
            Dim pengaturan = If(settings, New JsonSerializerSettings())
            Return JsonConvert.DeserializeObject(jsonString, tipe, pengaturan)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dari qualified type name
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="namaTipe">Nama tipe lengkap</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonByTypeName(
        jsonString As String,
        namaTipe As String) As Object

        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing
        If String.IsNullOrWhiteSpace(namaTipe) Then Return Nothing

        Try
            Dim tipe = Type.GetType(namaTipe)
            If tipe Is Nothing Then Return Nothing
            Return FromJson(jsonString, tipe)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi ke List dengan tipe item dinamis
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="tipeItem">Tipe item dalam list</param>
    ''' <returns>Object berupa IList</returns>
    Public Shared Function FromJsonToList(
        jsonArray As String,
        tipeItem As Type) As System.Collections.IList

        If String.IsNullOrWhiteSpace(jsonArray) Then Return Nothing
        If tipeItem Is Nothing Then Return Nothing

        Try
            Dim tipeList = GetType(List(Of )).MakeGenericType(tipeItem)
            Return DirectCast(
                JsonConvert.DeserializeObject(jsonArray, tipeList),
                System.Collections.IList)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi ke Dictionary dengan tipe dinamis
    ''' </summary>
    ''' <param name="jsonObject">String JSON object</param>
    ''' <param name="tipeKey">Tipe key</param>
    ''' <param name="tipeValue">Tipe value</param>
    ''' <returns>Object berupa IDictionary</returns>
    Public Shared Function FromJsonToDictionary(
        jsonObject As String,
        tipeKey As Type,
        tipeValue As Type) As System.Collections.IDictionary

        If String.IsNullOrWhiteSpace(jsonObject) Then Return Nothing
        If tipeKey Is Nothing OrElse tipeValue Is Nothing Then Return Nothing

        Try
            Dim tipeDict = GetType(Dictionary(Of ,)).MakeGenericType(tipeKey, tipeValue)
            Return DirectCast(
                JsonConvert.DeserializeObject(jsonObject, tipeDict),
                System.Collections.IDictionary)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan error handling
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="tipe">Type target</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonAman(
        jsonString As String,
        tipe As Type,
        ByRef pesanError As String) As Object

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string kosong"
            Return Nothing
        End If

        If tipe Is Nothing Then
            pesanError = "Type tidak boleh null"
            Return Nothing
        End If

        Try
            Return JsonConvert.DeserializeObject(jsonString, tipe)
        Catch ex As Exception
            pesanError = $"Gagal deserialisasi: {ex.Message}"
            Return Nothing
        End Try
    End Function

End Class