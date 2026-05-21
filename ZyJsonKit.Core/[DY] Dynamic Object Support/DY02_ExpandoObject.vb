Imports Newtonsoft.Json
Imports System.Dynamic

''' <summary>
''' DY02_ExpandoObject support
''' Support untuk ExpandoObject: konversi, manipulasi, serialisasi
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DY02_ExpandoObject
    ''' <summary>
    ''' Membuat ExpandoObject baru
    ''' </summary>
    ''' <returns>ExpandoObject kosong</returns>
    Public Shared Function Buat() As ExpandoObject
        Return New ExpandoObject()
    End Function

    ''' <summary>
    ''' Membuat ExpandoObject dari Dictionary
    ''' </summary>
    ''' <param name="dictionary">Dictionary sumber</param>
    ''' <returns>ExpandoObject</returns>
    Public Shared Function DariDictionary(dictionary As Dictionary(Of String, Object)) As ExpandoObject
        If dictionary Is Nothing Then Return New ExpandoObject()

        Dim expando As IDictionary(Of String, Object) = New ExpandoObject()

        Try
            For Each kvp In dictionary
                expando.Add(kvp.Key, kvp.Value)
            Next
        Catch
        End Try

        Return DirectCast(expando, ExpandoObject)
    End Function

    ''' <summary>
    ''' Konversi ExpandoObject ke Dictionary
    ''' </summary>
    ''' <param name="expando">ExpandoObject</param>
    ''' <returns>Dictionary(Of String, Object)</returns>
    Public Shared Function KeDictionary(expando As ExpandoObject) As Dictionary(Of String, Object)
        If expando Is Nothing Then Return New Dictionary(Of String, Object)()

        Try
            Return New Dictionary(Of String, Object)(DirectCast(expando, IDictionary(Of String, Object)))
        Catch
            Return New Dictionary(Of String, Object)()
        End Try
    End Function

    ''' <summary>
    ''' Ambil value dari ExpandoObject dengan tipe aman
    ''' </summary>
    ''' <typeparam name="T">Tipe value yang diinginkan</typeparam>
    ''' <param name="expando">ExpandoObject</param>
    ''' <param name="namaProperti">Nama properti</param>
    ''' <param name="nilaiDefault">Nilai default jika tidak ditemukan</param>
    ''' <returns>Value dengan tipe T atau default</returns>
    Public Shared Function AmbilValue(Of T)(expando As ExpandoObject, namaProperti As String, Optional nilaiDefault As T = Nothing) As T
        If expando Is Nothing OrElse String.IsNullOrWhiteSpace(namaProperti) Then
            Return nilaiDefault
        End If

        Try
            Dim dict = DirectCast(expando, IDictionary(Of String, Object))
            If dict.ContainsKey(namaProperti) Then
                Dim value = dict(namaProperti)
                If value IsNot Nothing Then
                    ' Jika tipe sudah sesuai
                    If TypeOf value Is T Then
                        Return DirectCast(value, T)
                    End If
                    ' Coba konversi
                    Return DirectCast(Convert.ChangeType(value, GetType(T)), T)
                End If
            End If
        Catch
        End Try

        Return nilaiDefault
    End Function

    ''' <summary>
    ''' Set properti ExpandoObject
    ''' </summary>
    ''' <param name="expando">ExpandoObject</param>
    ''' <param name="nama">Nama properti</param>
    ''' <param name="value">Value</param>
    Public Shared Sub AturProperti(expando As ExpandoObject, nama As String, value As Object)
        If expando Is Nothing OrElse String.IsNullOrWhiteSpace(nama) Then Return

        Try
            Dim dict = DirectCast(expando, IDictionary(Of String, Object))
            If dict.ContainsKey(nama) Then
                dict(nama) = value
            Else
                dict.Add(nama, value)
            End If
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Hapus properti ExpandoObject
    ''' </summary>
    Public Shared Function HapusProperti(expando As ExpandoObject, nama As String) As Boolean
        If expando Is Nothing OrElse String.IsNullOrWhiteSpace(nama) Then Return False

        Try
            Dim dict = DirectCast(expando, IDictionary(Of String, Object))
            Return dict.Remove(nama)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Cek properti exists
    ''' </summary>
    Public Shared Function PropertiAda(expando As ExpandoObject, nama As String) As Boolean
        If expando Is Nothing Then Return False

        Try
            Dim dict = DirectCast(expando, IDictionary(Of String, Object))
            Return dict.ContainsKey(nama)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Serialisasi ExpandoObject ke JSON
    ''' </summary>
    Public Shared Function KeJson(expando As ExpandoObject, Optional formatRapi As Boolean = True) As String
        If expando Is Nothing Then Return "null"

        Try
            Return JsonConvert.SerializeObject(expando, If(formatRapi, Formatting.Indented, Formatting.None))
        Catch
            Return "null"
        End Try
    End Function

    ''' <summary>
    ''' Dapatkan semua nama properti
    ''' </summary>
    Public Shared Function DaftarProperti(expando As ExpandoObject) As List(Of String)
        If expando Is Nothing Then Return New List(Of String)()

        Try
            Dim dict = DirectCast(expando, IDictionary(Of String, Object))
            Return dict.Keys.ToList()
        Catch
            Return New List(Of String)()
        End Try
    End Function
End Class