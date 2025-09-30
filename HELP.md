jobs:
analyze:
runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build with analyzers
      run: dotnet build --no-incremental -warnaserror

    - name: Generate SARIF report
      run: dotnet build -p:ErrorLog=analyzers.sarif

    - name: Upload SARIF report
      uses: actions/upload-artifact@v4
      with:
        name: analyzer-report
        path: analyzers.sarif

===============================
name: Code Quality Enforcement

on:
push:
branches: [ main ]
pull_request:
branches: [ main ]

jobs:
quality-check:
runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build with analyzers (full rebuild)
      run: dotnet build --no-incremental

    - name: Verify formatting and style
      run: dotnet format --verify-no-changes --severity error

    - name: Upload SARIF report (optional)
      run: dotnet build -p:ErrorLog=analyzers.sarif

    - name: Archive SARIF report
      uses: actions/upload-artifact@v4
      with:
        name: analyzer-report
        path: analyzers.sarif



dotnet build -v:diag
// Applies code style and formatting rules from
dotnet format --verify-no-changes 

// User secrets for local dev
<PropertyGroup>
  <UserSecretsId>your-app-guid-or-name</UserSecretsId>
</PropertyGroup>
dotnet user-secrets set "key1" "sec1"

// coverlet.runsettings
dotnet test --settings coverlet.runsettings
````
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <!-- Output format -->
          <Format>cobertura</Format>

          <!-- Output directory -->
          <OutputDirectory>TestResults\Coverage</OutputDirectory>

          <!-- Include only specific assemblies -->
          <Include>
            <ModulePath>.*MyApp\.dll$</ModulePath>
          </Include>

          <!-- Exclude test projects and generated code -->
          <Exclude>
            <ModulePath>.*Tests\.dll$</ModulePath>
            <Attribute>GeneratedCodeAttribute</Attribute>
          </Exclude>

          <!-- Optional settings -->
          <SkipAutoProps>true</SkipAutoProps>
          <IncludeSource>true</IncludeSource>

          <!-- Coverage threshold -->
          <Threshold>
            <ThresholdType>line</ThresholdType>
            <ThresholdStat>total</ThresholdStat>
            <Value>80</Value>
          </Threshold>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
````

Comprehensive table that outlines the most common ways to pass configuration to a .NET application:

---

### 📋 Configuration Sources in .NET

| Method | Description | Example |
|--------|-------------|---------|
| **appsettings.json** | Default JSON file for structured config. Supports nesting and environment-specific overrides. | `appsettings.json`:<br>`{ "ApiKey": "abc123" }`<br>Access via:<br>`configuration["ApiKey"]` |
| **Environment Variables** | Overrides config using OS-level variables. Ideal for secrets and CI/CD. Uses `__` for nesting. | Bash:<br>`export MySettings__ApiKey="env-key"`<br>Access via:<br>`configuration["MySettings:ApiKey"]` |
| **User Secrets** | Local dev-only secure storage. Avoids committing secrets to source control. | CLI:<br>`dotnet user-secrets set "ApiKey" "secret123"`<br>Access via:<br>`configuration["ApiKey"]` |
| **Command-Line Arguments** | Pass config at runtime. Useful for CLI tools or containerized apps. | Run:<br>`dotnet run --ApiKey=cmd-key`<br>Access via:<br>`configuration["ApiKey"]` |
| **launchSettings.json** | Used in Visual Studio for local dev. Injects env vars during debugging. | `launchSettings.json`:<br>`"environmentVariables": { "ApiKey": "dev-key" }`<br>Access via:<br>`configuration["ApiKey"]` |
| **Azure App Configuration / Key Vault** | Centralized config and secrets for cloud apps. Supports dynamic refresh. | Azure:<br>Bind `ApiKey` from Key Vault<br>Access via:<br>`configuration["ApiKey"]` |
| **Custom Configuration Providers** | Extend `IConfigurationProvider` to load from custom sources (e.g., database, XML, etc.) | Custom provider loads from DB:<br>`configuration["FeatureFlags:EnableBeta"]` |
| **.runsettings File (for Tests)** | Injects env vars and test settings during `dotnet test`. Not used in runtime app config. | `env.runsettings`:<br>`<EnvironmentVariables><ApiKey>test-key</ApiKey></EnvironmentVariables>` |

---

### 🧩 Best Practices for Team Governance

- Use **appsettings.json** for defaults and structure
- Override with **env vars** or **user secrets** for sensitive values
- Document expected keys in your **onboarding wiki**
- Use `.editorconfig` and `.runsettings` to align test and runtime environments
- Consider a **strongly typed options class** for grouped settings like `DatabaseSettings`
