# 🧮 Modern Glassmorphism Calculator

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512bd4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/Platform-Windows-0078d4?style=for-the-badge&logo=windows)](https://www.microsoft.com/windows)
[![UI Style](https://img.shields.io/badge/UI-Glassmorphism-ff69b4?style=for-the-badge)](https://en.wikipedia.org/wiki/Glassmorphism)


A high-fidelity, Apple-inspired calculator built with **C# and WinForms**. This project demonstrates advanced GDI+ rendering techniques, including **anti-aliased rounded corners**, **real-time scale animations**, and **frosted glass effects**—all within the classic WinForms framework.

---

## ✨ Features

- **💎 Glassmorphism UI**: Beautiful semi-transparent surfaces with inner glows and sharp anti-aliased edges.
- **🚀 Ultra-Smooth Animations**: 60 FPS scale-down effect on button press and vibrant color transitions.
- **📱 Apple / macOS Aesthetics**: Carefully curated color palette (`Apple Orange`, `Frosted Gray`) and `Segoe UI Semibold` typography.
- **⚡ Pro Logic Engine**: 
  - Full arithmetic support (`+`, `-`, `×`, `÷`).
  - Advanced functions: `%`, `+/-`, and decimal precision.
  - Automatic font scaling for large numbers.
- **⌨️ Keyboard Support**: Fully interactive via Numpad and standard keys.
- **🛠️ Clean Architecture**: Decoupled Logic (`CalculatorEngine`) from Design (`CalcButton`, `Form1`).

---



## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later.
- Windows OS (Required for WinForms).

### Installation
1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/modern-calculator.git
   cd modern-calculator/WinFormsCalculator
   ```
2. **Run the application**:
   ```pwsh
   dotnet run
   ```

---

## 🛠️ Built With

- **C# / .NET 10**: Modern language features and performance.
- **Windows Forms**: Classic desktop framework pushed to its visual limits.
- **GDI+**: Custom graphics engine for "Frosted" effects and anti-aliasing.

