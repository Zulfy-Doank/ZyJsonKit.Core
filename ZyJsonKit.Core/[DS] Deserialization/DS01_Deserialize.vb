Imports Newtonsoft.Json

''' <summary>
''' DS01_Deserialize
''' Deserialisasi JSON ke object standar
''' [PF02] Generic-safe - Aman untuk generic typing
''' [AF01] Generic methods - Method generic reusable
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class DS01_Deserialize

    ''' <summary>
    ''' Deserialisasi JSON ke object
    ''' </summary>
    ''' <typeparam name="T">Tipe object target</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="settings">Pengaturan serializer (opsional)</param>
    ''' <returns>Object hasil deserialisasi, Nothing jika gagal</returns>
    Public Shared Function FromJson(Of T)(
        jsonString As String,
        Optional settings As JsonSerializerSettings = Nothing) As T

        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Dim pengaturan = If(settings, DF03_Global.DapatkanTerkini())
            Return JsonConvert.DeserializeObject(Of T)(jsonString, pengaturan)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi JSON camelCase ke object
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonCamelCase">String JSON camelCase</param>
    ''' <returns>Object hasil deserialisasi</returns>
    Public Shared Function FromJsonCamelCase(Of T)(jsonCamelCase As String) As T
        If String.IsNullOrWhiteSpace(jsonCamelCase) Then Return Nothing

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver =
                    New Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                .MissingMemberHandling = MissingMemberHandling.Ignore,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.DeserializeObject(Of T)(jsonCamelCase, pengaturan)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi JSON snake_case ke object
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonSnakeCase">String JSON snake_case</param>
    ''' <returns>Object hasil deserialisasi</returns>
    Public Shared Function FromJsonSnakeCase(Of T)(jsonSnakeCase As String) As T
        If String.IsNullOrWhiteSpace(jsonSnakeCase) Then Return Nothing

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .ContractResolver =
                    New Newtonsoft.Json.Serialization.DefaultContractResolver With {
                        .NamingStrategy =
                            New Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
                    },
                .MissingMemberHandling = MissingMemberHandling.Ignore
            }
            Return JsonConvert.DeserializeObject(Of T)(jsonSnakeCase, pengaturan)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dari file JSON
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>Object hasil deserialisasi</returns>
    Public Shared Function FromJsonFile(Of T)(pathFile As String) As T
        If Not IO.File.Exists(pathFile) Then Return Nothing

        Try
            Dim jsonString = IO.File.ReadAllText(pathFile, Text.Encoding.UTF8)
            Return FromJson(Of T)(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan error handling
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>Object hasil atau Nothing</returns>
    Public Shared Function FromJsonAman(Of T)(
        jsonString As String,
        ByRef pesanError As String) As T

        pesanError = String.Empty

        If String.IsNullOrWhiteSpace(jsonString) Then
            pesanError = "JSON string kosong"
            Return Nothing
        End If

        Try
            Return JsonConvert.DeserializeObject(Of T)(jsonString)
        Catch ex As Exception
            pesanError = $"Gagal deserialisasi: {ex.Message}"
            Return Nothing
        End Try
    End Function

End Class