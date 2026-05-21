Imports Newtonsoft.Json

''' <summary>
''' DS05_Default value fallback
''' Deserialisasi dengan fallback ke nilai default jika gagal
''' Memastikan selalu ada return value, tidak pernah null (kecuali diinginkan)
''' [PF03] Exception-safe - Selalu return value
''' [PF04] Null-safe - Handle null dengan default
''' </summary>
Public NotInheritable Class DS05_Default

    ''' <summary>
    ''' Deserialisasi dengan fallback ke default value
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="nilaiDefault">Nilai default jika gagal</param>
    ''' <returns>Object hasil atau nilai default</returns>
    Public Shared Function FromJson(Of T)(
        jsonString As String,
        Optional nilaiDefault As T = Nothing) As T

        If String.IsNullOrWhiteSpace(jsonString) Then
            Return nilaiDefault
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of T)(jsonString)
            ' Kembalikan hasil jika tidak null, jika null kembalikan default
            If hasil IsNot Nothing Then
                Return hasil
            End If
            Return nilaiDefault
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan default value untuk value types (Structure)
    ''' </summary>
    ''' <typeparam name="T">Tipe value (Integer, Boolean, DateTime, dll)</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="nilaiDefault">Nilai default jika gagal</param>
    ''' <returns>Value hasil atau default</returns>
    Public Shared Function FromJsonValue(Of T As Structure)(
        jsonString As String,
        Optional nilaiDefault As T = Nothing) As T

        If String.IsNullOrWhiteSpace(jsonString) Then
            Return nilaiDefault
        End If

        Try
            Return JsonConvert.DeserializeObject(Of T)(jsonString)
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan default factory (lambda)
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="defaultFactory">Function untuk membuat default value</param>
    ''' <returns>Object hasil atau hasil factory</returns>
    Public Shared Function FromJsonDenganFactory(Of T)(
        jsonString As String,
        defaultFactory As Func(Of T)) As T

        If String.IsNullOrWhiteSpace(jsonString) Then
            If defaultFactory IsNot Nothing Then
                Return defaultFactory()
            End If
            Return Nothing
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of T)(jsonString)
            If hasil IsNot Nothing Then
                Return hasil
            End If
        Catch
            ' Lanjut ke default
        End Try

        If defaultFactory IsNot Nothing Then
            Return defaultFactory()
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Deserialisasi List dengan default empty list
    ''' </summary>
    ''' <typeparam name="T">Tipe item</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>List(Of T), tidak pernah null (minimal empty list)</returns>
    Public Shared Function FromJsonList(Of T)(jsonArray As String) As List(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then
            Return New List(Of T)()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)

            ' ✅ IDE0029 Fix Line 98: Sederhanakan null check
            ' List(Of T) adalah reference type, aman untuk 2-parameter If()
            Return If(hasil, New List(Of T)())
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi Dictionary dengan default empty
    ''' </summary>
    ''' <typeparam name="TKey">Tipe key</typeparam>
    ''' <typeparam name="TValue">Tipe value</typeparam>
    ''' <param name="jsonObject">String JSON object</param>
    ''' <returns>Dictionary, tidak pernah null</returns>
    Public Shared Function FromJsonDictionary(Of TKey, TValue)(
        jsonObject As String) As Dictionary(Of TKey, TValue)

        If String.IsNullOrWhiteSpace(jsonObject) Then
            Return New Dictionary(Of TKey, TValue)()
        End If

        Try
            Dim hasil = JsonConvert.DeserializeObject(Of Dictionary(Of TKey, TValue))(jsonObject)

            ' ✅ IDE0029 Fix Line 118: Sederhanakan null check
            ' Dictionary(Of TKey, TValue) adalah reference type, aman untuk 2-parameter If()
            Return If(hasil, New Dictionary(Of TKey, TValue)())
        Catch
            Return New Dictionary(Of TKey, TValue)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan multiple fallback values
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="daftarDefault">Daftar default value (dicoba berurutan)</param>
    ''' <returns>Object hasil atau default pertama yang valid</returns>
    Public Shared Function FromJsonDenganFallback(Of T)(
        jsonString As String,
        ParamArray daftarDefault As T()) As T

        ' Coba deserialisasi
        If Not String.IsNullOrWhiteSpace(jsonString) Then
            Try
                Dim hasil = JsonConvert.DeserializeObject(Of T)(jsonString)
                If hasil IsNot Nothing Then Return hasil
            Catch
            End Try
        End If

        ' Coba daftar default
        If daftarDefault IsNot Nothing Then
            For Each nilai In daftarDefault
                If nilai IsNot Nothing Then
                    Return nilai
                End If
            Next
        End If

        Return Nothing
    End Function

End Class