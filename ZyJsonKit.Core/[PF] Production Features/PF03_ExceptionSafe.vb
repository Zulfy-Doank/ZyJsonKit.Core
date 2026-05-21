Imports System

''' <summary>
''' PF03_Exception-safe
''' Method yang aman dari exception (tidak pernah throw)
''' Semua method return success flag atau default value
''' </summary>
Public NotInheritable Class PF03_ExceptionSafe
    ''' <summary>
    ''' Eksekusi action dengan exception handling
    ''' </summary>
    ''' <param name="action">Action yang akan dieksekusi</param>
    ''' <param name="pesanError">Output pesan error jika gagal</param>
    ''' <returns>True jika sukses</returns>
    Public Shared Function Eksekusi(action As Action, ByRef pesanError As String) As Boolean
        pesanError = String.Empty
        If action Is Nothing Then Return False

        Try
            action()
            Return True
        Catch ex As Exception
            pesanError = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Eksekusi function dengan exception handling
    ''' </summary>
    Public Shared Function Eksekusi(Of T)(func As Func(Of T), ByRef hasil As T, ByRef pesanError As String, Optional nilaiDefault As T = Nothing) As Boolean
        hasil = nilaiDefault
        pesanError = String.Empty
        If func Is Nothing Then Return False

        Try
            hasil = func()
            Return True
        Catch ex As Exception
            pesanError = ex.Message
            hasil = nilaiDefault
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Eksekusi action dengan silent fail (abaikan error)
    ''' </summary>
    Public Shared Sub EksekusiDiam(action As Action)
        If action Is Nothing Then Return

        Try
            action()
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Eksekusi function dengan default value fallback
    ''' </summary>
    Public Shared Function EksekusiDenganDefault(Of T)(func As Func(Of T), nilaiDefault As T) As T
        If func Is Nothing Then Return nilaiDefault

        Try
            Return func()
        Catch
            Return nilaiDefault
        End Try
    End Function

    ''' <summary>
    ''' Retry action beberapa kali jika gagal
    ''' </summary>
    Public Shared Function EksekusiDenganRetry(action As Action, Optional maxRetry As Integer = 3, Optional delayMs As Integer = 0) As Boolean
        If action Is Nothing Then Return False

        For i As Integer = 1 To maxRetry
            Try
                action()
                Return True
            Catch
                If i < maxRetry AndAlso delayMs > 0 Then
                    Threading.Thread.Sleep(delayMs)
                End If
            End Try
        Next

        Return False
    End Function
End Class