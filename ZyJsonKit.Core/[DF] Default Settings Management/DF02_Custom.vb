Imports Newtonsoft.Json

''' <summary>
''' DF02_Custom
''' Builder pattern untuk membuat JsonSerializerSettings kustom
'''
''' [FIX] BuatPengaturan signature disesuaikan dengan Program.vb:
'''   DF02_Custom.BuatPengaturan(
'''       NullValueHandling.Include,
'''       Formatting.None,
'''       True)
''' Parameter: (NullHandling, Formatting, IncludeDefaults)
'''
''' [PF03] Aman dari exception
''' [PF04] Aman dari null reference
''' </summary>
Public NotInheritable Class DF02_Custom

    ''' <summary>
    ''' Membuat settings dengan konfigurasi lengkap
    ''' Original method - parameter bernama
    ''' </summary>
    Public Shared Function Buat(
        Optional formatting As Formatting = Formatting.None,
        Optional nullHandling As NullValueHandling = NullValueHandling.Ignore,
        Optional missingMember As MissingMemberHandling = MissingMemberHandling.Ignore,
        Optional referenceLoop As ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        ) As JsonSerializerSettings

        Return New JsonSerializerSettings With {
            .Formatting = formatting,
            .NullValueHandling = nullHandling,
            .MissingMemberHandling = missingMember,
            .ReferenceLoopHandling = referenceLoop,
            .DateFormatHandling = DateFormatHandling.IsoDateFormat
        }
    End Function

    ''' <summary>
    ''' [FIX] BuatPengaturan - signature sesuai Program.vb:
    '''   BuatPengaturan(NullValueHandling.Include, Formatting.None, True)
    '''
    ''' Parameter:
    '''   nullHandling     = NullValueHandling (arg 1)
    '''   formatting       = Formatting        (arg 2)
    '''   includeDefaults  = Boolean           (arg 3, opsional)
    ''' </summary>
    Public Shared Function BuatPengaturan(
        nullHandling As NullValueHandling,
        formatting As Formatting,
        Optional includeDefaults As Boolean = True
        ) As JsonSerializerSettings

        Return New JsonSerializerSettings With {
            .NullValueHandling = nullHandling,
            .Formatting = formatting,
            .MissingMemberHandling = If(includeDefaults,
                MissingMemberHandling.Ignore,
                MissingMemberHandling.Error),
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            .DateFormatHandling = DateFormatHandling.IsoDateFormat
        }
    End Function

    ''' <summary>
    ''' Settings untuk output pretty print
    ''' </summary>
    Public Shared Function BuatPretty() As JsonSerializerSettings
        Return Buat(formatting:=Formatting.Indented)
    End Function

    ''' <summary>
    ''' Settings untuk output compact
    ''' </summary>
    Public Shared Function BuatCompact() As JsonSerializerSettings
        Return Buat(formatting:=Formatting.None)
    End Function

    ''' <summary>
    ''' Settings yang menyertakan nilai null
    ''' </summary>
    Public Shared Function BuatDenganNull() As JsonSerializerSettings
        Return Buat(nullHandling:=NullValueHandling.Include)
    End Function

    ''' <summary>
    ''' Settings strict
    ''' </summary>
    Public Shared Function BuatStrict() As JsonSerializerSettings
        Return Buat(missingMember:=MissingMemberHandling.Error)
    End Function

    ''' <summary>
    ''' Clone settings yang ada
    ''' </summary>
    Public Shared Function Clone(asalSettings As JsonSerializerSettings) As JsonSerializerSettings
        If asalSettings Is Nothing Then Return Buat()

        Return New JsonSerializerSettings With {
            .Formatting = asalSettings.Formatting,
            .NullValueHandling = asalSettings.NullValueHandling,
            .MissingMemberHandling = asalSettings.MissingMemberHandling,
            .ReferenceLoopHandling = asalSettings.ReferenceLoopHandling,
            .DateFormatHandling = asalSettings.DateFormatHandling,
            .ContractResolver = asalSettings.ContractResolver,
            .Converters = asalSettings.Converters
        }
    End Function

End Class