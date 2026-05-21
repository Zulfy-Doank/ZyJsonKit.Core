Imports System.Threading

''' <summary>
''' TS03_Static constructor initialization
''' Inisialisasi static constructor yang thread-safe
''' </summary>
Public NotInheritable Class TS03_StaticInit
    Private Shared ReadOnly _syncLock As New Object()
    Private Shared _isInitialized As Boolean = False
    Private Shared _initCount As Integer = 0

    ' Static constructor - AUTO INIT
    Shared Sub New()
        _isInitialized = True
        _initCount = 1
    End Sub

    ''' <summary>
    ''' Inisialisasi thread-safe dengan double-check locking
    ''' </summary>
    Public Shared Function Inisialisasi(initAction As Action) As Boolean
        If initAction Is Nothing Then Return False

        If Not _isInitialized Then
            SyncLock _syncLock
                If Not _isInitialized Then
                    Try
                        initAction()
                        _isInitialized = True
                        _initCount += 1
                        Return True
                    Catch
                        Return False
                    End Try
                End If
            End SyncLock
        End If

        Return True
    End Function

    Public Shared Sub Reset()
        SyncLock _syncLock
            _isInitialized = False
        End SyncLock
    End Sub

    Public Shared ReadOnly Property SudahInisialisasi As Boolean
        Get
            SyncLock _syncLock
                Return _isInitialized
            End SyncLock
        End Get
    End Property

    Public Shared ReadOnly Property JumlahInisialisasi As Integer
        Get
            SyncLock _syncLock
                Return _initCount
            End SyncLock
        End Get
    End Property
End Class