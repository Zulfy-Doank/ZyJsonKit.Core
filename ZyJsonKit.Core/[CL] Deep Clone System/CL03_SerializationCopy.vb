Imports Newtonsoft.Json

''' <summary>
''' C03_Serialization-based copy
''' Copy object menggunakan JSON serialization
''' Dengan opsi preserve references dan type handling
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class CL03_SerializationCopy
    ''' <summary>
    ''' Membuat copy object menggunakan JSON serialization dengan preserve references
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="sumber">Object sumber</param>
    ''' <returns>Object hasil copy</returns>
    Public Shared Function Salin(Of T)(sumber As T) As T
        If sumber Is Nothing Then Return Nothing

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                .ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                .TypeNameHandling = TypeNameHandling.Auto
            }
            Dim json = JsonConvert.SerializeObject(sumber, pengaturan)
            Return JsonConvert.DeserializeObject(Of T)(json, pengaturan)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Copy object dengan type information (polymorphic)
    ''' </summary>
    ''' <typeparam name="T">Tipe base</typeparam>
    ''' <param name="sumber">Object sumber (bisa subclass)</param>
    ''' <returns>Object hasil copy dengan tipe yang sama</returns>
    Public Shared Function SalinDenganTipe(Of T)(sumber As T) As T
        If sumber Is Nothing Then Return Nothing

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .TypeNameHandling = TypeNameHandling.All,
                .PreserveReferencesHandling = PreserveReferencesHandling.Objects
            }
            Dim json = JsonConvert.SerializeObject(sumber, pengaturan)
            Return JsonConvert.DeserializeObject(Of T)(json, pengaturan)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Copy object dengan mengabaikan properti null
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="sumber">Object sumber</param>
    ''' <returns>Object hasil copy (properti null diabaikan)</returns>
    Public Shared Function SalinTanpaNull(Of T)(sumber As T) As T
        If sumber Is Nothing Then Return Nothing

        Try
            Dim pengaturan = New JsonSerializerSettings With {
                .NullValueHandling = NullValueHandling.Ignore
            }
            Dim json = JsonConvert.SerializeObject(sumber, pengaturan)
            Return JsonConvert.DeserializeObject(Of T)(json, pengaturan)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Copy object dengan formatting compact (untuk performa)
    ''' </summary>
    ''' <typeparam name="T">Tipe object</typeparam>
    ''' <param name="sumber">Object sumber</param>
    ''' <returns>Object hasil copy</returns>
    Public Shared Function SalinCepat(Of T)(sumber As T) As T
        If sumber Is Nothing Then Return Nothing

        Try
            Dim json = JsonConvert.SerializeObject(sumber, Formatting.None)
            Return JsonConvert.DeserializeObject(Of T)(json)
        Catch
            Return Nothing
        End Try
    End Function
End Class