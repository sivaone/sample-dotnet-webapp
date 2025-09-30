---

## 🧭 CONTRIBUTING.md

```markdown
# Contributing to [Your Project Name]

Welcome! We're excited to have you contribute. This guide outlines the standards, workflows, and tools we use to ensure high-quality, maintainable, and secure code.

---

## 🧰 Developer Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/your-project.git
   cd your-project
   ```

2. **Install prerequisites**
    - [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
    - Optional: Visual Studio 2022+, Rider, or VS Code

3. **Configure secrets for local development**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "DatabaseSettings:Password" "your-local-password"
   ```

---

## ⚙️ Configuration Strategy

We use a layered configuration approach:

| Source | Purpose | Example |
|--------|---------|---------|
| `appsettings.json` | Default values | `"ApiKey": "dev-key"` |
| `appsettings.Development.json` | Local overrides | `"Logging": { "LogLevel": "Debug" }` |
| Environment Variables | CI/CD secrets | `DatabaseSettings__Password=secure-value` |
| User Secrets | Local dev secrets | `dotnet user-secrets set` |
| `launchSettings.json` | Debug-time env vars | `"environmentVariables": { "ApiKey": "debug-key" }` |

---

## 🧪 Testing & Coverage

We enforce a minimum of **80% line coverage** using Coverlet.

### Run tests with coverage:
```bash
dotnet test --settings coverlet.runsettings
```

### Coverage config:
See [`coverlet.runsettings`](./coverlet.runsettings) for thresholds, exclusions, and output format.

---

## 🧼 Code Quality

We use built-in .NET analyzers and custom Roslyn rules to enforce style, performance, and architectural boundaries.

### Analyzer settings:
```xml
<EnableNETAnalyzers>true</EnableNETAnalyzers>
<AnalysisMode>AllEnabledByDefault</AnalysisMode>
```

### Customize via `.editorconfig`:
```ini
dotnet_diagnostic.CA1062.severity = error
dotnet_diagnostic.CA1822.severity = warning
```

---

## 📦 Pull Request Checklist

Before submitting a PR:

- [ ] Code builds and passes all tests
- [ ] Coverage ≥ 80% (or justified exclusions)
- [ ] No analyzer warnings (unless suppressed with reason)
- [ ] Follows naming, async, and nullability conventions
- [ ] Includes relevant documentation or comments
- [ ] Adds or updates unit tests if needed

---

## 🤝 Contributor Etiquette

- Be kind, constructive, and inclusive
- Prefer clarity over cleverness
- Justify exceptions to rules in comments
- Use PR templates to guide your submission
- Ask questions—collaboration is key!

---

## 📚 Resources

- [Onboarding Wiki](./docs/onboarding.md)
- [Architecture Overview](./docs/architecture.md)
- [Code of Conduct](./CODE_OF_CONDUCT.md)
```

---
