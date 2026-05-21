Imports Newtonsoft.Json

''' <summary>
''' DF01_Settings
''' Wrapper JsonSerializerSettings dengan instance methods
'''
''' Program.vb usage:
'''   Dim df01 = DF01_ThreadSafe.Instance
'''   Dim settings = df01.GetSettings()
'''   df01.IsInitialized
'''   df01.SetFormatting(False)   ← Boolean: False=None, True=Indented
'''   df01.ResetToDefault()
'''
''' [TS01] SyncLock - Thread-safe
''' [PF03] Aman dari exception
''' </summary>
Public NotInheritable Class DF01_Settings
    Inherits JsonSerializerSettings

    Private ReadOnly _diinisialisasi As Boolean = False
    Private ReadOnly _syncLock As New Object()

    Public Sub New()
        ApplyDefault()
        _diinisialisasi = True
    End Sub

    ''' <summary>
    ''' GetSettings - return self (instance IS the settings)
    ''' </summary>
    Public Function GetSettings() As JsonSerializerSettings
        SyncLock _syncLock
            Return Me
        End SyncLock
    End Function

    ''' <summary>
    ''' IsInitialized property
    ''' </summary>
    Public ReadOnly Property IsInitialized As Boolean
        Get
            SyncLock _syncLock
                Return _diinisialisasi
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' [FIX] SetFormatting(Boolean)
    ''' Program.vb: df01.SetFormatting(False)
    '''   False → Formatting.None
    '''   True  → Formatting.Indented
    ''' </summary>
    Public Sub SetFormatting(indented As Boolean)
        SyncLock _syncLock
            Me.Formatting = If(indented, Formatting.Indented, Formatting.None)
        End SyncLock
    End Sub

    ''' <summary>
    ''' SetFormatting dengan Formatting enum (overload)
    ''' </summary>
    Public Sub SetFormatting(formatting As Formatting)
        SyncLock _syncLock
            Me.Formatting = formatting
        End SyncLock
    End Sub

    ''' <summary>
    ''' ResetToDefault - kembalikan ke nilai default
    ''' </summary>
    Public Sub ResetToDefault()
        SyncLock _syncLock
            ApplyDefault()
        End SyncLock
    End Sub

    ''' <summary>
    ''' SetNullHandling helper
    ''' </summary>
    Public Sub SetNullHandling(handling As NullValueHandling)
        SyncLock _syncLock
            Me.NullValueHandling = handling
        End SyncLock
    End Sub

    ''' <summary>
    ''' Apply nilai default
    ''' </summary>
    Private Sub ApplyDefault()
        Me.NullValueHandling = NullValueHandling.Ignore
        Me.MissingMemberHandling = MissingMemberHandling.Ignore
        Me.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Me.DateFormatHandling = DateFormatHandling.IsoDateFormat
        Me.Formatting = Formatting.None
    End Sub

End Class

''' <summary>
''' DF01_ThreadSafe
''' Singleton provider untuk DF01_Settings
''' [TS01] SyncLock - Double-check locking
''' </summary>
Public NotInheritable Class DF01_ThreadSafe

    Private Shared _instance As DF01_Settings
    Private Shared ReadOnly _syncLock As New Object()

    ''' <summary>
    ''' Singleton Instance - lazy init thread-safe
    ''' </summary>
    Public Shared ReadOnly Property Instance As DF01_Settings
        Get
            If _instance Is Nothing Then
                SyncLock _syncLock
                    If _instance Is Nothing Then
                        _instance = New DF01_Settings()
                    End If
                End SyncLock
            End If
            Return _instance
        End Get
    End Property

    ''' <summary>
    ''' Alias untuk Instance
    ''' </summary>
    Public Shared Function DapatkanSettings() As DF01_Settings
        Return Instance
    End Function

    ''' <summary>
    ''' Reset singleton
    ''' </summary>
    Public Shared Sub Reset()
        SyncLock _syncLock
            _instance = Nothing
        End SyncLock
    End Sub

    ''' <summary>
    ''' Cek status inisialisasi
    ''' </summary>
    Public Shared ReadOnly Property SudahDiinisialisasi As Boolean
        Get
            SyncLock _syncLock
                Return _instance IsNot Nothing AndAlso _instance.IsInitialized
            End SyncLock
        End Get
    End Property

End Class