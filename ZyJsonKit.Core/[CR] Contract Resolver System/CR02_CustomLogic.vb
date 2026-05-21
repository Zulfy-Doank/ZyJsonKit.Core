Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports System.Reflection

''' <summary>
''' CR02_Custom serialization logic
''' Contract resolver dengan custom logic untuk serialisasi
''' [AF05] Extensible - Bisa dikustomisasi lebih lanjut
''' </summary>
Public Class CR02_CustomLogic
    Inherits DefaultContractResolver

    Private ReadOnly _filterCallback As Func(Of String, Boolean)
    Private ReadOnly _renameCallback As Func(Of String, String)

    ''' <summary>
    ''' Constructor dengan filter dan rename callback
    ''' </summary>
    ''' <param name="filterCallback">Callback filter: return True untuk serialize</param>
    ''' <param name="renameCallback">Callback rename properti (opsional)</param>
    Public Sub New(
        Optional filterCallback As Func(Of String, Boolean) = Nothing,
        Optional renameCallback As Func(Of String, String) = Nothing)
        _filterCallback = filterCallback
        _renameCallback = renameCallback
    End Sub

    ''' <summary>
    ''' Override CreateProperty dengan custom logic
    ''' </summary>
    Protected Overrides Function CreateProperty(
        memberInfo As MemberInfo,
        memberSerialization As MemberSerialization) As JsonProperty

        Dim properti = MyBase.CreateProperty(memberInfo, memberSerialization)

        ' Terapkan filter callback
        If _filterCallback IsNot Nothing Then
            properti.ShouldSerialize = Function(instance) _filterCallback(properti.PropertyName)
        End If

        ' Terapkan rename callback
        If _renameCallback IsNot Nothing Then
            properti.PropertyName = _renameCallback(properti.PropertyName)
        End If

        Return properti
    End Function
End Class