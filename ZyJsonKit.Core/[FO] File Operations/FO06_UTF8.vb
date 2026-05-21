Imports System.IO
Imports System.Text

''' <summary>
''' FO06_UTF8
''' Dukungan penuh untuk encoding UTF8 pada file JSON
''' Termasuk BOM, encoding detection, dan konversi
''' [PF06] Memory optimized
''' [AF02] Reusable design
''' </summary>
Public NotInheritable Class FO06_UTF8

    ''' <summary>
    ''' Menyimpan file JSON dengan UTF8 encoding
    ''' </summary>
    ''' <param name="pathFile">Path file tujuan</param>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="gunakanBOM">True untuk sertakan BOM</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function SimpanUTF8(
        pathFile As String,
        jsonString As String,
        Optional gunakanBOM As Boolean = True) As Boolean

        If String.IsNullOrWhiteSpace(pathFile) OrElse
           jsonString Is Nothing Then
            Return False
        End If

        Try
            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso
               Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            Dim encoding As Encoding = New UTF8Encoding(gunakanBOM)
            File.WriteAllText(pathFile, jsonString, encoding)
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Membaca file JSON dengan deteksi encoding
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <returns>String JSON, Nothing jika gagal</returns>
    Public Shared Function BacaUTF8(pathFile As String) As String
        If String.IsNullOrWhiteSpace(pathFile) OrElse
           Not File.Exists(pathFile) Then
            Return Nothing
        End If

        Try
            Dim bytes = File.ReadAllBytes(pathFile)
            Dim encoding = DeteksiEncoding(bytes)
            Return encoding.GetString(bytes)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Mengecek apakah file memiliki BOM UTF8
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <returns>True jika file memiliki UTF8 BOM</returns>
    Public Shared Function MemilikiBOM(pathFile As String) As Boolean
        If String.IsNullOrWhiteSpace(pathFile) OrElse
           Not File.Exists(pathFile) Then
            Return False
        End If

        Try
            Using fs As New FileStream(pathFile, FileMode.Open, FileAccess.Read)
                If fs.Length < 3 Then Return False

                Dim bom(2) As Byte
                fs.Read(bom, 0, 3)
                Return bom(0) = &HEF AndAlso
                       bom(1) = &HBB AndAlso
                       bom(2) = &HBF
            End Using
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Konversi file ke UTF8
    ''' </summary>
    ''' <param name="pathFile">Path file sumber</param>
    ''' <param name="pathOutput">Path file output (Nothing = timpa sumber)</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function KonversiKeUTF8(
        pathFile As String,
        Optional pathOutput As String = Nothing) As Boolean

        If String.IsNullOrWhiteSpace(pathFile) OrElse
           Not File.Exists(pathFile) Then
            Return False
        End If

        Dim outputPath = If(pathOutput, pathFile)

        Try
            Dim bytes = File.ReadAllBytes(pathFile)
            Dim encoding = DeteksiEncoding(bytes)
            Dim content = encoding.GetString(bytes)
            File.WriteAllText(outputPath, content, New UTF8Encoding(True))
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menghapus BOM dari file UTF8
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function HapusBOM(pathFile As String) As Boolean
        If String.IsNullOrWhiteSpace(pathFile) OrElse
           Not File.Exists(pathFile) Then
            Return False
        End If

        Try
            Dim content = BacaUTF8(pathFile)
            If content Is Nothing Then Return False

            File.WriteAllText(pathFile, content, New UTF8Encoding(False))
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Menyimpan string ke file UTF8 dengan aman
    ''' </summary>
    ''' <param name="pathFile">Path file</param>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>True jika berhasil</returns>
    Public Shared Function SimpanUTF8Aman(
        pathFile As String,
        jsonString As String) As Boolean

        If String.IsNullOrWhiteSpace(pathFile) OrElse
           jsonString Is Nothing Then
            Return False
        End If

        Try
            Dim direktori = Path.GetDirectoryName(pathFile)
            If Not String.IsNullOrEmpty(direktori) AndAlso
               Not Directory.Exists(direktori) Then
                Directory.CreateDirectory(direktori)
            End If

            Using writer As New StreamWriter(pathFile, False, New UTF8Encoding(True))
                writer.Write(jsonString)
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

#Region "Private Helpers"

    ''' <summary>
    ''' Deteksi encoding dari byte array berdasarkan BOM
    ''' </summary>
    Private Shared Function DeteksiEncoding(bytes As Byte()) As Encoding
        ' UTF8 BOM: EF BB BF
        If bytes.Length >= 3 AndAlso
           bytes(0) = &HEF AndAlso
           bytes(1) = &HBB AndAlso
           bytes(2) = &HBF Then
            Return Encoding.UTF8
        End If

        ' UTF16 LE BOM: FF FE
        If bytes.Length >= 2 AndAlso
           bytes(0) = &HFF AndAlso
           bytes(1) = &HFE Then
            Return Encoding.Unicode
        End If

        ' UTF16 BE BOM: FE FF
        If bytes.Length >= 2 AndAlso
           bytes(0) = &HFE AndAlso
           bytes(1) = &HFF Then
            Return Encoding.BigEndianUnicode
        End If

        ' UTF32 BOM: 00 00 FE FF
        If bytes.Length >= 4 AndAlso
           bytes(0) = 0 AndAlso
           bytes(1) = 0 AndAlso
           bytes(2) = &HFE AndAlso
           bytes(3) = &HFF Then
            Return Encoding.UTF32
        End If

        ' Default: coba UTF8
        Try
            Dim test = Encoding.UTF8.GetString(bytes)
            If test.Contains(ChrW(0)) Then
                Return Encoding.Default
            End If
            Return Encoding.UTF8
        Catch
            Return Encoding.Default
        End Try
    End Function

#End Region

End Class