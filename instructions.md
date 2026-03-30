# Project Name: SharpSync (Core)
# Developer: Alireza (Full-stack Engineer - 6 years exp in .NET & React)

## 1. Project Goal
Develop a high-performance .NET NuGet package that automatically synchronizes C# Controllers and DTOs with TypeScript Interfaces and TanStack Query Hooks for Next.js 15.

## 2. Technical Requirements (Phase 1)
- **Technology:** .NET 9, MSBuild Task, Reflection API.
- **Output:** A clean, optimized `.ts` file generated during the .NET build process.
- **Developer Experience (DX):** Zero-config. Use a custom Attribute `[SharpSync]` to mark classes for generation.
- **Architecture:** Must be modular and extensible to support future "Science/Chemistry" extensions (SOLID principles).

## 3. Key Features for "Elite" Status (O-1 Visa Strategy)
- **Source Transparency:** Output code must be highly readable (Clean Code).
- **Modern Integration:** Automatic generation of TanStack Query (React Query) hooks.
- **Type Safety:** Ensure 100% sync between C# Types and TypeScript.

## 4. Immediate Tasks for Gemini (IDX AI)
- Help me design a lightweight `Attribute` for C#.
- Provide a robust `Reflection` logic to scan the assembly for marked classes.
- Help create a `.targets` file for MSBuild to trigger the generation on Build.
- Ensure the code generation follows a "Plugin" architecture to allow a separate `SharpSync.Chemistry` package later.