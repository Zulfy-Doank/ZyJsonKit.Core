Imports Newtonsoft.Json

''' <summary>
''' SR02_Compact
''' Serialisasi tanpa indentasi (compact/minified)
''' [PF06] Memory optimized - Output lebih kecil
''' [PF02] Generic-safe
''' </summary>
Public NotInheritable Class SR02_Compact

    ''' <summary>
    ''' Serialisasi object ke JSON compact (tanpa spasi/indent)
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object yang akan diserialisasi</param>
    ''' <returns>String JSON compact</returns>
    Public Shared Function ToJson(Of T)(obj As T) As String
        If obj Is Nothing Then Return "null"

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .Formatting = Formatting.None,
                .NullValueHandling = NullValueHandling.Ignore,
                .MissingMemberHandling = MissingMemberHandling.Ignore
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi compact dengan opsi include null
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <param name="sertakanNull">True untuk menyertakan properti null</param>
    ''' <returns>JSON compact</returns>
    Public Shared Function ToJsonDenganNull(Of T)(
        obj As T,
        Optional sertakanNull As Boolean = False) As String

        If obj Is Nothing Then Return "null"

        Try
            Dim nullHandling As NullValueHandling = If(
                sertakanNull,
                NullValueHandling.Include,
                NullValueHandling.Ignore)

            Dim pengaturan = New JsonSerializerSettings With {
                .Formatting = Formatting.None,
                .NullValueHandling = nullHandling
            }
            Return JsonConvert.SerializeObject(obj, pengaturan)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi compact untuk list/collection
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="list">List object</param>
    ''' <returns>JSON array compact</returns>
    Public Shared Function ToJsonList(Of T)(list As IEnumerable(Of T)) As String
        If list Is Nothing Then Return "[]"

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .Formatting = Formatting.None,
                .NullValueHandling = NullValueHandling.Ignore
            }
            Return JsonConvert.SerializeObject(list, pengaturan)
        Catch
            Return "[]"
        End Try
    End Function

    ''' <summary>
    ''' Minify JSON string yang sudah ada
    ''' </summary>
    ''' <param name="jsonString">String JSON berformat</param>
    ''' <returns>JSON compact</returns>
    Public Shared Function Minify(jsonString As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            Dim obj = Newtonsoft.Json.Linq.JToken.Parse(jsonString)
            Return obj.ToString(Formatting.None)
        Catch
            Return System.Text.RegularExpressions.Regex.Replace(
                jsonString, "\s+", " ")
        End Try
    End Function

    ''' <summary>
    ''' Hitung ukuran JSON compact dalam bytes
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="obj">Object</param>
    ''' <returns>Ukuran dalam bytes</returns>
    Public Shared Function UkuranCompact(Of T)(obj As T) As Integer
        Dim json = ToJson(obj)
        Return System.Text.Encoding.UTF8.GetByteCount(json)
    End Function

End Class