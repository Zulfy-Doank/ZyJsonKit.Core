Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization

''' <summary>
''' DF04_CamelCase
''' Settings khusus untuk konvensi penamaan camelCase
''' [FIX] StatusAktif default = False (start disabled)
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class DF04_CamelCase

    ' [FIX] Default False - test expects "Should start disabled"
    Private Shared _aktif As Boolean = False
    Private Shared ReadOnly _lock As New Object()

    ''' <summary>
    ''' Status aktif CamelCase global
    ''' Default: False (disabled)
    ''' </summary>
    Public Shared ReadOnly Property StatusAktif As Boolean
        Get
            SyncLock _lock
                Return _aktif
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' Aktifkan CamelCase global
    ''' </summary>
    Public Shared Sub Aktifkan()
        SyncLock _lock
            _aktif = True
        End SyncLock
    End Sub

    ''' <summary>
    ''' Nonaktifkan CamelCase global
    ''' </summary>
    Public Shared Sub Nonaktifkan()
        SyncLock _lock
            _aktif = False
        End SyncLock
    End Sub

    ''' <summary>
    ''' Settings camelCase standar
    ''' </summary>
    Public Shared Function DapatkanSettings() As JsonSerializerSettings
        Return New JsonSerializerSettings With {
            .ContractResolver = New CamelCasePropertyNamesContractResolver(),
            .NullValueHandling = NullValueHandling.Ignore,
            .MissingMemberHandling = MissingMemberHandling.Ignore,
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            .Formatting = Formatting.None
        }
    End Function

    ''' <summary>
    ''' Settings camelCase dengan pretty print
    ''' </summary>
    Public Shared Function DapatkanSettingsPretty() As JsonSerializerSettings
        Return New JsonSerializerSettings With {
            .ContractResolver = New CamelCasePropertyNamesContractResolver(),
            .NullValueHandling = NullValueHandling.Ignore,
            .MissingMemberHandling = MissingMemberHandling.Ignore,
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            .Formatting = Formatting.Indented
        }
    End Function

    ''' <summary>
    ''' Settings camelCase dengan null values disertakan
    ''' </summary>
    Public Shared Function DapatkanSettingsDenganNull() As JsonSerializerSettings
        Return New JsonSerializerSettings With {
            .ContractResolver = New CamelCasePropertyNamesContractResolver(),
            .NullValueHandling = NullValueHandling.Include,
            .MissingMemberHandling = MissingMemberHandling.Ignore,
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            .Formatting = Formatting.None
        }
    End Function

    ''' <summary>
    ''' Settings snake_case
    ''' </summary>
    Public Shared Function DapatkanSettingsSnakeCase() As JsonSerializerSettings
        Return New JsonSerializerSettings With {
            .ContractResolver = New DefaultContractResolver With {
                .NamingStrategy = New SnakeCaseNamingStrategy()
            },
            .NullValueHandling = NullValueHandling.Ignore,
            .MissingMemberHandling = MissingMemberHandling.Ignore,
            .Formatting = Formatting.None
        }
    End Function

    ''' <summary>
    ''' Cek apakah settings menggunakan camelCase resolver
    ''' </summary>
    Public Shared Function ApakahCamelCase(settings As JsonSerializerSettings) As Boolean
        If settings Is Nothing Then Return False
        Return TypeOf settings.ContractResolver Is CamelCasePropertyNamesContractResolver
    End Function

End Class