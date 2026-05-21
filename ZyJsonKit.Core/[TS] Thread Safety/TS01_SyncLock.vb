Imports Newtonsoft.Json
Imports System.Threading

''' <summary>
''' TS01_SyncLock settings
''' Implementasi SyncLock untuk melindungi shared settings
''' [TS02] Proteksi shared resource
''' [PF03] Exception-safe
''' </summary>
Public NotInheritable Class TS01_SyncLock
    Private Shared ReadOnly _syncLock As New Object()
    Private Shared _sharedSettings As JsonSerializerSettings

    ''' <summary>
    ''' Static constructor
    ''' </summary>
    Shared Sub New()
        _sharedSettings = New JsonSerializerSettings()
    End Sub

    ''' <summary>
    ''' Mendapatkan shared settings dengan lock
    ''' </summary>
    Public Shared Function DapatkanShared() As JsonSerializerSettings
        SyncLock _syncLock
            Return _sharedSettings
        End SyncLock
    End Function

    ''' <summary>
    ''' Mengatur shared settings dengan lock
    ''' </summary>
    Public Shared Sub AturShared(settings As JsonSerializerSettings)
        SyncLock _syncLock
            If settings IsNot Nothing Then
                _sharedSettings = settings
            End If
        End SyncLock
    End Sub

    ''' <summary>
    ''' Eksekusi action dengan lock
    ''' </summary>
    Public Shared Sub EksekusiDenganLock(action As Action)
        If action Is Nothing Then Return

        SyncLock _syncLock
            action()
        End SyncLock
    End Sub

    ''' <summary>
    ''' Eksekusi function dengan lock dan return value
    ''' </summary>
    Public Shared Function EksekusiDenganLock(Of T)(func As Func(Of T)) As T
        If func Is Nothing Then Return Nothing

        SyncLock _syncLock
            Return func()
        End SyncLock
    End Function
End Class