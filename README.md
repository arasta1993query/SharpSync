# ⚡ SharpSync

**SharpSync** is a high-performance, developer-friendly .NET to TypeScript synchronization engine. It automatically transforms your C# Controllers and DTOs into fully typed, production-ready **TanStack Query hooks** and **Zod validation schemas**.

Stop manually writing frontend API clients. Focus on your C# logic, and let SharpSync handle the rest.

---

## 🚀 Key Features

- **🎯 TanStack Query Ready**: Generates fully typed `useQuery` and `useMutation` hooks out-of-the-box.
- **🛡️ Selective Zod Validation**: Automatically translates C# Data Annotations (`[Required]`, `[Range]`, `[StringLength]`, etc.) into Zod schemas for form-based endpoints.
- **📦 Modular Architecture**: Generates a clean, multi-file structure with automated dependency tracking and relative imports.
- **📎 Smart FormData Support**: Intelligent handling of `[FromForm]` attributes and file uploads with automatic serialization.
- **🔌 Client Agnostic**: Built-in scaffolds for both **Axios** and **Fetch API**.
- **🔍 RFC 7807 Error Handling**: Robust, standardized error handling for consistent frontend feedback.

---

## 🛠️ Installation

### 1. Add the Core attributes (NuGet)
Install the core attributes package to your Web API project to enable selective features:

```bash
dotnet add package SharpSync.Core
```

### 2. Install the CLI Tool
Install the SharpSync global tool to generate your TypeScript files:

```bash
dotnet tool install --global SharpSync.Tool
```

---

## 📖 Quick Start

### 1. Annotate your C# Controller
Decorate any controller you want to sync. Use `[SharpSyncForm]` on methods that require Zod validation.

```csharp
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpPost]
    [SharpSyncForm] // Opt-in to Zod schema generation for this DTO
    public async Task<ActionResult<ForecastDto>> Create([FromBody] CreateForecastDto dto)
    {
        // Your logic...
    }
}

public class CreateForecastDto 
{
    [Required, MinLength(3)]
    public string Summary { get; set; }

    [Range(-100, 100)]
    public int TemperatureC { get; set; }
}
```

### 2. Run the Generator
Point the tool to your compiled assembly:

```bash
sharpsync "path/to/YourProject.dll" --output "./frontend/src/api" --client axios
```

### 3. Use in React/Next.js
SharpSync generates a clean directory with hooks and models:

```typescript
import { useCreateMutation } from './api/hooks/WeatherForecastHooks';

export function CreateForecastForm() {
    const { mutate, isLoading } = useCreateMutation();

    const onSubmit = (data: CreateForecastDto) => {
        mutate(data);
    };

    // ...
}
```

---

## 📂 Generated Structure

SharpSync follows a modular approach for better bundle size and maintainability:

```bash
SharpSyncGenerated/
├── apiClient.ts           # Configurable base client
├── models/                # Interfaces & Zod schemas (1 file per DTO)
│   ├── CreateForecastDto.ts
│   └── ForecastDto.ts
└── hooks/                 # TanStack Query Hooks (1 file per Controller)
    └── WeatherForecastHooks.ts
```

---

## ⚙️ CLI Options

| Option | Shorthand | Description | Default |
| --- | --- | --- | --- |
| `--output` | `-o` | Target directory for generated files | `./SharpSyncGenerated` |
| `--client` | `-c` | HTTP client to scaffold (`axios` \| `fetch`) | `axios` |
| `--force` | `-f` | Overwrite existing `apiClient.ts` | `false` |
| `--help` | `-h` | Show help information | - |

---

## 🧩 Data Annotations Support

SharpSync automatically translates the following to Zod rules:

- `[Required]` -> `.min(1)`
- `[StringLength(max, MinimumLength = min)]` -> `.max(max).min(min)`
- `[Range(min, max)]` -> `.min(min).max(max)`
- `[EmailAddress]` -> `.email()`
- `[Url]` -> `.url()`
- `[RegularExpression]` -> `.regex()`

---

## 📄 License
MIT License. Feel free to use and contribute!
",Description:
