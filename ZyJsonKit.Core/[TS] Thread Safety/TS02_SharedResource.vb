Imports System.Threading

''' <summary>
''' TS02_Shared resource protection
''' Proteksi resource yang dishare antar thread
''' [TS01] SyncLock - Mengunci akses
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class TS02_SharedResource
    Private Shared ReadOnly _resourceLock As New Object()
    Private Shared ReadOnly _sharedResources As New Dictionary(Of String, Object)()

    ''' <summary>
    ''' Mendapatkan shared resource dengan thread-safe
    ''' </summary>
    Public Shared Function DapatkanResource(kunci As String) As Object
        If String.IsNullOrWhiteSpace(kunci) Then Return Nothing

        SyncLock _resourceLock
            If _sharedResources.ContainsKey(kunci) Then
                Return _sharedResources(kunci)
            End If
            Return Nothing
        End SyncLock
    End Function

    ''' <summary>
    ''' Mendapatkan shared resource dengan tipe spesifik
    ''' </summary>
    Public Shared Function DapatkanResource(Of T)(kunci As String, Optional nilaiDefault As T = Nothing) As T
        Dim resource = DapatkanResource(kunci)
        If resource IsNot Nothing AndAlso TypeOf resource Is T Then
            Return DirectCast(resource, T)
        End If
        Return nilaiDefault
    End Function

    ''' <summary>
    ''' Menyimpan shared resource dengan thread-safe
    ''' </summary>
    Public Shared Sub SimpanResource(kunci As String, resource As Object)
        If String.IsNullOrWhiteSpace(kunci) Then Return

        SyncLock _resourceLock
            _sharedResources(kunci) = resource
        End SyncLock
    End Sub

    ''' <summary>
    ''' Menghapus shared resource
    ''' </summary>
    Public Shared Function HapusResource(kunci As String) As Boolean
        SyncLock _resourceLock
            Return _sharedResources.Remove(kunci)
        End SyncLock
    End Function

    ''' <summary>
    ''' Membersihkan semua shared resource
    ''' </summary>
    Public Shared Sub BersihkanSemua()
        SyncLock _resourceLock
            _sharedResources.Clear()
        End SyncLock
    End Sub

    ''' <summary>
    ''' Mengecek apakah resource exists
    ''' </summary>
    Public Shared Function ResourceAda(kunci As String) As Boolean
        SyncLock _resourceLock
            Return _sharedResources.ContainsKey(kunci)
        End SyncLock
    End Function
End Class