''' <summary>
''' AF04_Enterprise naming
''' Standar penamaan enterprise untuk konsistensi
''' Prefix convention untuk setiap modul
''' </summary>
Public NotInheritable Class AF04_EnterpriseNaming
    ''' <summary>
    ''' Dokumentasi prefix convention
    ''' </summary>
    Public Shared Function PrefixConvention() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"D", "Default Settings - Pengaturan default"},
            {"J", "JSON Validation - Validasi JSON"},
            {"S", "Serialization - Serialisasi"},
            {"DS", "Deserialization - Deserialisasi"},
            {"F", "File Operations - Operasi file"},
            {"JO", "JObject Operations - Operasi JObject"},
            {"DI", "Dictionary Conversion - Konversi Dictionary"},
            {"L", "List & Array - List dan Array"},
            {"FM", "JSON Formatting - Format JSON"},
            {"C", "Clone System - Sistem clone"},
            {"DY", "Dynamic Objects - Object dinamis"},
            {"CR", "Contract Resolver - Resolver kontrak"},
            {"TS", "Thread Safety - Keamanan thread"},
            {"PF", "Production Features - Fitur produksi"},
            {"AF", "Architecture Features - Fitur arsitektur"}
        }
    End Function

    ''' <summary>
    ''' Mendapatkan contoh nama class per modul
    ''' </summary>
    Public Shared Function ContohNamaClass() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"D01", "D01_Thread_safe - Pengaturan thread-safe"},
            {"J01", "J01_Validate - Validasi JSON"},
            {"S01", "S01_Serialize - Serialisasi object"},
            {"DS01", "DS01_Deserialize - Deserialisasi JSON"},
            {"F01", "F01_Save - Simpan ke file"},
            {"JO01", "JO01_GetValue - Ambil value via path"},
            {"DI01", "DI01_ToDictionary - Konversi ke Dictionary"},
            {"L01", "L01_SerializeList - Serialisasi List"},
            {"FM01", "FM01_PrettyPrint - Format rapi JSON"},
            {"C01", "C01_DeepClone - Clone mendalam"},
            {"DY01", "DY01_DynamicDeserialize - Deserialize dynamic"},
            {"CR01", "CR01_ExcludeProperty - Exclude properti"},
            {"TS01", "TS01_SyncLock - Lock shared resource"},
            {"PF01", "PF01_OptionStrictOn - Strict mode compatible"},
            {"AF01", "AF01_GenericMethods - Method generic reusable"}
        }
    End Function
End Class