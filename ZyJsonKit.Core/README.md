# 🚀 ZyJsonKit.Core

> **Enterprise-grade JSON library for VB.NET** — Thread-safe, Null-safe, Production-ready

[![NuGet Version](https://img.shields.io/nuget/v/ZyJsonKit.Core)](https://www.nuget.org/packages/ZyJsonKit.Core)
[![Build Status](https://github.com/Zulfy-Doank/ZyJsonKit.Core/actions/workflows/ci.yml/badge.svg)](https://github.com/Zulfy-Doank/ZyJsonKit.Core/actions)
[![License: MIT](https://img.shields.io/github/license/Zulfy-Doank/ZyJsonKit.Core)](https://opensource.org/licenses/MIT)
[![Tests](https://img.shields.io/badge/tests-66%2F66%20passed-brightgreen)](https://github.com/Zulfy-Doank/ZyJsonKit.Core)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0-blue)](https://dotnet.microsoft.com)
[![VB.NET](https://img.shields.io/badge/language-VB.NET-purple)](https://learn.microsoft.com/en-us/dotnet/visual-basic/)

---

## 📖 Overview

**ZyJsonKit.Core** adalah library JSON serialization/deserialization enterprise-grade untuk VB.NET yang dibangun di atas **Newtonsoft.Json (Json.NET)**.

Library ini lahir dari real-world production pain points:
- 🔒 **Thread-safety issues** → Semua singleton & shared resource thread-safe
- ⚡ **Triple-parse overhead** → Eliminasi hingga 3x parse untuk repeated types
- 💥 **Null-reference exception** → Full null-safety di semua public API
- 🔧 **Inconsistent settings** → Global runtime settings + builder pattern

---

## ✨ Features

| # | Module | Classes | Description |
|---|--------|---------|-------------|
| 🛠️ | **DF** Default Settings | 4 | Thread-safe singleton, custom builder, global runtime, CamelCase resolver |
| 📥 | **JP** JSON Parsing | 5 | Validate, Parse JObject/JArray/JToken, optimized bulk parser |
| 📤 | **SR** Serialization | 5 | Serialize object, compact mode, custom contract resolver, property exclusion, generic |
| 📦 | **DS** Deserialization | 6 | Standard, safe mode, typed, Try-pattern, default value, strict schema |
| 💾 | **FO** File Operations | 6 | Save/Load sync, async read/write, TryLoad pattern, UTF-8 guaranteed |
| 🔧 | **JT** JObject/JToken | 6 | Get value (plain & typed), set value, deep merge, JSONPath support, dynamic token |
| 📚 | **DC** Dictionary | 4 | ToDictionary, recursive nesting, array handling, primitive type mapping |
| 📋 | **LA** List & Array | 4 | Serialize list, deserialize list, deserialize array, generic collections |
| 🎨 | **FM** Formatting | 3 | Pretty print, minify/compact, format control per-serialization |
| 🧬 | **CL** Deep Clone | 3 | Deep clone via JToken, generic clone, serialization-based copy |
| 🎭 | **DY** Dynamic Objects | 3 | Dynamic deserialize, ExpandoObject support, anonymous object handling |
| 🎯 | **CR** Contract Resolver | 3 | Exclude properties, custom resolver logic, reflection-based filtering |
| 🔒 | **TS** Thread Safety | 3 | SyncLock wrapper, shared resource management, static initialization safety |
| 🏭 | **PF** Production | 6 | Option Strict On, generic-safe, exception-safe, null-safe, async-ready, memory optimized |
| 🏗️ | **AF** Architecture | 5 | Generic methods, reusable design patterns, modular regions, enterprise naming, extensible |

> **Total: 16 Modules · 66 Classes · 100% Test Pass Rate**

---

## 📦 Installation

### NuGet Package Manager
```bash
dotnet add package ZyJsonKit.Core