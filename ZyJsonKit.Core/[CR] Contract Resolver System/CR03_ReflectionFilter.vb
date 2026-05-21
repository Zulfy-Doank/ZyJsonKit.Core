Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports System.Reflection

''' <summary>
''' CR03_Reflection-based filtering
''' Filtering properti via reflection (tipe, attribute, dll)
''' [AF05] Extensible
''' </summary>
Public Class CR03_ReflectionFilter
    Inherits DefaultContractResolver

    Private ReadOnly _tipeDiizinkan As HashSet(Of Type)
    Private ReadOnly _hanyaBaca As Boolean

    ''' <summary>
    ''' Constructor dengan filter tipe
    ''' </summary>
    ''' <param name="tipeDiizinkan">Tipe properti yang diizinkan untuk diserialize</param>
    ''' <param name="hanyaBaca">Jika True, hanya serialize properti readonly</param>
    Public Sub New(Optional tipeDiizinkan As Type() = Nothing, Optional hanyaBaca As Boolean = False)
        _tipeDiizinkan = New HashSet(Of Type)(If(tipeDiizinkan, Array.Empty(Of Type)()))
        _hanyaBaca = hanyaBaca
    End Sub

    ''' <summary>
    ''' Override CreateProperty dengan filter reflection
    ''' </summary>
    Protected Overrides Function CreateProperty(
        memberInfo As MemberInfo,
        memberSerialization As MemberSerialization) As JsonProperty

        Dim properti = MyBase.CreateProperty(memberInfo, memberSerialization)

        ' Filter berdasarkan tipe
        If _tipeDiizinkan.Count > 0 Then
            Dim tipeProperti = GetPropertyType(memberInfo)
            If tipeProperti IsNot Nothing AndAlso Not _tipeDiizinkan.Contains(tipeProperti) Then
                properti.ShouldSerialize = Function(instance) False
            End If
        End If

        ' Filter hanya readonly
        If _hanyaBaca Then
            Dim propInfo = TryCast(memberInfo, PropertyInfo)
            If propInfo IsNot Nothing AndAlso propInfo.CanWrite Then
                properti.ShouldSerialize = Function(instance) False
            End If
        End If

        Return properti
    End Function

    ''' <summary>
    ''' Mendapatkan tipe properti dari MemberInfo
    ''' </summary>
    Private Shared Function GetPropertyType(memberInfo As MemberInfo) As Type
        If TypeOf memberInfo Is PropertyInfo Then
            Return DirectCast(memberInfo, PropertyInfo).PropertyType
        End If

        If TypeOf memberInfo Is FieldInfo Then
            Return DirectCast(memberInfo, FieldInfo).FieldType
        End If

        Return Nothing
    End Function
End Class