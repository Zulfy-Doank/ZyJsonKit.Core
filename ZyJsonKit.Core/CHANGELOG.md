# Changelog

All notable changes to **ZyJsonKit.Core** will be documented in this file.

Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
Versioning based on [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

---

## [1.0.0] - 2026-05-21

### 🎉 Initial Release — Production Ready

> **66/66 Tests Passed · Grade A+ · Memory 0.32–0.40 MB**

---

### ✅ Added — [DF] Default Settings (4 classes)

| Class | Description |
|---|---|
| `DF01_ThreadSafe` | Singleton thread-safe `JsonSerializerSettings` dengan double-check locking pattern |
| `DF02_Custom` | Builder pattern untuk membuat `JsonSerializerSettings` kustom |
| `DF03_Global` | Runtime global settings management — update tanpa restart |
| `DF04_CamelCase` | CamelCase / snake_case resolver dengan toggle Aktifkan/Nonaktifkan |

**Key methods:**
```vbnet
Dim s = DF01_ThreadSafe.Instance
s.GetSettings()
s.SetFormatting(False)        ' False = Compact, True = Indented
s.ResetToDefault()

DF02_Custom.BuatPengaturan(NullValueHandling.Include, Formatting.None)
DF02_Custom.BuatPretty()
DF02_Custom.BuatStrict()

DF03_Global.DapatkanTerkini()
DF03_Global.AturFormat(False)
DF03_Global.ResetKeDefault()

DF04_CamelCase.Aktifkan()       ' Default: False (disabled)
DF04_CamelCase.Nonaktifkan()
DF04_CamelCase.StatusAktif      ' Boolean property