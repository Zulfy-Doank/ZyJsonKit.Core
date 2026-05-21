Imports System
Imports System.Collections.Generic

''' <summary>
''' PF04_Null-safe
''' Method yang aman dari null reference exception
''' Null checking dan safe navigation
''' </summary>
Public NotInheritable Class PF04_NullSafe

    ''' <summary>
    ''' Safe null check dengan Elvis operator pattern
    ''' </summary>
    Public Shared Function NullCoalesce(Of T)(obj As T, nilaiDefault As T) As T
        ' Tetap explicit untuk generic type T karena
        ' If(obj, nilaiDefault) tidak selalu aman untuk value type
        Return If(obj IsNot Nothing, obj, nilaiDefault)
    End Function

    ''' <summary>
    ''' Safe string null/empty check
    ''' </summary>
    Public Shared Function StringSafe(
        str As String,
        Optional nilaiDefault As String = "") As String

        Return If(String.IsNullOrWhiteSpace(str), nilaiDefault, str)
    End Function

    ''' <summary>
    ''' Safe execute action jika object tidak null
    ''' </summary>
    Public Shared Sub IfNotNull(Of T As Class)(
        obj As T,
        action As Action(Of T))

        If obj IsNot Nothing AndAlso action IsNot Nothing Then
            action(obj)
        End If
    End Sub

    ''' <summary>
    ''' Safe execute function jika object tidak null
    ''' </summary>
    Public Shared Function IfNotNull(Of T As Class, TResult)(
        obj As T,
        func As Func(Of T, TResult),
        Optional nilaiDefault As TResult = Nothing) As TResult

        If obj IsNot Nothing AndAlso func IsNot Nothing Then
            Return func(obj)
        End If
        Return nilaiDefault
    End Function

    ''' <summary>
    ''' Safe list access - tidak pernah return null list
    ''' </summary>
    Public Shared Function ListSafe(Of T)(list As List(Of T)) As List(Of T)
        ' ✅ IDE0029 Fix: Gunakan null-coalescing 2-parameter If()
        Return If(list, New List(Of T)())
    End Function

    ''' <summary>
    ''' Safe array access - tidak pernah return null array
    ''' </summary>
    Public Shared Function ArraySafe(Of T)(arr As T()) As T()
        ' ✅ IDE0029 Fix: Gunakan null-coalescing 2-parameter If()
        Return If(arr, Array.Empty(Of T)())
    End Function

    ''' <summary>
    ''' Safe dictionary access - tidak pernah return null dictionary
    ''' </summary>
    Public Shared Function DictionarySafe(Of TKey, TValue)(
        dict As Dictionary(Of TKey, TValue)) As Dictionary(Of TKey, TValue)

        ' ✅ IDE0029 Fix: Gunakan null-coalescing 2-parameter If()
        Return If(dict, New Dictionary(Of TKey, TValue)())
    End Function

    ''' <summary>
    ''' Safe object to string
    ''' </summary>
    Public Shared Function ToStringSafe(
        obj As Object,
        Optional nilaiDefault As String = "") As String

        Return If(obj IsNot Nothing, obj.ToString(), nilaiDefault)
    End Function

End Class