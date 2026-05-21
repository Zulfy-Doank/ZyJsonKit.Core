Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports System.Text

''' <summary>
''' FM01_PrettyPrint
''' Format JSON menjadi rapi dan mudah dibaca (pretty print)
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class FM01_PrettyPrint
    ''' <summary>
    ''' Format JSON string menjadi pretty print (indent rapi)
    ''' </summary>
    ''' <param name="jsonString">String JSON (bisa compact atau berantakan)</param>
    ''' <returns>JSON yang sudah diformat rapi, string kosong jika invalid</returns>
    Public Shared Function Format(jsonString As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            Dim obj = JToken.Parse(jsonString)
            Return obj.ToString(Formatting.Indented)
        Catch
            Return jsonString ' Kembalikan asli jika bukan JSON valid
        End Try
    End Function

    ''' <summary>
    ''' Format JSON dengan custom indent
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="indentChar">Karakter indent (default: spasi)</param>
    ''' <param name="indentCount">Jumlah indent (default: 2)</param>
    ''' <returns>JSON terformat</returns>
    Public Shared Function FormatDenganIndent(jsonString As String, Optional indentChar As Char = " "c, Optional indentCount As Integer = 2) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            Dim obj = JToken.Parse(jsonString)
            Dim indentStr = New String(indentChar, indentCount)

            Using writer As New StringWriter()
                Using jsonWriter As New JsonTextWriter(writer) With {
                    .Formatting = Formatting.Indented,
                    .Indentation = indentStr.Length,
                    .IndentChar = indentChar
                }
                    obj.WriteTo(jsonWriter)
                End Using
                Return writer.ToString()
            End Using
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Format JSON dengan tab indent
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JSON dengan tab indent</returns>
    Public Shared Function FormatDenganTab(jsonString As String) As String
        Return FormatDenganIndent(jsonString, ControlChars.Tab, 1)
    End Function

    ''' <summary>
    ''' Format JSON dengan warna syntax (HTML)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>HTML string dengan syntax highlighting</returns>
    Public Shared Function FormatKeHtml(jsonString As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            ' Format dulu
            Dim formatted = Format(jsonString)

            ' Escape HTML
            formatted = System.Net.WebUtility.HtmlEncode(formatted)

            ' Tambahkan syntax highlighting sederhana
            Dim sb As New StringBuilder()
            sb.AppendLine("<pre style='background:#1e1e1e;color:#d4d4d4;padding:10px;border-radius:5px;font-family:Consolas,monospace;'>")
            sb.AppendLine("<code>")

            ' Highlight strings
            formatted = System.Text.RegularExpressions.Regex.Replace(
                formatted,
                "&quot;(.*?)&quot;",
                "<span style='color:#ce9178;'>$&</span>"
            )

            ' Highlight numbers
            formatted = System.Text.RegularExpressions.Regex.Replace(
                formatted,
                "\b(\d+\.?\d*)\b",
                "<span style='color:#b5cea8;'>$1</span>"
            )

            ' Highlight booleans dan null
            formatted = System.Text.RegularExpressions.Regex.Replace(
                formatted,
                "\b(true|false|null)\b",
                "<span style='color:#569cd6;'>$1</span>"
            )

            sb.AppendLine(formatted)
            sb.AppendLine("</code>")
            sb.AppendLine("</pre>")

            Return sb.ToString()
        Catch
            Return $"<pre>{jsonString}</pre>"
        End Try
    End Function

    ''' <summary>
    ''' Format JToken ke string rapi
    ''' </summary>
    ''' <param name="token">JToken</param>
    ''' <returns>JSON terformat</returns>
    Public Shared Function FormatToken(token As JToken) As String
        If token Is Nothing Then Return "null"

        Try
            Return token.ToString(Formatting.Indented)
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Format dan simpan ke file
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pathFile">Path file output</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function FormatDanSimpan(jsonString As String, pathFile As String) As Boolean
        Try
            Dim formatted = Format(jsonString)
            Return FO01_Save.Simpan(pathFile, formatted)
        Catch
            Return False
        End Try
    End Function
End Class