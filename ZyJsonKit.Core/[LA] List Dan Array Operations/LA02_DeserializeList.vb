Imports Newtonsoft.Json

''' <summary>
''' LA02_DeserializeList
''' Deserialisasi JSON array ke List(Of T)
''' [PF02] Generic-safe
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class LA02_DeserializeList

    ''' <summary>
    ''' Deserialisasi JSON array ke List(Of T)
    ''' </summary>
    ''' <typeparam name="T">Tipe item dalam list</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="settings">Pengaturan serializer (opsional)</param>
    ''' <returns>List(Of T), list kosong jika gagal atau input invalid</returns>
    Public Shared Function FromJson(Of T)(
        jsonArray As String,
        Optional settings As JsonSerializerSettings = Nothing) As List(Of T)

        If String.IsNullOrWhiteSpace(jsonArray) Then
            Return New List(Of T)()
        End If

        Try
            Dim pengaturan = If(settings, New JsonSerializerSettings With {
                .MissingMemberHandling = MissingMemberHandling.Ignore,
                .NullValueHandling = NullValueHandling.Ignore
            })

            Dim hasil = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray, pengaturan)
            Return If(hasil, New List(Of T)())
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi JSON camelCase ke List
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonCamelCase">String JSON camelCase array</param>
    ''' <returns>List(Of T)</returns>
    Public Shared Function FromJsonCamelCase(Of T)(jsonCamelCase As String) As List(Of T)
        If String.IsNullOrWhiteSpace(jsonCamelCase) Then Return New List(Of T)()

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver =
                    New Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                .MissingMemberHandling = MissingMemberHandling.Ignore
            }
            Return FromJson(Of T)(jsonCamelCase, pengaturan)
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dari file JSON ke List
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>List(Of T)</returns>
    Public Shared Function FromFile(Of T)(pathFile As String) As List(Of T)
        If Not IO.File.Exists(pathFile) Then Return New List(Of T)()

        Try
            Dim jsonString = IO.File.ReadAllText(pathFile, Text.Encoding.UTF8)
            Return FromJson(Of T)(jsonString)
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan error handling detail
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>List(Of T)</returns>
    Public Shared Function FromJsonAman(Of T)(
        jsonArray As String,
        ByRef pesanError As String) As List(Of T)

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonArray) Then
            pesanError = "JSON string kosong"
            Return New List(Of T)()
        End If

        If Not JP01_Validate.ApakahValid(jsonArray) Then
            pesanError = "Format JSON tidak valid"
            Return New List(Of T)()
        End If

        If Not JP01_Validate.ApakahArray(jsonArray) Then
            pesanError = "JSON bukan array"
            Return New List(Of T)()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            Return If(hasil, New List(Of T)())
        Catch ex As Exception
            pesanError = $"Gagal deserialisasi list: {ex.Message}"
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan filter (hanya item yang memenuhi kondisi)
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="kondisi">Callback filter</param>
    ''' <returns>List terfilter</returns>
    Public Shared Function FromJsonDenganFilter(Of T)(
        jsonArray As String,
        kondisi As Func(Of T, Boolean)) As List(Of T)

        Dim list = FromJson(Of T)(jsonArray)

        If kondisi Is Nothing Then Return list

        Try
            Return list.Where(kondisi).ToList()
        Catch
            Return list
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dan transformasi
    ''' </summary>
    ''' <typeparam name="T">Tipe item sumber</typeparam>
    ''' <typeparam name="TResult">Tipe hasil transformasi</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <param name="transform">Callback transformasi</param>
    ''' <returns>List hasil transformasi</returns>
    Public Shared Function FromJsonDanTransform(Of T, TResult)(
        jsonArray As String,
        transform As Func(Of T, TResult)) As List(Of TResult)

        Dim list = FromJson(Of T)(jsonArray)

        If transform Is Nothing Then Return New List(Of TResult)()

        Try
            Return list.Select(transform).ToList()
        Catch
            Return New List(Of TResult)()
        End Try
    End Function

End Class