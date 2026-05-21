Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions

''' <summary>
''' FM02_Minify JSON
''' Menghapus whitespace dari JSON (minify/compact)
''' Menghasilkan JSON dengan ukuran terkecil
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' [PF06] Memory optimized
''' </summary>
Public NotInheritable Class FM02_Minify
    ''' <summary>
    ''' Minify JSON string (hapus semua whitespace tidak penting)
    ''' </summary>
    ''' <param name="jsonString">String JSON (bisa formatted atau compact)</param>
    ''' <returns>JSON compact tanpa spasi/indent berlebih</returns>
    Public Shared Function Minify(jsonString As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            ' Gunakan JToken untuk minify yang akurat
            Dim obj = JToken.Parse(jsonString)
            Return obj.ToString(Formatting.None)
        Catch
            ' Fallback: hapus whitespace manual jika bukan JSON valid
            Return MinifyManual(jsonString)
        End Try
    End Function

    ''' <summary>
    ''' Minify manual dengan regex (fallback)
    ''' </summary>
    Private Shared Function MinifyManual(jsonString As String) As String
        If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty

        Try
            ' Hapus whitespace di luar string
            Dim result = Regex.Replace(jsonString, "\s+", " ")
            result = Regex.Replace(result, "\s*([{}\[\]:,])\s*", "$1")
            Return result.Trim()
        Catch
            Return jsonString
        End Try
    End Function

    ''' <summary>
    ''' Minify JSON dari file
    ''' </summary>
    ''' <param name="pathFile">Path file JSON</param>
    ''' <returns>JSON compact, Nothing jika gagal</returns>
    Public Shared Function MinifyDariFile(pathFile As String) As String
        If Not IO.File.Exists(pathFile) Then Return Nothing

        Try
            Dim jsonString = IO.File.ReadAllText(pathFile, Text.Encoding.UTF8)
            Return Minify(jsonString)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Minify dan simpan ke file
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pathFile">Path file output</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function MinifyDanSimpan(jsonString As String, pathFile As String) As Boolean
        Try
            Dim minified = Minify(jsonString)
            Return FO01_Save.Simpan(pathFile, minified)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Bandingkan ukuran sebelum dan sesudah minify
    ''' </summary>
    ''' <param name="jsonString">String JSON original</param>
    ''' <returns>Informasi perbandingan ukuran</returns>
    Public Shared Function BandingkanUkuran(jsonString As String) As MinifyInfo
        Dim info As New MinifyInfo()

        If String.IsNullOrWhiteSpace(jsonString) Then Return info

        Try
            Dim minified = Minify(jsonString)

            info.UkuranOriginal = Text.Encoding.UTF8.GetByteCount(jsonString)
            info.UkuranMinified = Text.Encoding.UTF8.GetByteCount(minified)
            info.PenghematanBytes = info.UkuranOriginal - info.UkuranMinified

            If info.UkuranOriginal > 0 Then
                info.PersentasePenghematan = Math.Round((info.PenghematanBytes / info.UkuranOriginal) * 100, 2)
            End If
        Catch
        End Try

        Return info
    End Function

    ''' <summary>
    ''' Minify dengan opsi preserve certain whitespace
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="preserveNewlines">Pertahankan newline</param>
    ''' <returns>JSON minified</returns>
    Public Shared Function MinifyDenganOpsi(jsonString As String, Optional preserveNewlines As Boolean = False) As String
        If preserveNewlines Then
            ' Hanya hapus indentasi, pertahankan newline
            If String.IsNullOrWhiteSpace(jsonString) Then Return String.Empty
            Try
                Return Regex.Replace(jsonString, "^\s+", "", RegexOptions.Multiline)
            Catch
                Return jsonString
            End Try
        End If

        Return Minify(jsonString)
    End Function

#Region "MinifyInfo Class"

    ''' <summary>
    ''' Informasi hasil perbandingan ukuran minify
    ''' </summary>
    Public Class MinifyInfo
        Public Property UkuranOriginal As Integer = 0
        Public Property UkuranMinified As Integer = 0
        Public Property PenghematanBytes As Integer = 0
        Public Property PersentasePenghematan As Double = 0.0

        Public ReadOnly Property UkuranOriginalKB As Double
            Get
                Return Math.Round(UkuranOriginal / 1024.0, 2)
            End Get
        End Property

        Public ReadOnly Property UkuranMinifiedKB As Double
            Get
                Return Math.Round(UkuranMinified / 1024.0, 2)
            End Get
        End Property

        Public ReadOnly Property PenghematanKB As Double
            Get
                Return Math.Round(PenghematanBytes / 1024.0, 2)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"Original: {UkuranOriginalKB} KB | Minified: {UkuranMinifiedKB} KB | Hemat: {PenghematanKB} KB ({PersentasePenghematan}%)"
        End Function
    End Class

#End Region

End Class