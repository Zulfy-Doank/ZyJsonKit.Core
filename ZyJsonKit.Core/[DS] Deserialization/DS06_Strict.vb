Imports Newtonsoft.Json

''' <summary>
''' DS06_Strict
''' Deserialisasi full strict mode support
''' Kompatibel penuh dengan Option Strict On
''' [PF01] Option Strict On compatible - Semua casting explicit
''' [PF02] Generic-safe - Constraint class untuk reference types
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class DS06_Strict

    ''' <summary>
    ''' Deserialisasi dengan Option Strict On compatible
    ''' Hanya untuk reference types (Class)
    ''' </summary>
    ''' <typeparam name="T">Tipe object (harus Class, bukan Structure)</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Object hasil deserialisasi, Nothing jika gagal</returns>
    Public Shared Function FromJson(Of T As Class)(jsonString As String) As T
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Dim hasil As Object = JsonConvert.DeserializeObject(Of T)(jsonString)
            Return DirectCast(hasil, T)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan explicit type checking
    ''' </summary>
    ''' <typeparam name="T">Tipe object (harus Class)</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="pesanError">Output pesan error</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonAman(Of T As Class)(
        jsonString As String,
        ByRef pesanError As String) As T

        pesanError = String.Empty

        If jsonString Is Nothing Then
            pesanError = "JSON string tidak boleh null"
            Return Nothing
        End If

        If jsonString.Length = 0 Then
            pesanError = "JSON string tidak boleh kosong"
            Return Nothing
        End If

        Try
            Dim hasil As Object = JsonConvert.DeserializeObject(Of T)(jsonString)

            If hasil Is Nothing Then
                pesanError = "Hasil deserialisasi null"
                Return Nothing
            End If

            If TypeOf hasil IsNot T Then
                pesanError = $"Tipe hasil tidak cocok. " &
                             $"Expected: {GetType(T).Name}, " &
                             $"Actual: {hasil.GetType().Name}"
                Return Nothing
            End If

            Return DirectCast(hasil, T)

        Catch ex As JsonException
            pesanError = $"Error JSON: {ex.Message}"
            Return Nothing
        Catch ex As InvalidCastException
            pesanError = $"Error casting: {ex.Message}"
            Return Nothing
        Catch ex As Exception
            pesanError = $"Error: {ex.Message}"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan pengaturan strict
    ''' </summary>
    ''' <typeparam name="T">Tipe object (Class)</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="settings">Pengaturan serializer</param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonDenganSettings(Of T As Class)(
        jsonString As String,
        settings As JsonSerializerSettings) As T

        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing
        If settings Is Nothing Then Return Nothing

        Try
            Dim hasil As Object = JsonConvert.DeserializeObject(Of T)(jsonString, settings)
            Return DirectCast(hasil, T)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan strict type validation
    ''' Throw exception jika gagal (intentional - untuk critical paths)
    ''' </summary>
    ''' <typeparam name="T">Tipe object (Class)</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>Object hasil, throw jika invalid</returns>
    Public Shared Function FromJsonStrictValidation(Of T As Class)(jsonString As String) As T
        If String.IsNullOrWhiteSpace(jsonString) Then
            Throw New ArgumentNullException(
                NameOf(jsonString),
                "JSON string tidak boleh null atau kosong")
        End If

        If Not JP01_Validate.ApakahValid(jsonString) Then
            Throw New FormatException("String bukan JSON yang valid")
        End If

        Dim hasil As Object = JsonConvert.DeserializeObject(Of T)(jsonString)

        If hasil Is Nothing Then
            Throw New InvalidOperationException(
                $"Deserialisasi menghasilkan null untuk tipe {GetType(T).Name}")
        End If

        If TypeOf hasil IsNot T Then
            Throw New InvalidCastException(
                $"Hasil deserialisasi bukan tipe {GetType(T).Name}")
        End If

        Return DirectCast(hasil, T)
    End Function

    ''' <summary>
    ''' Deserialisasi List dengan strict mode
    ''' </summary>
    ''' <typeparam name="T">Tipe item (Class)</typeparam>
    ''' <param name="jsonArray">String JSON array</param>
    ''' <returns>List(Of T), list kosong jika gagal</returns>
    Public Shared Function FromJsonList(Of T As Class)(jsonArray As String) As List(Of T)
        If String.IsNullOrWhiteSpace(jsonArray) Then Return New List(Of T)()

        Try
            Dim hasil As Object = JsonConvert.DeserializeObject(Of List(Of T))(jsonArray)
            If hasil Is Nothing Then Return New List(Of T)()
            Return DirectCast(hasil, List(Of T))
        Catch
            Return New List(Of T)()
        End Try
    End Function

    ''' <summary>
    ''' Deserialisasi dengan MissingMemberHandling strict
    ''' </summary>
    ''' <typeparam name="T">Tipe object (Class)</typeparam>
    ''' <param name="jsonString">String JSON</param>
    ''' <param name="throwOnMissingMember">
    ''' True  = Throw jika ada properti tidak dikenal
    ''' False = Abaikan (default)
    ''' </param>
    ''' <returns>Object hasil</returns>
    Public Shared Function FromJsonDenganMissingMemberHandling(Of T As Class)(
        jsonString As String,
        Optional throwOnMissingMember As Boolean = False) As T

        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        Try
            Dim missingHandling As MissingMemberHandling = If(
                throwOnMissingMember,
                MissingMemberHandling.Error,
                MissingMemberHandling.Ignore)

            Dim settings = New JsonSerializerSettings With {
                .MissingMemberHandling = missingHandling
            }

            Dim hasil As Object = JsonConvert.DeserializeObject(Of T)(jsonString, settings)
            Return DirectCast(hasil, T)

        Catch ex As JsonSerializationException When throwOnMissingMember
            Throw New JsonSerializationException(
                $"Properti tidak dikenal ditemukan: {ex.Message}", ex)
        Catch
            Return Nothing
        End Try
    End Function

End Class