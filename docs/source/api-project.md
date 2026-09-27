# API project and startup

Project file, configuration, launch profiles, tools, HTTP examples, and application bootstrap.

This page contains **complete file contents** for 7 files. Copy each block to the exact path shown. The code is embedded in this Markdown page and remains visible when the sample source directory is unavailable.

Return to [all source files](/source/) or read [setup instructions](/start). The [source bundle](/sample-source.zip) also includes binary icons and license files.

## `sample/EmployeeManagement.Api/appsettings.Development.json`

**File:** `sample/EmployeeManagement.Api/appsettings.Development.json` — **Use:** Supplies application settings for this environment; development values are for local practice.

```json
{
  "ConnectionStrings": {
    "EmployeeManagement": "Server=.\\SQLEXPRESS;Database=EmployeeManagementInterviewDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "Jwt": {
    "Issuer": "EmployeeManagement.Api",
    "Audience": "EmployeeManagement.Angular",
    "Key": "development-only-signing-key-change-before-any-real-deployment-2026"
  },
  "DemoAdmin": {
    "Username": "admin",
    "Password": "Admin123!"
  },
  "DemoViewer": {
    "Username": "viewer",
    "Password": "Viewer123!"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## `sample/EmployeeManagement.Api/appsettings.json`

**File:** `sample/EmployeeManagement.Api/appsettings.json` — **Use:** Supplies application settings for this environment; development values are for local practice.

```json
{
  "ConnectionStrings": {
    "EmployeeManagement": ""
  },
  "Jwt": {
    "Issuer": "EmployeeManagement.Api",
    "Audience": "EmployeeManagement.Angular",
    "Key": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## `sample/EmployeeManagement.Api/dotnet-tools.json`

**File:** `sample/EmployeeManagement.Api/dotnet-tools.json` — **Use:** Pins the local EF Core command-line tool.

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-ef": {
      "version": "8.0.31",
      "commands": [
        "dotnet-ef"
      ],
      "rollForward": false
    }
  }
}
```

## `sample/EmployeeManagement.Api/EmployeeManagement.Api.csproj`

**File:** `sample/EmployeeManagement.Api/EmployeeManagement.Api.csproj` — **Use:** Defines the .NET target framework and NuGet dependencies.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.31" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.31">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.31" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
  </ItemGroup>

</Project>
```

## `sample/EmployeeManagement.Api/EmployeeManagement.Api.http`

**File:** `sample/EmployeeManagement.Api/EmployeeManagement.Api.http` — **Use:** Provides runnable API request examples.

```http
@baseUrl = http://localhost:5087/api
@token = paste-token-from-login-response-here

### Login
POST {{baseUrl}}/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}

### Paged employee list
GET {{baseUrl}}/employees?search=&page=1&pageSize=10&sortBy=name&sortDirection=asc
Authorization: Bearer {{token}}

### Lookups
GET {{baseUrl}}/departments/lookup
Authorization: Bearer {{token}}

### Stored procedure report
GET {{baseUrl}}/dashboard/department-counts
Authorization: Bearer {{token}}
```

## `sample/EmployeeManagement.Api/Program.cs`

**File:** `sample/EmployeeManagement.Api/Program.cs` — **Use:** Configures services, middleware, routes, and development database migration.

```csharp
using System.Text;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EmployeeManagement")
    ?? throw new InvalidOperationException("The EmployeeManagement connection string is missing.");
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("The JWT signing key is missing.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<DatabaseInitializer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee Management API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }] = Array.Empty<string>()
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options => options.AddPolicy("AngularClient", policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors("AngularClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().InitializeAsync();
}

app.Run();

public partial class Program;
```

## `sample/EmployeeManagement.Api/Properties/launchSettings.json`

**File:** `sample/EmployeeManagement.Api/Properties/launchSettings.json` — **Use:** Defines local HTTP and HTTPS launch profiles.

```json
﻿{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:58605",
      "sslPort": 44360
    }
  },
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5087",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7087;http://localhost:5087",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```
