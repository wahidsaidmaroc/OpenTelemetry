# DemoConsole - OpenTelemetry

Application console .NET 10 montrant les trois signaux OpenTelemetry :

- **Traces** : une operation `DemoOperation`, avec sa duree et son statut.
- **Metriques** : un compteur `demo.operations`, incremente une fois.
- **Logs** : deux messages correles a la trace active.

Tout est exporte dans le terminal. Aucun serveur, collecteur ou conteneur
n'est necessaire. Cet export console sert a apprendre et a verifier
l'instrumentation, pas a stocker la telemetrie en production.

## 1. Prerequis

Installer le SDK .NET 10, puis verifier sa presence :

```powershell
dotnet --list-sdks
```

## 2. Creer le projet

Depuis le dossier `ProjetDotNet` (etapes de reproduction ; le projet est deja cree) :

```powershell
dotnet new console --name DemoConsole --output .\DemoConsole --framework net10.0
Set-Location .\DemoConsole
```

## 3. Installer OpenTelemetry

```powershell
dotnet add package OpenTelemetry.Exporter.Console --version 1.20.1
```

Ce package apporte aussi le SDK `OpenTelemetry` via ses dependances.
La reference est enregistree dans [DemoConsole.csproj](DemoConsole.csproj).

## 4. Configurer l'instrumentation

L'implementation complete se trouve dans [Program.cs](Program.cs) :

1. Definir le nom du service (`DemoConsole`) et sa version (`1.0.0`).
2. Creer une `ActivitySource` pour les traces et un `Meter` pour les metriques.
3. Construire un `TracerProvider` avec `Sdk.CreateTracerProviderBuilder()`,
   enregistrer la source avec `AddSource(serviceName)` et activer
   `AddConsoleExporter()`.
4. Construire un `MeterProvider` avec `Sdk.CreateMeterProviderBuilder()`,
   enregistrer le meter avec `AddMeter(serviceName)` et activer
   `AddConsoleExporter()`.
5. Creer une `LoggerFactory`, appeler `AddOpenTelemetry()` et ajouter
   l'exporteur console pour les logs.
6. Associer chaque provider a une ressource contenant le meme nom de service
   grace a `ResourceBuilder.CreateDefault().AddService(...)`.

Les noms passes a `AddSource` et `AddMeter` doivent correspondre a ceux de
`ActivitySource` et `Meter`, sinon les signaux correspondants ne seront pas collectes.

## 5. Produire de la telemetrie

Le programme :

1. Cree le compteur `demo.operations` avec `CreateCounter<int>()`.
2. Demarre une activite avec `StartActivity("DemoOperation")`.
3. Ajoute le tag `demo.operation = say_hello`.
4. Ecrit des logs avec `LogInformation()` pendant l'activite :
   leurs `TraceId` et `SpanId` permettent de les relier a la trace.
5. Affiche un message, incremente le compteur avec `Add(1)` et marque
   l'activite comme reussie avec `SetStatus(ActivityStatusCode.Ok)`.

Les blocs `using` liberent les ressources en fin d'execution. L'activite est
fermee avant les providers pour exporter la trace ; le provider de metriques
effectue sa collecte finale a sa fermeture, meme si le programme est tres court.

## 6. Compiler et executer

Depuis le dossier `DemoConsole` :

```powershell
dotnet build
dotnet run --no-build
```

Pour lancer le projet directement depuis `ProjetDotNet` :

```powershell
dotnet run --project .\DemoConsole\DemoConsole.csproj
```

### Resultat attendu

- Le message `Bonjour depuis DemoConsole avec OpenTelemetry !`.
- Deux logs avec leurs identifiants de trace et de span.
- Une trace nommee `DemoOperation` avec le statut `Ok`.
- Une metrique `demo.operations` avec une valeur de `1`.
- Le service `DemoConsole` dans les ressources exportees.

Les identifiants, dates et durees changent a chaque execution.

### Compatibilite avec le controle d'applications Windows

Le projet definit `UseAppHost` a `false` : `dotnet run` lance la DLL avec
l'hote .NET installe plutot qu'un executable natif genere. Cela evite le
blocage de cet executable rencontre avec la strategie de controle
d'applications de cet environnement.

## Pour aller plus loin

Pour envoyer les signaux vers un OpenTelemetry Collector ou un backend
d'observabilite, installer `OpenTelemetry.Exporter.OpenTelemetryProtocol`,
remplacer les exports console par `AddOtlpExporter()` et configurer le point
de terminaison OTLP du collecteur.

Documentation officielle : <https://opentelemetry.io/docs/languages/dotnet/>.
