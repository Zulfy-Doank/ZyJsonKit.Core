Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports System.Reflection

''' <summary>
''' CR01_Exclude property dynamically
''' Contract resolver untuk mengecualikan properti secara dinamis saat runtime
''' [CR03] Reflection-based filtering
''' [PF02] Generic-safe
''' </summary>
Public Class CR01_ExcludeProperty
    Inherits DefaultContractResolver

    Private ReadOnly _propertiDikecualikan As HashSet(Of String)

    ''' <summary>
    ''' Constructor dengan daftar properti yang dikecualikan
    ''' </summary>
    ''' <param name="daftarProperti">Daftar nama properti (case insensitive)</param>
    Public Sub New(ParamArray daftarProperti As String())
        _propertiDikecualikan = New HashSet(Of String)(
            If(daftarProperti, Array.Empty(Of String)()),
            StringComparer.OrdinalIgnoreCase
        )
    End Sub

    ''' <summary>
    ''' Override CreateProperty untuk filter properti
    ''' </summary>
    Protected Overrides Function CreateProperty(
        memberInfo As MemberInfo,
        memberSerialization As MemberSerialization) As JsonProperty

        Dim properti = MyBase.CreateProperty(memberInfo, memberSerialization)

        ' Exclude properti yang ada dalam daftar
        If _propertiDikecualikan.Contains(properti.PropertyName) Then
            properti.ShouldSerialize = Function(instance) False
        End If

        Return properti
    End Function

    ''' <summary>
    ''' Menambah properti yang dikecualikan saat runtime
    ''' </summary>
    Public Sub TambahExclude(namaProperti As String)
        If Not String.IsNullOrWhiteSpace(namaProperti) Then
            _propertiDikecualikan.Add(namaProperti)
        End If
    End Sub

    ''' <summary>
    ''' Menghapus properti dari daftar exclude
    ''' </summary>
    Public Sub HapusExclude(namaProperti As String)
        _propertiDikecualikan.Remove(namaProperti)
    End Sub
End Class