Imports Newtonsoft.Json

''' <summary>
''' DF03_Global
''' Manajemen settings global untuk seluruh aplikasi
'''
''' [FIX] AturFormat signature sesuai Program.vb:
'''   DF03_Global.AturFormat(False)
'''   → Parameter Boolean, bukan Formatting enum
'''   False = Formatting.None
'''   True  = Formatting.Indented
'''
''' [TS01] SyncLock - Thread-safe
''' [PF03] Aman dari exception
''' </summary>
Public NotInheritable Class DF03_Global

    Private Shared _globalSettings As JsonSerializerSettings
    Private Shared ReadOnly _lock As New Object()

    Shared Sub New()
        _globalSettings = BuatDefault()
    End Sub

    ''' <summary>
    ''' Mendapatkan settings global terkini
    ''' </summary>
    Public Shared Function DapatkanTerkini() As JsonSerializerSettings
        SyncLock _lock
            Return _globalSettings
        End SyncLock
    End Function

    ''' <summary>
    ''' Mengatur settings global baru
    ''' </summary>
    Public Shared Sub Atur(settings As JsonSerializerSettings)
        If settings Is Nothing Then Return
        SyncLock _lock
            _globalSettings = settings
        End SyncLock
    End Sub

    ''' <summary>
    ''' Reset ke pengaturan default
    ''' </summary>
    Public Shared Sub Reset()
        SyncLock _lock
            _globalSettings = BuatDefault()
        End SyncLock
    End Sub

    ''' <summary>
    ''' Alias Reset() untuk Program.vb
    ''' </summary>
    Public Shared Sub ResetKeDefault()
        Reset()
    End Sub

    ''' <summary>
    ''' Update formatting dengan Formatting enum
    ''' </summary>
    Public Shared Sub UpdateFormatting(formatting As Formatting)
        SyncLock _lock
            _globalSettings.Formatting = formatting
        End SyncLock
    End Sub

    ''' <summary>
    ''' [FIX] AturFormat - Program.vb memanggil: DF03_Global.AturFormat(False)
    '''
    ''' OVERLOAD 1: Boolean
    '''   False → Formatting.None    (compact)
    '''   True  → Formatting.Indented (pretty)
    ''' </summary>
    Public Shared Sub AturFormat(indented As Boolean)
        UpdateFormatting(If(indented, Formatting.Indented, Formatting.None))
    End Sub

    ''' <summary>
    ''' [OVERLOAD 2] AturFormat dengan Formatting enum
    ''' Untuk backward compatibility
    ''' </summary>
    Public Shared Sub AturFormat(formatting As Formatting)
        UpdateFormatting(formatting)
    End Sub

    ''' <summary>
    ''' Update null handling
    ''' </summary>
    Public Shared Sub UpdateNullHandling(handling As NullValueHandling)
        SyncLock _lock
            _globalSettings.NullValueHandling = handling
        End SyncLock
    End Sub

    ''' <summary>
    ''' Buat settings default
    ''' </summary>
    Private Shared Function BuatDefault() As JsonSerializerSettings
        Return New JsonSerializerSettings With {
            .NullValueHandling = NullValueHandling.Ignore,
            .MissingMemberHandling = MissingMemberHandling.Ignore,
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            .DateFormatHandling = DateFormatHandling.IsoDateFormat,
            .Formatting = Formatting.None
        }
    End Function

End Class