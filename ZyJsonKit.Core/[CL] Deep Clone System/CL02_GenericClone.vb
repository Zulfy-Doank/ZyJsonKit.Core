Imports Newtonsoft.Json

''' <summary>
''' C02_Generic cloning
''' Clone object generic dengan constraint class
''' [PF02] Generic-safe
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class CL02_GenericClone
    ''' <summary>
    ''' Clone object generic dengan type parameter constraint Class
    ''' </summary>
    ''' <typeparam name="T">Tipe object (harus Class)</typeparam>
    ''' <param name="obj">Object sumber</param>
    ''' <returns>Object hasil clone, Nothing jika gagal</returns>
    Public Shared Function Clone(Of T As Class)(obj As T) As T
        If obj Is Nothing Then Return Nothing

        Try
            Dim json = JsonConvert.SerializeObject(obj, Formatting.None)
            Return JsonConvert.DeserializeObject(Of T)(json)
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Clone object generic dengan factory (untuk tipe yang tidak punya parameterless constructor)
    ''' </summary>
    ''' <typeparam name="T">Tipe object (Class)</typeparam>
    ''' <param name="obj">Object sumber</param>
    ''' <param name="factory">Factory untuk membuat instance jika clone gagal</param>
    ''' <returns>Object hasil clone atau hasil factory</returns>
    Public Shared Function CloneDenganFactory(Of T As Class)(obj As T, factory As Func(Of T)) As T
        Dim hasil = Clone(obj)

        If hasil IsNot Nothing Then Return hasil
        If factory IsNot Nothing Then Return factory()

        Return Nothing
    End Function

    ''' <summary>
    ''' Clone object dan modifikasi (clone + transform)
    ''' </summary>
    ''' <typeparam name="T">Tipe object (Class)</typeparam>
    ''' <param name="obj">Object sumber</param>
    ''' <param name="transform">Action untuk memodifikasi hasil clone</param>
    ''' <returns>Object hasil clone yang sudah dimodifikasi</returns>
    Public Shared Function CloneDanModifikasi(Of T As Class)(obj As T, transform As Action(Of T)) As T
        Dim hasil = Clone(obj)

        If hasil IsNot Nothing AndAlso transform IsNot Nothing Then
            transform(hasil)
        End If

        Return hasil
    End Function

    ''' <summary>
    ''' Clone object dengan tipe berbeda (mapping)
    ''' </summary>
    ''' <typeparam name="TSource">Tipe sumber (Class)</typeparam>
    ''' <typeparam name="TTarget">Tipe target (Class)</typeparam>
    ''' <param name="source">Object sumber</param>
    ''' <returns>Object target dengan data dari sumber</returns>
    Public Shared Function CloneKeTipeLain(Of TSource As Class, TTarget As Class)(source As TSource) As TTarget
        If source Is Nothing Then Return Nothing

        Try
            Dim json = JsonConvert.SerializeObject(source)
            Return JsonConvert.DeserializeObject(Of TTarget)(json)
        Catch
            Return Nothing
        End Try
    End Function
End Class