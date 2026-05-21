Imports Newtonsoft.Json.Linq

''' <summary>
''' JP05_Optimized
''' Optimasi parsing JSON dengan caching untuk performa tinggi
''' Cocok untuk skenario parsing berulang dengan JSON yang sama
'''
''' PERBAIKAN BUG:
''' [BUG01] Cache korupsi: pisahkan cache key per tipe
'''         SEBELUM: 1 _jsonCache untuk 3 tipe → collision
'''         SESUDAH: _jsonCacheToken/_jsonCacheObject/_jsonCacheArray terpisah
''' [BUG02] Cross-contamination: ParseJObject tidak override _tokenCache
'''
''' [TS01] SyncLock - Thread-safe cache
''' [PF06] Memory optimized - Cache terkontrol
''' </summary>
Public NotInheritable Class JP05_Optimized

    ' Cache JToken (generic)
    Private Shared _tokenCache As JToken
    Private Shared _jsonCacheToken As String

    ' Cache JObject (independen)
    Private Shared _jObjectCache As JObject
    Private Shared _jsonCacheObject As String

    ' Cache JArray (independen)
    Private Shared _jArrayCache As JArray
    Private Shared _jsonCacheArray As String

    ' Lock object
    Private Shared ReadOnly _syncLock As New Object()

    ' Flag cache
    Private Shared _cacheEnabled As Boolean = True

    ''' <summary>
    ''' Parse JSON dengan cache optimization
    ''' </summary>
    ''' <param name="jsonString">String JSON</param>
    ''' <returns>JToken hasil parse</returns>
    Public Shared Function Parse(jsonString As String) As JToken
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        If Not _cacheEnabled Then
            Try
                Return JToken.Parse(jsonString)
            Catch
                Return Nothing
            End Try
        End If

        SyncLock _syncLock
            If _jsonCacheToken = jsonString AndAlso _tokenCache IsNot Nothing Then
                Return _tokenCache
            End If

            Try
                _jsonCacheToken = jsonString
                _tokenCache = JToken.Parse(jsonString)
                Return _tokenCache
            Catch
                _jsonCacheToken = Nothing
                Return Nothing
            End Try
        End SyncLock
    End Function

    ''' <summary>
    ''' Parse ke JObject dengan cache independen
    ''' [BUG01 FIX] _jsonCacheObject terpisah
    ''' [BUG02 FIX] Tidak override _tokenCache
    ''' </summary>
    ''' <param name="jsonString">String JSON object</param>
    ''' <returns>JObject, Nothing jika gagal</returns>
    Public Shared Function ParseJObject(jsonString As String) As JObject
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        If Not _cacheEnabled Then
            Try
                Return JObject.Parse(jsonString)
            Catch
                Return Nothing
            End Try
        End If

        SyncLock _syncLock
            If _jsonCacheObject = jsonString AndAlso _jObjectCache IsNot Nothing Then
                Return _jObjectCache
            End If

            Try
                _jsonCacheObject = jsonString
                _jObjectCache = JObject.Parse(jsonString)
                Return _jObjectCache
            Catch
                _jsonCacheObject = Nothing
                Return Nothing
            End Try
        End SyncLock
    End Function

    ''' <summary>
    ''' Parse ke JArray dengan cache independen
    ''' [BUG01 FIX] _jsonCacheArray terpisah
    ''' [BUG02 FIX] Tidak override _tokenCache
    ''' </summary>
    ''' <param name="jsonString">String JSON array</param>
    ''' <returns>JArray, Nothing jika gagal</returns>
    Public Shared Function ParseJArray(jsonString As String) As JArray
        If String.IsNullOrWhiteSpace(jsonString) Then Return Nothing

        If Not _cacheEnabled Then
            Try
                Return JArray.Parse(jsonString)
            Catch
                Return Nothing
            End Try
        End If

        SyncLock _syncLock
            If _jsonCacheArray = jsonString AndAlso _jArrayCache IsNot Nothing Then
                Return _jArrayCache
            End If

            Try
                _jsonCacheArray = jsonString
                _jArrayCache = JArray.Parse(jsonString)
                Return _jArrayCache
            Catch
                _jsonCacheArray = Nothing
                Return Nothing
            End Try
        End SyncLock
    End Function

    ''' <summary>
    ''' Membersihkan semua cache
    ''' </summary>
    Public Shared Sub BersihkanCache()
        SyncLock _syncLock
            _tokenCache = Nothing
            _jObjectCache = Nothing
            _jArrayCache = Nothing
            _jsonCacheToken = Nothing
            _jsonCacheObject = Nothing
            _jsonCacheArray = Nothing
        End SyncLock
    End Sub

    ''' <summary>
    ''' Membersihkan cache jika JSON berubah
    ''' </summary>
    ''' <param name="jsonString">JSON baru untuk dibandingkan</param>
    ''' <returns>True jika cache dibersihkan</returns>
    Public Shared Function BersihkanJikaBerubah(jsonString As String) As Boolean
        SyncLock _syncLock
            Dim adaYangBerubah = (
                _jsonCacheToken <> jsonString OrElse
                _jsonCacheObject <> jsonString OrElse
                _jsonCacheArray <> jsonString
            )
            If adaYangBerubah Then
                BersihkanCache()
                Return True
            End If
            Return False
        End SyncLock
    End Function

    ''' <summary>
    ''' Mengaktifkan/menonaktifkan cache
    ''' </summary>
    ''' <param name="aktif">True untuk aktifkan cache</param>
    Public Shared Sub SetCacheEnabled(aktif As Boolean)
        SyncLock _syncLock
            _cacheEnabled = aktif
            If Not aktif Then BersihkanCache()
        End SyncLock
    End Sub

    ''' <summary>
    ''' Status cache aktif/nonaktif
    ''' </summary>
    Public Shared ReadOnly Property CacheAktif As Boolean
        Get
            SyncLock _syncLock
                Return _cacheEnabled
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' Jumlah cache yang tersimpan
    ''' </summary>
    Public Shared ReadOnly Property JumlahCache As Integer
        Get
            SyncLock _syncLock
                Dim count As Integer = 0
                If _tokenCache IsNot Nothing Then count += 1
                If _jObjectCache IsNot Nothing Then count += 1
                If _jArrayCache IsNot Nothing Then count += 1
                Return count
            End SyncLock
        End Get
    End Property

End Class