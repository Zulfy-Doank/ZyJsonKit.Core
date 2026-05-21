''' <summary>
''' AF03_Modular region structure
''' Struktur region modular untuk organisasi kode
''' Setiap region memiliki class terpisah dengan tanggung jawab jelas
''' </summary>
Public NotInheritable Class AF03_ModularRegion
    ''' <summary>
    ''' Mendapatkan struktur modul framework
    ''' </summary>
    Public Shared Function StrukturModul() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"[D]", "Default Settings Management - Pengaturan serializer global"},
            {"[J]", "JSON Validation & Parsing - Validasi & parsing JSON"},
            {"[S]", "Serialization - Convert object ke JSON"},
            {"[DS]", "Deserialization - Convert JSON ke object"},
            {"[F]", "File Operations - Operasi file JSON"},
            {"[JO]", "JObject/JToken Operations - Manipulasi token JSON"},
            {"[DI]", "Dictionary Conversion - JSON ke Dictionary"},
            {"[L]", "List & Array Operations - Operasi collection JSON"},
            {"[FM]", "JSON Formatting - Formatting tampilan JSON"},
            {"[C]", "Deep Clone System - Clone object mendalam"},
            {"[DY]", "Dynamic Object Support - Support dynamic object"},
            {"[CR]", "Contract Resolver System - Custom serialization resolver"},
            {"[TS]", "Thread Safety - Sistem thread-safe"},
            {"[PF]", "Production Features - Feature production-ready"},
            {"[AF]", "Architecture Features - Struktur arsitektur modern"}
        }
    End Function

    ''' <summary>
    ''' Mendapatkan jumlah class per modul
    ''' </summary>
    Public Shared Function JumlahClassPerModul() As Dictionary(Of String, Integer)
        Return New Dictionary(Of String, Integer) From {
            {"[D]", 4},
            {"[J]", 5},
            {"[S]", 5},
            {"[DS]", 6},
            {"[F]", 6},
            {"[JO]", 6},
            {"[DI]", 4},
            {"[L]", 4},
            {"[FM]", 3},
            {"[C]", 3},
            {"[DY]", 3},
            {"[CR]", 3},
            {"[TS]", 3},
            {"[PF]", 6},
            {"[AF]", 5}
        }
    End Function
End Class