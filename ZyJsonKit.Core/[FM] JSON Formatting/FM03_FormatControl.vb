Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports System.Text

''' <summary>
''' FM03_Formatting control
''' Kontrol formatting JSON dengan berbagai opsi kustom
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' [AF05] Extensible - Mudah ditambah opsi baru
''' </summary>
Public NotInheritable Class FM03_FormatControl
    ''' <summary>
    ''' Format JSON dengan opsi kustom lengkap
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="formatRapi">True = indent, False = compact</param>
    ''' <param name="indentChar">Karakter indent (default: spasi)</param>
    ''' <param name="indentCount">Jumlah indent (default: 2)</param>
    ''' <returns>JSON terformat</returns>
    Public Shared Function Format(
        jsonString As String,
        Optional formatRapi As Boolean = True,
        Optional indentChar As Char = " "c,
        Optional indentCount As Integer = 2) As String

        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            Dim obj = JToken.Parse(jsonString)
            ' Gunakan enum Formatting langsung, bukan variable yang sama dengan nama fungsi
            Dim fmt As Formatting = If(formatRapi, Formatting.Indented, Formatting.None)

            If formatRapi Then
                Using writer As New StringWriter()
                    Using jsonWriter As New JsonTextWriter(writer) With {
                        .Formatting = fmt,
                        .Indentation = indentCount,
                        .IndentChar = indentChar
                    }
                        obj.WriteTo(jsonWriter)
                    End Using
                    Return writer.ToString()
                End Using
            Else
                Return obj.ToString(Formatting.None)
            End If
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Format JSON dengan wrapper (tambahkan root key)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="rootKey">Nama root key wrapper</param>
    ''' <returns>JSON dengan root wrapper</returns>
    Public Shared Function Bungkus(jsonString As String, rootKey As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return "{}"
        If String.IsNullOrWhiteSpace(rootKey) Then Return jsonString

        Try
            Dim token = JToken.Parse(jsonString)
            Dim wrapper As New JObject From {
                {rootKey, token}
            }
            Return wrapper.ToString(Formatting.Indented)
        Catch
            Return $"{{""{rootKey}"":{jsonString}}}"
        End Try
    End Function

    ''' <summary>
    ''' Unwrap JSON (hapus root key)
    ''' </summary>
    ''' <param name="jsonString">String JSON dengan wrapper</param>
    ''' <param name="rootKey">Nama root key yang akan dihapus</param>
    ''' <returns>JSON tanpa wrapper</returns>
    Public Shared Function BukaBungkus(jsonString As String, rootKey As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            Dim obj = JObject.Parse(jsonString)
            Dim token = obj.SelectToken(rootKey)
            If token IsNot Nothing Then
                Return token.ToString(Formatting.Indented)
            End If
            Return jsonString
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Format JSON dengan line break setelah setiap properti
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JSON dengan line break antar properti</returns>
    Public Shared Function FormatDenganLineBreak(jsonString As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            Dim obj = JToken.Parse(jsonString)

            Using writer As New StringWriter()
                Using jsonWriter As New JsonTextWriter(writer) With {
                    .Formatting = Formatting.Indented,
                    .Indentation = 4,
                    .IndentChar = " "c
                }
                    obj.WriteTo(jsonWriter)
                End Using
                Return writer.ToString().TrimStart()
            End Using
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Format JSON array menjadi satu item per baris
    ''' </summary>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>JSON array dengan setiap item di baris terpisah</returns>
    Public Shared Function FormatArrayPerBaris(jsonArray As String) As String
        If String.IsNullOrWhiteSpace(jsonArray) Then Return "[]"

        Try
            Dim arr = JArray.Parse(jsonArray)
            Dim sb As New StringBuilder()
            sb.AppendLine("[")

            For i As Integer = 0 To arr.Count - 1
                Dim item = arr(i).ToString(Formatting.None)
                sb.Append("  " & item)

                If i < arr.Count - 1 Then
                    sb.AppendLine(",")
                Else
                    sb.AppendLine()
                End If
            Next

            sb.Append("]")
            Return sb.ToString()
        Catch
            Return jsonArray
        End Try
    End Function

    ''' <summary>
    ''' Sort properti JSON secara alfabetis
    ''' </summary>
    ''' <param name="jsonString">String JSON object</param>
    ''' <param name="ascending">True = A-Z, False = Z-A</param>
    ''' <returns>JSON dengan properti terurut</returns>
    Public Shared Function UrutkanProperti(jsonString As String, Optional ascending As Boolean = True) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return "{}"

        Try
            Dim obj = JObject.Parse(jsonString)
            Dim sorted As New JObject()

            Dim properti = obj.Properties().ToList()
            If ascending Then
                properti = properti.OrderBy(Function(p) p.Name).ToList()
            Else
                properti = properti.OrderByDescending(Function(p) p.Name).ToList()
            End If

            For Each prop In properti
                sorted.Add(prop.Name, prop.Value)
            Next

            Return sorted.ToString(Formatting.Indented)
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Hapus properti tertentu dari JSON
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="daftarProperti">Daftar properti yang akan dihapus</param>
    ''' <returns>JSON tanpa properti yang dihapus</returns>
    Public Shared Function HapusProperti(jsonString As String, ParamArray daftarProperti As String()) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return "{}"
        If daftarProperti Is Nothing OrElse daftarProperti.Length = 0 Then Return jsonString

        Try
            Dim obj = JObject.Parse(jsonString)

            For Each namaProp In daftarProperti
                obj.Remove(namaProp)
            Next

            Return obj.ToString(Formatting.Indented)
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Hanya pertahankan properti tertentu (whitelist)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="daftarProperti">Daftar properti yang dipertahankan</param>
    ''' <returns>JSON hanya dengan properti yang dipilih</returns>
    Public Shared Function PertahankanProperti(jsonString As String, ParamArray daftarProperti As String()) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return "{}"
        If daftarProperti Is Nothing OrElse daftarProperti.Length = 0 Then Return "{}"

        Try
            Dim obj = JObject.Parse(jsonString)
            Dim hasil As New JObject()

            For Each namaProp In daftarProperti
                Dim token = obj.SelectToken(namaProp)
                If token IsNot Nothing Then
                    hasil.Add(namaProp, token)
                End If
            Next

            Return hasil.ToString(Formatting.Indented)
        Catch
            Return "{}"
        End Try
    End Function

    ''' <summary>
    ''' Konversi JSON ke single line string (escape)
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Single line escaped JSON</returns>
    Public Shared Function KeSingleLine(jsonString As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            Dim minified = FM02_Minify.Minify(jsonString)
            ' Escape double quotes untuk penggunaan dalam string literal
            Return minified.Replace("""", "\""")
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Mendapatkan statistik JSON
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Informasi statistik</returns>
    Public Shared Function Statistik(jsonString As String) As JsonStatistics
        Dim stats As New JsonStatistics()

        If String.IsNullOrWhiteSpace(jsonString) Then Return stats

        Try
            Dim obj = JToken.Parse(jsonString)

            stats.TipeToken = obj.Type.ToString()
            stats.JumlahKarakter = jsonString.Length
            stats.UkuranBytes = Text.Encoding.UTF8.GetByteCount(jsonString)
            stats.FormatCompact = FM02_Minify.Minify(jsonString)
            stats.UkuranBytesCompact = Text.Encoding.UTF8.GetByteCount(stats.FormatCompact)

            If TypeOf obj Is JObject Then
                Dim jObj = DirectCast(obj, JObject)
                stats.JumlahProperti = jObj.Count
                stats.DaftarProperti = jObj.Properties().Select(Function(p) p.Name).ToList()
            ElseIf TypeOf obj Is JArray Then
                Dim jArr = DirectCast(obj, JArray)
                stats.JumlahItem = jArr.Count
            End If

            ' Hitung nesting depth
            stats.KedalamanMaks = HitungKedalaman(obj)
        Catch
        End Try

        Return stats
    End Function

    ''' <summary>
    ''' Hitung kedalaman maksimum JSON
    ''' </summary>
    Private Shared Function HitungKedalaman(token As JToken) As Integer
        If token Is Nothing Then Return 0

        Dim maxDepth As Integer = 1

        If TypeOf token Is JObject Then
            For Each prop In DirectCast(token, JObject).Properties()
                Dim childDepth = HitungKedalaman(prop.Value) + 1
                If childDepth > maxDepth Then maxDepth = childDepth
            Next
        ElseIf TypeOf token Is JArray Then
            For Each item In DirectCast(token, JArray)
                Dim childDepth = HitungKedalaman(item) + 1
                If childDepth > maxDepth Then maxDepth = childDepth
            Next
        End If

        Return maxDepth
    End Function

#Region "JsonStatistics Class"

    ''' <summary>
    ''' Statistik JSON
    ''' </summary>
    Public Class JsonStatistics
        Public Property TipeToken As String = ""
        Public Property JumlahKarakter As Integer = 0
        Public Property UkuranBytes As Integer = 0
        Public Property UkuranBytesCompact As Integer = 0
        Public Property FormatCompact As String = ""
        Public Property JumlahProperti As Integer = 0
        Public Property JumlahItem As Integer = 0
        Public Property KedalamanMaks As Integer = 0
        Public Property DaftarProperti As List(Of String) = New List(Of String)()

        Public ReadOnly Property PenghematanPersen As Double
            Get
                If UkuranBytes > 0 Then
                    Return Math.Round((1 - (UkuranBytesCompact / UkuranBytes)) * 100, 2)
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property UkuranKB As Double
            Get
                Return Math.Round(UkuranBytes / 1024.0, 2)
            End Get
        End Property

        Public ReadOnly Property UkuranCompactKB As Double
            Get
                Return Math.Round(UkuranBytesCompact / 1024.0, 2)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendLine($"Tipe: {TipeToken}")
            sb.AppendLine($"Karakter: {JumlahKarakter:N0}")
            sb.AppendLine($"Ukuran: {UkuranKB} KB")
            sb.AppendLine($"Ukuran Compact: {UkuranCompactKB} KB")
            sb.AppendLine($"Hemat: {PenghematanPersen}%")
            sb.AppendLine($"Kedalaman Maks: {KedalamanMaks}")
            If JumlahProperti > 0 Then sb.AppendLine($"Jumlah Properti: {JumlahProperti}")
            If JumlahItem > 0 Then sb.AppendLine($"Jumlah Item: {JumlahItem}")
            If DaftarProperti.Count > 0 Then
                sb.AppendLine($"Properti: {String.Join(", ", DaftarProperti.Take(10))}")
                If DaftarProperti.Count > 10 Then sb.AppendLine($"  ... dan {DaftarProperti.Count - 10} lainnya")
            End If
            Return sb.ToString().TrimEnd()
        End Function
    End Class

#End Region

End Class