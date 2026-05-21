Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports ZyJsonKit.Core

Module Program

    ' Statistics tracking
    Private _totalTests As Integer = 0
    Private _passedTests As Integer = 0
    Private _failedTests As Integer = 0
    Private _totalTime As Long = 0
    Private _startMemory As Long = 0
    Private ReadOnly _testResults As New List(Of TestResult)()

    Sub Main()
        Console.Title = "ZyJsonKit.Core - Enterprise Test Suite"
        Console.OutputEncoding = Text.Encoding.UTF8

        _startMemory = GC.GetTotalMemory(True)

        PrintHeader()

        ' Run all test sections
        RunTestSection("[DF] DEFAULT SETTINGS", AddressOf Test_DF_Settings)
        RunTestSection("[JP] JSON PARSING", AddressOf Test_JP_Parsing)
        RunTestSection("[SR] SERIALIZATION", AddressOf Test_SR_Serialization)
        RunTestSection("[DS] DESERIALIZATION", AddressOf Test_DS_Deserialization)
        RunTestSection("[FO] FILE OPERATIONS", AddressOf Test_FO_FileOperations)
        RunTestSection("[JT] JOBJECT/JTOKEN", AddressOf Test_JT_Operations)
        RunTestSection("[DC] DICTIONARY CONVERSION", AddressOf Test_DC_Dictionary)
        RunTestSection("[LA] LIST & ARRAY", AddressOf Test_LA_ListArray)
        RunTestSection("[FM] FORMATTING", AddressOf Test_FM_Formatting)
        RunTestSection("[CL] CLONE SYSTEM", AddressOf Test_CL_Clone)
        RunTestSection("[DY] DYNAMIC OBJECTS", AddressOf Test_DY_Dynamic)
        RunTestSection("[CR] CONTRACT RESOLVER", AddressOf Test_CR_Resolver)
        RunTestSection("[TS] THREAD SAFETY", AddressOf Test_TS_ThreadSafety)
        RunTestSection("[PF] PRODUCTION FEATURES", AddressOf Test_PF_Production)
        RunTestSection("[AF] ARCHITECTURE", AddressOf Test_AF_Architecture)

        PrintSummary()
        Console.ReadLine()
    End Sub

#Region "Test Infrastructure"

    ''' <summary>
    ''' Run a test section with timing and error handling
    ''' </summary>
    Sub RunTestSection(sectionName As String, testAction As Action)
        Console.WriteLine()
        Console.WriteLine($"════════════════════════════════════════")
        Console.WriteLine($"  {sectionName}")
        Console.WriteLine($"════════════════════════════════════════")

        Dim sw = Stopwatch.StartNew()
        Try
            testAction()
            sw.Stop()
            Console.WriteLine($"  ⏱  Section Time: {sw.ElapsedMilliseconds} ms")
        Catch ex As Exception
            sw.Stop()
            Console.WriteLine($"  ❌ SECTION ERROR: {ex.Message}")
            _failedTests += 1
        End Try
    End Sub

    ''' <summary>
    ''' Run a single test with pass/fail tracking
    ''' </summary>
    Sub RunTest(testName As String, testAction As Action)
        _totalTests += 1
        Dim sw = Stopwatch.StartNew()
        Try
            testAction()
            sw.Stop()
            _passedTests += 1
            _totalTime += sw.ElapsedMilliseconds
            _testResults.Add(New TestResult With {
                .Name = testName,
                .Passed = True,
                .TimeMs = sw.ElapsedMilliseconds
            })
            Console.WriteLine($"  ✅ {testName} ({sw.ElapsedMilliseconds} ms)")
        Catch ex As Exception
            sw.Stop()
            _failedTests += 1
            _testResults.Add(New TestResult With {
                .Name = testName,
                .Passed = False,
                .TimeMs = sw.ElapsedMilliseconds,
                .ErrorMessage = ex.Message
            })
            Console.WriteLine($"  ❌ {testName} - {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Print header
    ''' </summary>
    Sub PrintHeader()
        Console.WriteLine("══════════════════════════════════════════════")
        Console.WriteLine("  ZyJsonKit.Core - Enterprise JSON Test Suite")
        Console.WriteLine($"  Version: {AF02_ReusableDesign.VersiFramework()}")
        Console.WriteLine($"  DateTime: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
        Console.WriteLine($"  Machine: {Environment.MachineName}")
        Console.WriteLine($"  OS: {Environment.OSVersion}")
        Console.WriteLine("══════════════════════════════════════════════")
    End Sub

    ''' <summary>
    ''' Print summary
    ''' </summary>
    Sub PrintSummary()
        Dim endMemory = GC.GetTotalMemory(False)
        Dim memoryUsed = (endMemory - _startMemory) / 1024.0 / 1024.0

        Console.WriteLine()
        Console.WriteLine("══════════════════════════════════════════════")
        Console.WriteLine("  TEST SUMMARY")
        Console.WriteLine("══════════════════════════════════════════════")
        Console.WriteLine($"  Total Tests:    {_totalTests}")
        Console.WriteLine($"  Passed:         {_passedTests} ✅")
        Console.WriteLine($"  Failed:         {_failedTests} ❌")
        Console.WriteLine($"  Pass Rate:      {If(_totalTests > 0, (_passedTests / _totalTests * 100).ToString("F1"), "0")}%")
        Console.WriteLine($"  Total Time:     {_totalTime} ms")
        Console.WriteLine($"  Memory Used:    {memoryUsed:F2} MB")
        Console.WriteLine("══════════════════════════════════════════════")

        If _failedTests > 0 Then
            Console.WriteLine()
            Console.WriteLine("  FAILED TESTS:")
            For Each result In _testResults.Where(Function(x) Not x.Passed)
                Console.WriteLine($"  ❌ {result.Name}: {result.ErrorMessage}")
            Next
        End If

        Dim grade = CalculateGrade()
        Console.WriteLine($"  Framework Grade: {grade}")
        Console.WriteLine("══════════════════════════════════════════════")
    End Sub

    ''' <summary>
    ''' Calculate framework grade
    ''' </summary>
    Function CalculateGrade() As String
        If _passedTests = _totalTests AndAlso _totalTests > 0 Then
            Return "🏆 A+ (All Tests Passed)"
        ElseIf _passedTests >= _totalTests * 0.95 Then
            Return "✅ A (95%+ Passed)"
        ElseIf _passedTests >= _totalTests * 0.8 Then
            Return "⚠️ B (80%+ Passed)"
        Else
            Return "❌ C (Needs Fix)"
        End If
    End Function

    ''' <summary>
    ''' Test result data class
    ''' </summary>
    Class TestResult
        Public Property Name As String
        Public Property Passed As Boolean
        Public Property TimeMs As Long
        Public Property ErrorMessage As String
    End Class

#End Region

#Region "Test Data Classes"

    Public Class Orang
        Public Property Nama As String
        Public Property Umur As Integer
        Public Property Alamat As String
        Public Property Aktif As Boolean
        Public Property TanggalLahir As DateTime
    End Class

    Public Class OrangDTO
        Public Property Nama As String
        Public Property Umur As Integer
    End Class

#End Region

#Region "[DF] DEFAULT SETTINGS Tests"

    Sub Test_DF_Settings()

        ' DF01 - Thread-safe settings
        RunTest("DF01_ThreadSafe", Sub()
                                       Dim df01 = DF01_ThreadSafe.Instance
                                       Dim settings = df01.GetSettings()
                                       Debug.Assert(settings IsNot Nothing, "Settings should not be null")
                                       Debug.Assert(df01.IsInitialized, "Should be initialized")
                                       df01.SetFormatting(False)
                                       df01.ResetToDefault()
                                   End Sub)

        ' DF02 - Custom settings
        RunTest("DF02_Custom", Sub()
                                   Dim custom = DF02_Custom.BuatPengaturan(
                                       NullValueHandling.Include,
                                       Formatting.None,
                                       True)
                                   Debug.Assert(
                                       custom.NullValueHandling = NullValueHandling.Include,
                                       "Null handling should be Include")
                                   Debug.Assert(
                                       custom.Formatting = Formatting.None,
                                       "Formatting should be None")
                               End Sub)

        ' DF03 - Global update
        RunTest("DF03_Global", Sub()
                                   Dim current = DF03_Global.DapatkanTerkini()
                                   Debug.Assert(current IsNot Nothing, "Global settings should not be null")
                                   DF03_Global.AturFormat(False)
                                   DF03_Global.ResetKeDefault()
                               End Sub)

        ' DF04 - CamelCase
        RunTest("DF04_CamelCase", Sub()
                                      Debug.Assert(Not DF04_CamelCase.StatusAktif, "Should start disabled")
                                      DF04_CamelCase.Aktifkan()
                                      Debug.Assert(DF04_CamelCase.StatusAktif, "Should be enabled")
                                      DF04_CamelCase.Nonaktifkan()
                                      Debug.Assert(Not DF04_CamelCase.StatusAktif, "Should be disabled")
                                  End Sub)

    End Sub

#End Region

#Region "[JP] JSON PARSING Tests"

    Sub Test_JP_Parsing()
        Dim validJson = "{""nama"":""Budi"",""umur"":25}"
        Dim invalidJson = "{nama: Budi}"
        Dim jsonArray = "[1,2,3,4,5]"

        ' JP01 - Validate
        RunTest("JP01_Validate", Sub()
                                     Debug.Assert(JP01_Validate.ApakahValid(validJson), "Should be valid")
                                     Debug.Assert(Not JP01_Validate.ApakahValid(invalidJson), "Should be invalid")
                                     Debug.Assert(JP01_Validate.ApakahObject(validJson), "Should be object")
                                     Debug.Assert(JP01_Validate.ApakahArray(jsonArray), "Should be array")
                                 End Sub)

        ' JP02 - Parse JObject
        RunTest("JP02_ParseJObject", Sub()
                                         Dim jObj = JP02_ParseJObject.Parse(validJson)
                                         Debug.Assert(jObj IsNot Nothing, "Should parse successfully")
                                         Debug.Assert(jObj("nama").ToString() = "Budi", "Should have nama")
                                     End Sub)

        ' JP03 - Parse JArray
        RunTest("JP03_ParseJArray", Sub()
                                        Dim jArr = JP03_ParseJArray.Parse(jsonArray)
                                        Debug.Assert(jArr IsNot Nothing, "Should parse successfully")
                                        Debug.Assert(jArr.Count = 5, "Should have 5 items")
                                    End Sub)

        ' JP04 - Parse JToken
        RunTest("JP04_ParseJToken", Sub()
                                        Dim token1 = JP04_ParseJToken.Parse(validJson)
                                        Dim token2 = JP04_ParseJToken.Parse(jsonArray)
                                        Debug.Assert(token1.Type = JTokenType.Object, "Should be Object")
                                        Debug.Assert(token2.Type = JTokenType.Array, "Should be Array")
                                    End Sub)

        ' JP05 - Optimized parsing
        RunTest("JP05_Optimized", Sub()
                                      JP05_Optimized.Parse(validJson)
                                      JP05_Optimized.Parse(validJson) ' Cache hit
                                      Debug.Assert(JP05_Optimized.CacheAktif, "Cache should be active")
                                      JP05_Optimized.BersihkanCache()
                                  End Sub)

    End Sub

#End Region

#Region "[SR] SERIALIZATION Tests"

    Sub Test_SR_Serialization()
        Dim orang = New Orang With {
            .Nama = "Budi Santoso",
            .Umur = 28,
            .Alamat = "Jakarta",
            .Aktif = True,
            .TanggalLahir = New DateTime(1996, 5, 15)
        }

        ' SR01 - Object to JSON
        RunTest("SR01_ToJson", Sub()
                                   Dim json = SR01_Serialize.ToJson(orang)
                                   Debug.Assert(Not String.IsNullOrEmpty(json), "Should produce JSON")
                                   Debug.Assert(json.Contains("Budi Santoso"), "Should contain name")
                               End Sub)

        ' SR02 - Compact
        RunTest("SR02_Compact", Sub()
                                    Dim compact = SR02_Compact.ToJson(orang)
                                    Debug.Assert(Not compact.Contains(vbCrLf), "Should not have newlines")
                                End Sub)

        ' SR03 - Custom resolver
        RunTest("SR03_CustomResolver", Sub()
                                           Dim snake = SR03_CustomResolver.ToJsonSnakeCase(orang)
                                           Debug.Assert(snake.Contains("nama"), "Should contain snake_case key")
                                       End Sub)

        ' SR04 - Exclude
        RunTest("SR04_Exclude", Sub()
                                    Dim excluded = SR04_Exclude.ToJson(orang, "Alamat")
                                    Debug.Assert(Not excluded.Contains("Alamat"), "Should not contain Alamat")
                                End Sub)

        ' SR05 - Generic
        RunTest("SR05_Generic", Sub()
                                    Dim json = SR05_Generic.ToJson(orang, True, True, True)
                                    Debug.Assert(json.Contains("nama"), "Should have camelCase nama")
                                End Sub)

    End Sub

#End Region

#Region "[DS] DESERIALIZATION Tests"

    Sub Test_DS_Deserialization()
        Dim jsonOrang = "{""Nama"":""Ani"",""Umur"":25,""Alamat"":""Bandung""," &
                        """Aktif"":true,""TanggalLahir"":""1999-03-20T00:00:00""}"
        Dim jsonArray = "[{""Nama"":""A"",""Umur"":10},{""Nama"":""B"",""Umur"":20}," &
                        "{""Nama"":""C"",""Umur"":30}]"

        ' DS01 - JSON to Object
        RunTest("DS01_FromJson", Sub()
                                     Dim orang = DS01_Deserialize.FromJson(Of Orang)(jsonOrang)
                                     Debug.Assert(orang IsNot Nothing, "Should deserialize")
                                     Debug.Assert(orang.Nama = "Ani", "Should have correct name")
                                 End Sub)

        ' DS02 - Safe deserialize
        RunTest("DS02_Safe", Sub()
                                 Dim pesanError As String = ""
                                 Dim orang = DS02_Safe.FromJson(Of Orang)(jsonOrang, pesanError)
                                 Debug.Assert(orang IsNot Nothing, "Should deserialize safely")
                                 Debug.Assert(String.IsNullOrEmpty(pesanError), "Should have no error")
                             End Sub)

        ' DS03 - Typed deserialize
        RunTest("DS03_Typed", Sub()
                                  Dim obj = DS03_Typed.FromJson(jsonOrang, GetType(Orang))
                                  Debug.Assert(obj IsNot Nothing, "Should deserialize with type")
                              End Sub)

        ' DS04 - Try deserialize
        RunTest("DS04_Try", Sub()
                                Dim hasil As Orang = Nothing
                                Dim success = DS04_Try.TryDeserialize(Of Orang)(jsonOrang, hasil)
                                Debug.Assert(success, "Should succeed")
                                Debug.Assert(hasil IsNot Nothing, "Should have result")
                            End Sub)

        ' DS05 - Default fallback
        RunTest("DS05_Default", Sub()
                                    Dim invalid = "{invalid}"
                                    Dim def = DS05_Default.FromJson(Of Orang)(
                                        invalid,
                                        New Orang With {.Nama = "Default"})
                                    Debug.Assert(def.Nama = "Default", "Should return default")
                                End Sub)

        ' DS06 - Strict
        RunTest("DS06_Strict", Sub()
                                   Dim orang = DS06_Strict.FromJson(Of Orang)(jsonOrang)
                                   Debug.Assert(orang IsNot Nothing, "Should deserialize strict")
                               End Sub)

    End Sub

#End Region

#Region "[FO] FILE OPERATIONS Tests"

    Sub Test_FO_FileOperations()
        Dim testPath = IO.Path.Combine(IO.Path.GetTempPath(), "ZyJsonKit_Test")
        Dim testFile = IO.Path.Combine(testPath, "test.json")
        Dim testJson = "{""nama"":""File Test"",""umur"":100}"

        Try
            ' FO01 - Save
            RunTest("FO01_Save", Sub()
                                     Dim saved = FO01_Save.Simpan(testFile, testJson)
                                     Debug.Assert(saved, "Should save successfully")
                                     Debug.Assert(IO.File.Exists(testFile), "File should exist")
                                 End Sub)

            ' FO02 - Load
            RunTest("FO02_Load", Sub()
                                     Dim loaded = FO02_Load.Muat(testFile)
                                     Debug.Assert(loaded IsNot Nothing, "Should load successfully")
                                 End Sub)

            ' FO03 - Async Save
            RunTest("FO03_Async", Sub()
                                      Dim task = FO03_Async.SimpanAsync(testFile, testJson)
                                      task.Wait()
                                      Debug.Assert(task.Result, "Should save async")
                                  End Sub)

            ' FO04 - Async Load
            RunTest("FO04_ReadAsync", Sub()
                                          Dim task = FO04_ReadAsync.BacaAsync(testFile)
                                          task.Wait()
                                          Debug.Assert(task.Result IsNot Nothing, "Should load async")
                                      End Sub)

            ' FO05 - Try Load
            RunTest("FO05_TryLoad", Sub()
                                        Dim hasil As String = ""
                                        Dim success = FO05_TryLoad.CobaMuat(testFile, hasil)
                                        Debug.Assert(success, "Should load with Try")
                                        Debug.Assert(Not String.IsNullOrEmpty(hasil), "Should have content")
                                    End Sub)

            ' FO06 - UTF8
            RunTest("FO06_UTF8", Sub()
                                     Dim saved = FO06_UTF8.SimpanUTF8(testFile, testJson, True)
                                     Debug.Assert(saved, "Should save UTF8")
                                     Dim hasBOM = FO06_UTF8.MemilikiBOM(testFile)
                                     Debug.Assert(hasBOM, "Should have BOM")
                                 End Sub)

        Finally
            If IO.Directory.Exists(testPath) Then
                Try
                    IO.Directory.Delete(testPath, True)
                Catch
                End Try
            End If
        End Try
    End Sub

#End Region

#Region "[JT] JOBJECT/JTOKEN Tests"

    Sub Test_JT_Operations()
        Dim json = "{""data"":{""nama"":""Budi"",""umur"":25}," &
                   """items"":[{""id"":1},{""id"":2},{""id"":3}]}"
        Dim jObj = JObject.Parse(json)

        ' JT01 - GetValue
        RunTest("JT01_GetValue", Sub()
                                     ' ✅ Fix: JO01_GetValue → JT01_GetValue
                                     Dim nama = JT01_GetValue.AmbilString(jObj, "data.nama")
                                     Debug.Assert(nama = "Budi", "Should get correct value")
                                 End Sub)

        ' JT02 - GetValue(Of T)
        RunTest("JT02_GetValueTyped", Sub()
                                          ' ✅ Fix: JO02_GetValueTyped → JT02_GetValueTyped
                                          Dim umur = JT02_GetValueTyped.Ambil(Of Integer)(jObj, "data.umur", 0)
                                          Debug.Assert(umur = 25, "Should get typed value")
                                      End Sub)

        ' JT03 - SetValue
        RunTest("JT03_SetValue", Sub()
                                     ' ✅ Fix: JO03_SetValue → JT03_SetValue
                                     JT03_SetValue.AturString(jObj, "data.alamat", "Jakarta")
                                     Debug.Assert(
                                         jObj.SelectToken("data.alamat").ToString() = "Jakarta",
                                         "Should set value")
                                 End Sub)

        ' JT04 - Merge
        RunTest("JT04_Merge", Sub()
                                  ' ✅ Fix: JO04_Merge → JT04_Merge
                                  Dim target = JObject.Parse("{""a"":1}")
                                  Dim sumber = JObject.Parse("{""b"":2}")
                                  JT04_Merge.Gabungkan(target, sumber)
                                  Debug.Assert(target("b") IsNot Nothing, "Should merge")
                              End Sub)

        ' JT05 - Path Support
        RunTest("JT05_PathSupport", Sub()
                                        ' ✅ Fix: JO05_PathSupport → JT05_PathSupport
                                        Dim token = JT05_PathSupport.CariSatu(jObj, "$.data.nama")
                                        Debug.Assert(token IsNot Nothing, "Should find token")
                                    End Sub)

        ' JT06 - Dynamic Token
        RunTest("JT06_DynamicToken", Sub()
                                         ' ✅ Fix: JO06_DynamicToken → JT06_DynamicToken
                                         Dim newObj = JT06_DynamicToken.BuatDariObject(New With {.x = 1})
                                         Debug.Assert(newObj("x").Value(Of Integer)() = 1, "Should create dynamic")
                                     End Sub)

    End Sub

#End Region

#Region "[DC] DICTIONARY CONVERSION Tests"

    Sub Test_DC_Dictionary()
        Dim jsonObj = "{""nama"":""Budi"",""umur"":25,""aktif"":true}"

        ' DC01 - To Dictionary
        RunTest("DC01_ToDictionary", Sub()
                                         ' ✅ Fix: DI01_ToDictionary → DC01_ToDictionary
                                         Dim dict = DC01_ToDictionary.DariJson(jsonObj)
                                         Debug.Assert(dict.Count = 3, "Should have 3 keys")
                                     End Sub)

        ' DC02 - Recursive
        RunTest("DC02_Recursive", Sub()
                                      ' ✅ Fix: DI02_Recursive → DC02_Recursive
                                      Dim nested = DC02_Recursive.DariJson("{""data"":{""nama"":""Budi""}}")
                                      Debug.Assert(nested IsNot Nothing, "Should convert recursive")
                                  End Sub)

        ' DC03 - Array Handling
        RunTest("DC03_ArrayHandling", Sub()
                                          ' ✅ Fix: DI03_ArrayHandling → DC03_ArrayHandling
                                          Dim list = DC03_ArrayHandling.KeListDictionary("[{""id"":1}]")
                                          Debug.Assert(list.Count = 1, "Should have 1 item")
                                      End Sub)

        ' DC04 - Primitive Mapping
        RunTest("DC04_PrimitiveMapping", Sub()
                                             ' ✅ Fix: DI04_PrimitiveMapping → DC04_PrimitiveMapping
                                             Dim token = New JValue(123)
                                             Dim primitif = DC04_PrimitiveMapping.KePrimitif(token)
                                             Debug.Assert(TypeOf primitif Is Integer, "Should be Integer")
                                         End Sub)

    End Sub

#End Region

#Region "[LA] LIST & ARRAY Tests"

    Sub Test_LA_ListArray()
        Dim data As New List(Of Orang)() From {
            New Orang With {.Nama = "Budi", .Umur = 25},
            New Orang With {.Nama = "Ani", .Umur = 30}
        }

        ' LA01 - Serialize List
        RunTest("LA01_SerializeList", Sub()
                                          Dim json = LA01_SerializeList.ToJson(data)
                                          Debug.Assert(Not String.IsNullOrEmpty(json), "Should serialize list")
                                      End Sub)

        ' LA02 - Deserialize List
        RunTest("LA02_DeserializeList", Sub()
                                            Dim json = LA01_SerializeList.ToJson(data)
                                            Dim list = LA02_DeserializeList.FromJson(Of Orang)(json)
                                            Debug.Assert(list.Count = 2, "Should deserialize list")
                                        End Sub)

        ' LA03 - Deserialize Array
        RunTest("LA03_DeserializeArray", Sub()
                                             Dim json = LA01_SerializeList.ToJson(data)
                                             Dim arr = LA03_DeserializeArray.FromJson(Of Orang)(json)
                                             Debug.Assert(arr.Length = 2, "Should deserialize array")
                                         End Sub)

        ' LA04 - Collection Support
        RunTest("LA04_Collection", Sub()
                                       Dim hashSet = LA04_Collection.ToHashSet(Of Integer)("[1,2,3]")
                                       Debug.Assert(hashSet.Count = 3, "Should create HashSet")
                                   End Sub)

    End Sub

#End Region

#Region "[FM] FORMATTING Tests"

    Sub Test_FM_Formatting()
        Dim jsonCompact = "{""nama"":""Budi"",""umur"":25}"

        ' FM01 - PrettyPrint
        RunTest("FM01_PrettyPrint", Sub()
                                        Dim rapi = FM01_PrettyPrint.Format(jsonCompact)
                                        Debug.Assert(rapi.Contains(vbCrLf), "Should have newlines")
                                    End Sub)

        ' FM02 - Minify
        RunTest("FM02_Minify", Sub()
                                   Dim rapi = FM01_PrettyPrint.Format(jsonCompact)
                                   Dim minified = FM02_Minify.Minify(rapi)
                                   Debug.Assert(Not minified.Contains(vbCrLf), "Should not have newlines")
                               End Sub)

        ' FM03 - Format Control
        RunTest("FM03_FormatControl", Sub()
                                          Dim sorted = FM03_FormatControl.UrutkanProperti(jsonCompact)
                                          Debug.Assert(sorted.Contains("nama"), "Should have nama")
                                      End Sub)

    End Sub

#End Region

#Region "[CL] CLONE SYSTEM Tests"

    Sub Test_CL_Clone()
        Dim original = New Orang With {.Nama = "Budi", .Umur = 28}

        ' CL01 - Deep Clone
        RunTest("CL01_DeepClone", Sub()
                                      Dim cloned = CL01_DeepClone.Clone(original)
                                      cloned.Nama = "Modified"
                                      Debug.Assert(original.Nama <> cloned.Nama, "Should be deep cloned")
                                  End Sub)

        ' CL02 - Generic Clone
        RunTest("CL02_GenericClone", Sub()
                                         Dim cloned = CL02_GenericClone.Clone(Of Orang)(original)
                                         Debug.Assert(cloned IsNot Nothing, "Should clone generic")
                                     End Sub)

        ' CL03 - Serialization Copy
        RunTest("CL03_SerializationCopy", Sub()
                                              Dim copy = CL03_SerializationCopy.Salin(original)
                                              Debug.Assert(copy.Nama = original.Nama, "Should copy")
                                          End Sub)

    End Sub

#End Region

#Region "[DY] DYNAMIC OBJECTS Tests"

    Sub Test_DY_Dynamic()

        ' DY01 - Dynamic Deserialize
        RunTest("DY01_DynamicDeserialize", Sub()
                                               Dim expando = DY01_DynamicDeserialize.FromJson("{""nama"":""Budi""}")
                                               Debug.Assert(expando IsNot Nothing, "Should deserialize dynamic")
                                           End Sub)

        ' DY02 - ExpandoObject
        RunTest("DY02_ExpandoObject", Sub()
                                          Dim expando = DY02_ExpandoObject.Buat()
                                          DY02_ExpandoObject.AturProperti(expando, "key", "value")
                                          Dim value = DY02_ExpandoObject.AmbilValue(Of String)(expando, "key")
                                          Debug.Assert(value = "value", "Should get value")
                                      End Sub)

        ' DY03 - Anonymous Object
        RunTest("DY03_AnonymousObject", Sub()
                                            Dim anon = New With {.x = 10, .y = 20}
                                            Dim json = DY03_AnonymousObject.ToJson(anon)
                                            Debug.Assert(json.Contains("x"), "Should serialize anonymous")
                                        End Sub)

    End Sub

#End Region

#Region "[CR] CONTRACT RESOLVER Tests"

    Sub Test_CR_Resolver()
        Dim orang = New Orang With {
            .Nama = "Test",
            .Umur = 30,
            .Alamat = "Secret"
        }

        ' CR01 - Exclude Property
        RunTest("CR01_ExcludeProperty", Sub()
                                            Dim resolver = New CR01_ExcludeProperty("Alamat")
                                            Dim settings = New JsonSerializerSettings With {
                                                .ContractResolver = resolver
                                            }
                                            Dim json = JsonConvert.SerializeObject(orang, settings)
                                            Debug.Assert(Not json.Contains("Alamat"), "Should exclude Alamat")
                                        End Sub)

        ' CR02 - Custom Logic
        RunTest("CR02_CustomLogic", Sub()
                                        Dim resolver = New CR02_CustomLogic(
                                            Function(name) name <> "Alamat",
                                            Function(name) name.ToLower())
                                        Dim settings = New JsonSerializerSettings With {
                                            .ContractResolver = resolver
                                        }
                                        Dim json = JsonConvert.SerializeObject(orang, settings)
                                        Debug.Assert(json.Contains("nama"), "Should have lowercase nama")
                                    End Sub)

        ' CR03 - Reflection Filter
        RunTest("CR03_ReflectionFilter", Sub()
                                             Dim resolver = New CR03_ReflectionFilter({GetType(String)})
                                             Dim settings = New JsonSerializerSettings With {
                                                 .ContractResolver = resolver
                                             }
                                             Dim json = JsonConvert.SerializeObject(orang, settings)
                                             Debug.Assert(json.Contains("Nama"), "Should have string properties")
                                         End Sub)

    End Sub

#End Region

#Region "[TS] THREAD SAFETY Tests"

    Sub Test_TS_ThreadSafety()

        ' TS01 - SyncLock
        RunTest("TS01_SyncLock", Sub()
                                     Dim result = TS01_SyncLock.EksekusiDenganLock(Function() "test")
                                     Debug.Assert(result = "test", "Should execute with lock")
                                 End Sub)

        ' TS02 - Shared Resource
        RunTest("TS02_SharedResource", Sub()
                                           TS02_SharedResource.SimpanResource("key", "value")
                                           Dim value = TS02_SharedResource.DapatkanResource(Of String)("key")
                                           Debug.Assert(value = "value", "Should get shared resource")
                                       End Sub)

        ' TS03 - Static Init
        RunTest("TS03_StaticInit", Sub()
                                       Dim initResult = TS03_StaticInit.Inisialisasi(Sub()
                                                                                         ' Action inisialisasi
                                                                                     End Sub)
                                       Debug.Assert(initResult, "Inisialisasi harus berhasil")
                                       Debug.Assert(TS03_StaticInit.SudahInisialisasi, "Harus sudah diinisialisasi")
                                       Debug.Assert(TS03_StaticInit.JumlahInisialisasi > 0, "Jumlah inisialisasi harus > 0")
                                   End Sub)

    End Sub

#End Region

#Region "[PF] PRODUCTION FEATURES Tests"

    Sub Test_PF_Production()

        ' PF01 - Option Strict
        RunTest("PF01_OptionStrictOn", Sub()
                                           Dim obj = PF01_OptionStrictOn.Deserialize(Of Orang)(
                                               "{""Nama"":""Test"",""Umur"":50}")
                                           Debug.Assert(obj IsNot Nothing, "Should deserialize strict")
                                       End Sub)

        ' PF02 - Generic Safe
        RunTest("PF02_GenericSafe", Sub()
                                        Dim json = PF02_GenericSafe.Serialize(
                                            New Orang With {.Nama = "Safe"})
                                        Debug.Assert(Not String.IsNullOrEmpty(json), "Should serialize safely")
                                    End Sub)

        ' PF03 - Exception Safe
        RunTest("PF03_ExceptionSafe", Sub()
                                          Dim result = PF03_ExceptionSafe.EksekusiDenganDefault(
                                              Function() 42, -1)
                                          Debug.Assert(result = 42, "Should execute safely")
                                      End Sub)

        ' PF04 - Null Safe
        RunTest("PF04_NullSafe", Sub()
                                     Dim safe = PF04_NullSafe.StringSafe(Nothing, "default")
                                     Debug.Assert(safe = "default", "Should return default")
                                 End Sub)

        ' PF05 - Async Ready
        RunTest("PF05_AsyncReady", Sub()
                                       Dim task = PF05_AsyncReady.SerializeAsync(
                                           New Orang With {.Nama = "Async"})
                                       task.Wait()
                                       Debug.Assert(task.Result IsNot Nothing, "Should serialize async")
                                   End Sub)

        ' PF06 - Memory Optimized
        RunTest("PF06_MemoryOptimized", Sub()
                                            Dim json = PF06_MemoryOptimized.Serialize(
                                                New Orang With {.Nama = "Mem"})
                                            Debug.Assert(Not String.IsNullOrEmpty(json), "Should serialize optimized")
                                        End Sub)

    End Sub

#End Region

#Region "[AF] ARCHITECTURE Tests"

    Sub Test_AF_Architecture()

        ' AF01 - Generic Methods
        RunTest("AF01_GenericMethods", Sub()
                                           Dim json = AF01_GenericMethods.Serialize(
                                               New Orang With {.Nama = "Arch"})
                                           Debug.Assert(Not String.IsNullOrEmpty(json), "Should serialize generic")
                                       End Sub)

        ' AF02 - Reusable Design
        RunTest("AF02_ReusableDesign", Sub()
                                           Dim features = AF02_ReusableDesign.DaftarFitur()
                                           Debug.Assert(features.Count = 15, "Should have 15 features")
                                       End Sub)

        ' AF03 - Modular Region
        RunTest("AF03_ModularRegion", Sub()
                                          Dim modules = AF03_ModularRegion.StrukturModul()
                                          Debug.Assert(modules.Count = 15, "Should have 15 modules")
                                      End Sub)

        ' AF04 - Enterprise Naming
        RunTest("AF04_EnterpriseNaming", Sub()
                                             Dim prefixes = AF04_EnterpriseNaming.PrefixConvention()
                                             Debug.Assert(prefixes.Count = 15, "Should have 15 prefixes")
                                         End Sub)

        ' AF05 - Extensible Architecture
        RunTest("AF05_ExtensibleArchitecture", Sub()
                                                   Dim serializer = AF05_ExtensibleArchitecture.BuatSerializer()
                                                   Debug.Assert(serializer IsNot Nothing, "Should create serializer")
                                                   Dim json = serializer.Serialize(New Orang With {.Nama = "Ext"})
                                                   Debug.Assert(Not String.IsNullOrEmpty(json), "Should serialize")
                                               End Sub)

    End Sub

#End Region

End Module