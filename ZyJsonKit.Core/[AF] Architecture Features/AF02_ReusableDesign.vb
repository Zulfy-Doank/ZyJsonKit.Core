''' <summary>
''' AF02_Reusable design
''' Desain yang bisa digunakan ulang di berbagai project
''' Semua method static, tidak bergantung pada state
''' </summary>
Public NotInheritable Class AF02_ReusableDesign
    ''' <summary>
    ''' Informasi versi framework
    ''' </summary>
    Public Shared Function VersiFramework() As String
        Return "ZyJsonKit.Core v1.0.0 - Enterprise Edition"
    End Function

    ''' <summary>
    ''' Informasi author
    ''' </summary>
    Public Shared Function Author() As String
        Return "ZyJsonKit Team"
    End Function

    ''' <summary>
    ''' Daftar dependency
    ''' </summary>
    Public Shared Function DaftarDependency() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"Newtonsoft.Json", "13.0.0+"},
            {"System", "4.0.0+"},
            {"System.Core", "4.0.0+"}
        }
    End Function

    ''' <summary>
    ''' Daftar fitur yang tersedia
    ''' </summary>
    Public Shared Function DaftarFitur() As List(Of String)
        Return New List(Of String) From {
            "[D] Default Settings Management",
            "[J] JSON Validation & Parsing",
            "[S] Serialization",
            "[DS] Deserialization",
            "[F] File Operations",
            "[JO] JObject / JToken Operations",
            "[DI] Dictionary Conversion",
            "[L] List & Array Operations",
            "[FM] JSON Formatting",
            "[C] Deep Clone System",
            "[DY] Dynamic Object Support",
            "[CR] Contract Resolver System",
            "[TS] Thread Safety",
            "[PF] Production Features",
            "[AF] Architecture Features"
        }
    End Function
End Class