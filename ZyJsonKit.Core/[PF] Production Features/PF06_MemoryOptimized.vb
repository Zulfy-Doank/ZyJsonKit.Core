Imports Newtonsoft.Json
Imports System.IO
Imports System.Text

''' <summary>
''' PF06_Memory optimized
''' Optimasi penggunaan memory saat serialisasi/deserialisasi
''' Menggunakan StreamWriter/StreamReader untuk mengurangi alokasi
''' </summary>
Public NotInheritable Class PF06_MemoryOptimized
    ''' <summary>
    ''' Serialize dengan StringWriter (lebih hemat memory)
    ''' </summary>
    Public Shared Function Serialize(Of T)(obj As T, Optional formatRapi As Boolean = False) As String
        If obj Is Nothing Then Return "null"

        Try
            Using stringWriter As New StringWriter()
                Using jsonWriter As New JsonTextWriter(stringWriter) With {
                    .Formatting = If(formatRapi, Formatting.Indented, Formatting.None)
                }
                    Dim serializer = JsonSerializer.Create(New JsonSerializerSettings With {
                        .NullValueHandling = NullValueHandling.Ignore,
                        .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                    serializer.Serialize(jsonWriter, obj)
                End Using
                Return stringWriter.ToString()
            End Using
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Serialize ke file langsung (tanpa string perantara)
    ''' </summary>
    Public Shared Function SerializeKeFile(Of T)(obj As T, pathFile As String, Optional formatRapi As Boolean = True) As Boolean
        If obj Is Nothing OrElse String.IsNullOrWhiteSpace(pathFile) Then Return False

        Try
            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            Using fileStream As New FileStream(pathFile, FileMode.Create, FileAccess.Write)
                Using streamWriter As New StreamWriter(fileStream, Encoding.UTF8)
                    Using jsonWriter As New JsonTextWriter(streamWriter) With {
                        .Formatting = If(formatRapi, Formatting.Indented, Formatting.None)
                    }
                        Dim serializer = JsonSerializer.Create(New JsonSerializerSettings With {
                            .NullValueHandling = NullValueHandling.Ignore
                        })
                        serializer.Serialize(jsonWriter, obj)
                    End Using
                End Using
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Deserialize dari file langsung (tanpa string perantara)
    ''' </summary>
    Public Shared Function DeserializeDariFile(Of T)(pathFile As String) As T
        If Not File.Exists(pathFile) Then Return Nothing

        Try
            Using fileStream As New FileStream(pathFile, FileMode.Open, FileAccess.Read)
                Using streamReader As New StreamReader(fileStream, Encoding.UTF8)
                    Using jsonReader As New JsonTextReader(streamReader)
                        Dim serializer = JsonSerializer.Create(New JsonSerializerSettings With {
                            .MissingMemberHandling = MissingMemberHandling.Ignore
                        })
                        Return serializer.Deserialize(Of T)(jsonReader)
                    End Using
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Menggunakan StringBuilder untuk efisiensi
    ''' </summary>
    Public Shared Function SerializeDenganStringBuilder(Of T)(obj As T) As StringBuilder
        Dim sb As New StringBuilder()

        If obj Is Nothing Then
            sb.Append("null")
            Return sb
        End If

        Try
            Using stringWriter As New StringWriter(sb)
                Dim serializer = JsonSerializer.Create(New JsonSerializerSettings With {
                    .NullValueHandling = NullValueHandling.Ignore,
                    .Formatting = Formatting.None
                })
                serializer.Serialize(stringWriter, obj)
            End Using
            Return sb
        Catch
            sb.Clear()
            sb.Append("null")
            Return sb
        End Try
    End Function
End Class