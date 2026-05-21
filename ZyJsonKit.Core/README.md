# Changelog

All notable changes to **ZyJsonKit.Core** will be documented in this file.

Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)  
Versioning based on [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

---

## [1.0.0] - 2026-05-21

### 🎉 Initial Release — Production Ready

#### Added
- **[DF] Default Settings** (4 classes)
  - `DF01_ThreadSafe` — Singleton thread-safe settings dengan double-check locking
  - `DF02_Custom` — Builder pattern untuk custom `JsonSerializerSettings`
  - `DF03_Global` — Runtime global settings management
  - `DF04_CamelCase` — CamelCase / snake_case resolver management

- **[JP] JSON Parsing** (5 classes)
  - `JP01_Validate` — Validasi JSON string dengan combined parse+validate
  - `JP02_ParseJObject` — Parse ke `JObject` (eliminasi triple-parse)
  - `JP03_ParseJArray` — Parse ke `JArray` (eliminasi triple-parse)
  - `JP04_ParseJToken` — Auto-detect parse ke `JToken`
  - `JP05_Optimized` — Cache-based parsing dengan per-type cache keys

- **[SR] Serialization** (5 classes)
  - `SR01_Serialize` — Standard serialization dengan CamelCase support
  - `SR02_Compact` — Compact/minified serialization
  - `SR03_CustomResolver` — Custom naming convention (snake_case, lowercase)
  - `SR04_Exclude` — Property exclusion/inclusion (whitelist/blacklist)
  - `SR05_Generic` — Full-options serialization

- **[DS] Deserialization** (6 classes)
  - `DS01_Deserialize` — Standard deserialization dengan CamelCase/SnakeCase
  - `DS02_Safe` — Safe deserialization dengan full validation + logging
  - `DS03_Typed` — Runtime type deserialization
  - `DS04_Try` — Try-pattern deserialization (never throws)
  - `DS05_Default` — Default fallback (never returns null)
  - `DS06_Strict` — Option Strict On compatible (Class constraint)

- **[FO] File Operations** (6 classes)
  - `FO01_Save` — Save JSON ke file dengan error handling
  - `FO02_Load` — Load JSON dari file
  - `FO03_Async` — Async file save
  - `FO04_ReadAsync` — Async file read
  - `FO05_TryLoad` — Try-pattern file loading dengan retry support
  - `FO06_UTF8` — UTF-8 dengan BOM control

- **[JT] JObject/JToken** (6 classes)
  - `JT01_GetValue` — Get value by path dengan typed defaults
  - `JT02_GetValueTyped` — Generic typed value extraction
  - `JT03_SetValue` — Set/update/delete value by path
  - `JT04_Merge` — Merge JSON objects (mutating dan non-mutating)
  - `JT05_PathSupport` — JSONPath expression support
  - `JT06_DynamicToken` — Dynamic token creation dan manipulation

- **[DC] Dictionary** (4 classes)
  - `DC01_ToDictionary` — JSON ↔ Dictionary conversion
  - `DC02_Recursive` — Nested recursive conversion dengan flatten
  - `DC03_ArrayHandling` — JSON array ↔ List/Dictionary dengan filter/sort
  - `DC04_PrimitiveMapping` — JToken ↔ .NET primitive type mapping

- **[LA] List & Array** (4 classes)
  - `LA01_SerializeList` — List serialization
  - `LA02_DeserializeList` — List deserialization dengan filter/transform
  - `LA03_DeserializeArray` — Array deserialization
  - `LA04_Collection` — HashSet, Queue, Stack support

- **[FM] Formatting** (3 classes)
  - `FM01_PrettyPrint` — Pretty print dengan custom indent
  - `FM02_Minify` — Minify dengan size comparison
  - `FM03_FormatControl` — Sort properties, wrap/unwrap, statistics

- **[CL] Clone System** (3 classes)
  - `CL01_DeepClone` — Deep clone via JSON serialization
  - `CL02_GenericClone` — Generic clone dengan transformasi
  - `CL03_SerializationCopy` — Fast serialization copy

- **[DY] Dynamic Objects** (3 classes)
  - `DY01_DynamicDeserialize` — JSON → ExpandoObject
  - `DY02_ExpandoObject` — ExpandoObject manipulation
  - `DY03_AnonymousObject` — Anonymous object serialization

- **[CR] Contract Resolver** (3 classes)
  - `CR01_ExcludeProperty` — Property exclusion resolver
  - `CR02_CustomLogic` — Custom filter + rename resolver
  - `CR03_ReflectionFilter` — Filter by property type

- **[TS] Thread Safety** (3 classes)
  - `TS01_SyncLock` — SyncLock execution wrapper
  - `TS02_SharedResource` — Thread-safe shared key-value store
  - `TS03_StaticInit` — One-time static initialization

- **[PF] Production Features** (6 classes)
  - `PF01_OptionStrictOn` — Option Strict On compatible methods
  - `PF02_GenericSafe` — Generic methods dengan Class constraint
  - `PF03_ExceptionSafe` — Exception-safe execution wrapper
  - `PF04_NullSafe` — Null-safe helpers (never null)
  - `PF05_AsyncReady` — Async serialization/deserialization
  - `PF06_MemoryOptimized` — StreamWriter/StringBuilder optimization

- **[AF] Architecture** (5 classes)
  - `AF01_GenericMethods` — Reusable generic methods
  - `AF02_ReusableDesign` — Framework metadata dan version
  - `AF03_ModularRegion` — Module structure documentation
  - `AF04_EnterpriseNaming` — Naming convention reference
  - `AF05_ExtensibleArchitecture` — Factory pattern + ICustomSerializer

#### Test Results
- **66/66 tests passed** (100% pass rate)
- **Grade: 🏆 A+**
- Memory: 0.32–0.40 MB
- Total time: 679–923ms (JIT warmup included)

#### Bug Fixes (during development)
- `JP05_Optimized` — Fixed cache collision bug (BUG01: shared cache key)
- `JP05_Optimized` — Fixed cross-contamination bug (BUG02: token cache override)
- `JP01_Validate` — Eliminated double-parse in `ApakahObject`/`ApakahArray`
- `JP02_ParseJObject` — Eliminated triple-parse in `ParseAman`
- `JP03_ParseJArray` — Eliminated triple-parse in `ParseAman`
- `DF04_CamelCase` — Fixed `StatusAktif` default (start disabled = False)
- `DS05_Default` — Fixed IDE0031 warning (simplified null check)
- `DS06_Strict` — Fixed IDE0084 warning (use `IsNot` expression)

---

## [Unreleased]

### Planned for v1.1.0
- [ ] JSON Schema validation support
- [ ] `System.Text.Json` adapter
- [ ] Streaming JSON support untuk large files
- [ ] Benchmark suite (BenchmarkDotNet)
- [ ] LINQ-to-JSON helper extensions

---

*Format: [MAJOR.MINOR.PATCH] - YYYY-MM-DD*  
*MAJOR: breaking change | MINOR: new feature | PATCH: bug fix*