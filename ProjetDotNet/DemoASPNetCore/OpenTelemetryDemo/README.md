# OpenTelemetryDemo — Observabilité avec ASP.NET Core, SQL Server et OpenTelemetry

Projet pédagogique de démonstration montrant comment utiliser **OpenTelemetry** pour observer :

- les requêtes HTTP
- les appels à SQL Server (Entity Framework Core)
- les temps d'exécution
- les erreurs (HTTP 500)
- les traces distribuées (TraceId / SpanId)
- les opérations lentes
- les spans personnalisés

> 📝 Ce README documente pour l'instant les **étapes d'installation**. La partie pédagogique complète (observabilité, traces, Jaeger, exercices) sera ajoutée à l'ÉTAPE 15.

---

## Prérequis

| Outil | Version | Vérification |
|---|---|---|
| .NET SDK | 9.0 ou supérieur | `dotnet --version` |
| Docker Desktop | récent | `docker --version` |
| SQL Server | via Docker (étape 13) | — |

---

## ÉTAPE 1 — Création de la solution et du projet

```powershell
# Créer la solution
dotnet new sln -n OpenTelemetryDemo

# Créer le projet Web API (.NET 9, avec contrôleurs classiques)
dotnet new webapi -n OpenTelemetryDemo -f net9.0 --use-controllers

# Ajouter le projet à la solution
dotnet sln add .\OpenTelemetryDemo\OpenTelemetryDemo.csproj
```

Fichiers du template supprimés (non nécessaires) :

- `WeatherForecast.cs`
- `Controllers\WeatherForecastController.cs`

Structure cible du projet :

```
OpenTelemetryDemo
│
├── Controllers
│   └── ProductsController.cs
│
├── Data
│   └── AppDbContext.cs
│
├── Models
│   └── Product.cs
│
├── Services
│   └── ProductService.cs
│
├── Telemetry
│   └── TelemetrySources.cs
│
├── Program.cs
├── appsettings.json
├── docker-compose.yml
└── README.md
```

---

## ÉTAPE 2 — Installation des packages NuGet

Toutes les commandes s'exécutent **dans le dossier du projet** (`OpenTelemetryDemo`).

### 2.1 Entity Framework Core + SQL Server

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### 2.2 Swagger / OpenAPI

```powershell
dotnet add package Swashbuckle.AspNetCore
```

### 2.3 OpenTelemetry

```powershell
# Intégration avec l'hôte ASP.NET Core : fournit AddOpenTelemetry()
dotnet add package OpenTelemetry.Extensions.Hosting

# Instrumentation automatique des requêtes HTTP entrantes (ASP.NET Core)
dotnet add package OpenTelemetry.Instrumentation.AspNetCore

# Instrumentation automatique des appels HttpClient sortants
dotnet add package OpenTelemetry.Instrumentation.Http

# Instrumentation automatique d'Entity Framework Core (spans SQL)
# ⚠️ package encore en préversion, d'où --prerelease
dotnet add package OpenTelemetry.Instrumentation.EntityFrameworkCore --prerelease

# Métriques du runtime .NET (GC, threads, allocations…)
dotnet add package OpenTelemetry.Instrumentation.Runtime

# Exporter Console : affiche les traces directement dans le terminal (pédagogique)
dotnet add package OpenTelemetry.Exporter.Console

# Exporter OTLP : envoie les traces vers l'OpenTelemetry Collector / Jaeger
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

### 2.4 Récapitulatif des packages installés

| Package | Version | Rôle |
|---|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.22 | Accès SQL Server via EF Core |
| `Swashbuckle.AspNetCore` | 10.3.1 | Interface Swagger UI |
| `OpenTelemetry.Extensions.Hosting` | 1.20.1 | Point d'entrée `AddOpenTelemetry()` |
| `OpenTelemetry.Instrumentation.AspNetCore` | 1.20.1 | Traces + métriques HTTP entrantes |
| `OpenTelemetry.Instrumentation.Http` | 1.20.1 | Traces HttpClient sortantes |
| `OpenTelemetry.Instrumentation.EntityFrameworkCore` | 1.20.1-beta.1 | Spans des requêtes SQL EF Core |
| `OpenTelemetry.Instrumentation.Runtime` | 1.20.1 | Métriques runtime .NET |
| `OpenTelemetry.Exporter.Console` | 1.20.1 | Export des traces dans le terminal |
| `OpenTelemetry.Exporter.OpenTelemetryProtocol` | 1.20.1 | Export OTLP vers Collector / Jaeger |

Vérifier l'installation :

```powershell
dotnet list package
```

---

## ÉTAPE 3 — Modèle `Product` et `AppDbContext`

### 3.1 Entité `Product` (`Models\Product.cs`)

| Propriété | Type |
|---|---|
| `Id` | `int` |
| `Name` | `string` |
| `Description` | `string` |
| `Price` | `decimal` |
| `Stock` | `int` |
| `CreatedAt` | `DateTime` |

### 3.2 `AppDbContext` (`Data\AppDbContext.cs`)

- `DbSet<Product> Products`
- Configuration du modèle : `HasMaxLength`, `HasPrecision(18, 2)` pour `Price`
- **Seed automatique de 6 produits de démonstration** via `HasData()`, dont
  *« PC Portable Dell XPS 15 »* et *« PC Fixe Gamer »* pour que la recherche
  `GET /api/products/search/PC` retourne des résultats.

> 💡 Les dates du seed sont **statiques** (pas de `DateTime.UtcNow`) car
> EF Core exige des valeurs déterministes dans `HasData()`.

### 3.3 Vérifier la compilation

```powershell
dotnet build
```

Résultat attendu : `La génération a réussi. 0 Avertissement(s), 0 Erreur(s)`.

---

## Étapes suivantes

| Étape | Description | Statut |
|---|---|---|
| 1 | Créer la solution et le projet | ✅ |
| 2 | Installer les packages NuGet | ✅ |
| 3 | Créer `Product` et `AppDbContext` | ✅ |
| 4 | Configurer SQL Server | ⏳ |
| 5 | Créer `ProductService` | ⏳ |
| 6 | Créer `ProductsController` | ⏳ |
| 7 | Tester l'API sans OpenTelemetry | ⏳ |
| 8 | Ajouter OpenTelemetry | ⏳ |
| 9 | Traces automatiques ASP.NET Core + EF Core | ⏳ |
| 10 | Spans personnalisés | ⏳ |
| 11 | Scénario lent (`search/{name}`) | ⏳ |
| 12 | Scénario d'erreur (`/error`) | ⏳ |
| 13 | Docker Compose (SQL Server) | ⏳ |
| 14 | Jaeger + OTLP | ⏳ |
| 15 | README pédagogique complet | ⏳ |
