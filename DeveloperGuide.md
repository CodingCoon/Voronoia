# Voronation – Developer Guide

Dieser Leitfaden enthält lokale Entwicklungsentscheidungen. Allgemeine Regeln werden aus [Shared Engineering](../shared-engineering-guidelines/engineering.md), [C#](../shared-engineering-guidelines/csharp.md) und [Unity](../shared-engineering-guidelines/unity.md) übernommen und dort gepflegt. Coding Agents lesen außerdem [AGENTS.md](AGENTS.md).

## Setup und Start

Maßgeblich sind [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt), [manifest.json](Packages/manifest.json) und [packages-lock.json](Packages/packages-lock.json). Dokumentationsstand: Unity 6000.6.0f1; die Umstellung von 2023.2.3f1 wurde begonnen, aber noch nicht durch einen hier dokumentierten Build bestätigt.

1. Git-Status prüfen und vorhandene Änderungen erfassen, insbesondere Pakete, Projektsettings und Asset-Metadaten.
2. Die Projektversion samt erforderlichen Windows-/WebGL-Buildmodulen in Unity Hub bereitstellen. Keine andere Versionslinie stillschweigend verwenden.
3. Projekt importieren lassen, Console prüfen und Paketänderungen bewusst zuordnen.
4. [Menu.unity](Assets/Scenes/Menu.unity) als regulären Einstieg öffnen. Spiel und Tutorial starten; Szenenfolge mit [EditorBuildSettings.asset](ProjectSettings/EditorBuildSettings.asset) abgleichen.
5. [Inkrement 00](Docs/Overhaul/00-unity-basis.md) für den ersten nachgewiesenen Ausgangsbuild abschließen. Erst dann auf diesen Stand als geprüft verweisen.

Ein direkter GameScene-Start kann durch Singleton-Fallbacks oder Inspectorwerte anders initialisieren. Solche Unterschiede diagnostizieren; nicht durch neue automatische Komponenten-Erzeugung verdecken.

## Basisbuild 00

Verbindlich ist Unity **6000.6.0f1** mit Windows- und WebGL-Buildmodul. Der Editor-Helfer `Assets/Editor/Increment00Build.cs` bietet **Voronation → Build → Windows Development** und **Voronation → Build → WebGL Development**. Beide bauen `Menu.unity` vor `GameScene.unity`, prüfen das erwartete Skripting-Backend und melden BuildReport-Ergebnis, Warnungen und Fehler. Windows verwendet x64/Mono/Development, WebGL IL2CPP/Development. Die API-Kompatibilität bleibt aus den Player Settings und wird im Log ausgegeben. Ausgaben liegen unter `Build/Increment00/Windows` und `Build/Increment00/WebGL`; `Build` ist ignoriert.

Für Batch-Builds den Editor auf **einer eigenen Projektkopie** ohne `Library` und `UserSettings` starten, wenn das Arbeitsprojekt bereits im Editor geöffnet ist. In PowerShell:

```powershell
$sourceRoot = 'E:/6. Unity/Voronoia'
$projectCopy = 'E:/6. Unity/Voronoia/Build/Increment00/CleanCopy'
New-Item -ItemType Directory -Force -Path $projectCopy | Out-Null
Copy-Item -LiteralPath "$sourceRoot/Assets", "$sourceRoot/Packages", "$sourceRoot/ProjectSettings" -Destination $projectCopy -Recurse -Force
$unityExe = 'F:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Unity.exe'
& $unityExe -batchmode -nographics -projectPath $projectCopy -buildTarget StandaloneWindows64 -executeMethod Increment00Build.Windows -logFile 'E:/6. Unity/Voronoia/Build/Increment00/windows-build.log' -quit
& $unityExe -batchmode -nographics -projectPath $projectCopy -buildTarget WebGL -executeMethod Increment00Build.WebGL -logFile 'E:/6. Unity/Voronoia/Build/Increment00/webgl-build.log' -quit
```

Für einen echten zweiten Kaltstart eine **neue** Kopie ohne vorhandene `Library`, `UserSettings` oder alte Buildausgaben verwenden; `Copy-Item` nicht als Bereinigung einer bereits benutzten Kopie verstehen. Die Befehle nacheinander ausführen und Exitcode, BuildReport und Logs prüfen. Für WebGL im Ausgabeordner einen lokalen HTTP-Server starten und `index.html` über `http://localhost` in Edge und Firefox öffnen. Die Dateien nicht über `file://` prüfen. Falls komprimierte Dateien ausgeliefert werden, muss der Server passende Content-Encoding-Header setzen oder Unitys Dekompressions-Fallback verwendet werden.

Editor- und Development-Builds protokollieren `Basis00`: Szenenstart-Speicher, Rundendauer ab APPLY bis Ende der Todesanimation, Geometrieberechnung und Zellzuweisung. Für vergleichbare Werte die gleiche Zugfolge dreimal bei gleicher Ritterzahl verwenden; gerenderte und nicht gerenderte Läufe getrennt ausweisen.

## Lokale Code- und Assetkonventionen

| Thema | Entscheidung für dieses Projekt |
| --- | --- |
| Terminologie | Neue Spielertexte verwenden konsistent Ritter/Fraktion; historische Klassen/APIs bleiben bis zu einer fachlich begründeten Änderung erhalten. |
| Namespaces | Bestehender eigener Code ist überwiegend global. Keine flächendeckende Namespace-Migration; neue Subsystemgrenzen im jeweiligen Inkrement festlegen und dokumentieren. |
| Formatierung | Vorhandenen Stil im betroffenen Bereich erhalten. Kein festgelegtes universelles Feldpräfix oder ScriptableObject-Suffix ergänzen. |
| Unity-Referenzen | Pflichtreferenzen an Szene/Prefab ausdrücklich zuweisen; Konfigurationsfehler sichtbar machen. Bestehende Singleton-Fallbacks sind technische Schuld, keine Vorlage. |
| Serialisierung | Asset-GUIDs und Referenzen erhalten; Feld-/Typumbenennungen auf bestehende Daten prüfen und nötige Migration mitliefern. |
| Spielregeln | Fachliche Werte beim zuständigen Modell beziehungsweise Regelservice ändern; Anzeigen, KI und Auswertung verwenden dieselbe Regelquelle. |
| Frameworks | DOTween ist seit 01 gezielt für Animationen eingebunden; siehe [Dependencies](Docs/Dependencies.md). Kein pauschaler Bedarf an DI-Container, Odin, ECS oder eigener Event-Bus-Infrastruktur. |

Details und Zuständigkeiten stehen in [Architecture.md](Architecture.md). Eine Änderung an einem Vertrag zieht die dortige Ist-Beschreibung nach; allgemeine Stilregeln werden nicht hier dupliziert.

## Tooling und Abhängigkeiten

Voronoia hat bisher keine eigene `.editorconfig` und ist kein Teilnehmer des zentralen Formatterexports. [Shared Tooling](../shared-engineering-guidelines/tooling.md) beschreibt die bewusste Opt-in-Regel; [readability.editorconfig](../shared-engineering-guidelines/profiles/readability.editorconfig) ist eine mögliche Quelle, derzeit kein aktiver Projektvertrag. Das Sync-Skript nicht beiläufig ausführen und keine C#-Massenformatierung starten.

Neue Pakete nur für einen konkreten Arbeitsschritt übernehmen. Zweck, exakte Version, Lizenz, Referenzeinbindung und Windows-/WebGL-Prüfung dokumentieren. Bei Geometriebibliotheken zusätzlich Präzision und Ausgabeverträge prüfen. Es gibt keinen Beschluss, Clipper2 oder NetTopologySuite bereits einzubauen.

`Library`, `Temp`, `obj`, generierte `.sln`/`.csproj` und Paketcache sind keine manuell gepflegten Quellen. Paket- und Projekteinstellungen über den vorgesehenen Unity-/Paketweg ändern und resultierende Diffs prüfen. Benötigte `.meta`-Dateien gehören zu ihren Assets.

## Umsetzung eines Inkrements

### Unity-Authoring aus 01

Die folgenden Zuweisungen sind bereits in den Assets gespeichert; es ist kein manueller Setup-Schritt erforderlich. `Increment01Assets.Apply` ist der Editor-Helfer zur reproduzierbaren Erstellung, kein Laufzeit-Reparaturpfad.

- `Assets/Prefabs/Leader.prefab`: `KnightNumber` enthält TextMeshPro; `Leader.numberLabel` und `PreacherKnob.numberLabel` verweisen darauf. Auf der Gebietskomponente ist `SpriteShapeController.autoUpdateCollider` deaktiviert, damit `PreacherArea` die echten Polygonpunkte kontrolliert.
- `Assets/Scenes/GameScene.unity`: `Game` besitzt Referenzen auf `MatchStatusPanel`, Auswahl und Aktionsplanung. `StartPhase`/`DeathPhase` besitzen den VoronoiController; `ActionPhase`/`EvaluatePhase`/`DeathPhase` das EvaluationPanel. Die vorhandene `Blend`-Instanz ist aktiv für Szenenwechsel.
- `MatchStatus`/`ResultOverlay`: Canvas auf Sorting Layer UI, Reihenfolge 100; `MatchStatusPanel` besitzt Panel-, Titel-, Beschreibung- und beide Buttonreferenzen. Neue Partie und Menü sind verdrahtet.

Noch manuell prüfen: GameScene über Menu öffnen, Ritter-IDs auch zweistellig ansehen, Ergebnis- und Fehlerpanel bei üblicher Fenstergröße prüfen, beide Buttons betätigen und Tutorial vollständig durchlaufen. Die Editorprüfungen und offenen Plattformnachweise stehen in [01](Docs/Overhaul/01-akute-fehler.md).

### Allgemeiner Ablauf

1. [Roadmap](Docs/Overhaul/README.md), konkrete Inkrementbeschreibung und [Entscheidungen](Docs/Overhaul/ENTSCHEIDUNGEN.md) lesen.
2. Ausgangszustand und betroffenen Fehler/Anwendungsfall nachvollziehen. Offene Regelvorschläge von bereits beschlossenem Verhalten unterscheiden.
3. Kleinste spielbare Änderung umsetzen. Bei größeren Inkrementen die vorgesehenen Teilstücke separat prüfen.
4. Geeignete Prüfungen nach [Testing](Docs/Testing.md) ausführen und tatsächliche Ergebnisse festhalten.
5. Falls Szene/Prefab angepasst werden muss, Zuordnung von Asset, GameObject, Komponente und Inspectorfeld dokumentieren. Einen kurzen manuellen Prüfweg mitliefern.
6. Inkrementstatus und Nachweise aktualisieren. Offene Prüfungen ausdrücklich offen lassen.

### Unity-Authoring aus 02

Für 02 sind keine neuen Inspector-Zuweisungen erforderlich. `Game` auf dem GameObject **Game** bleibt der Kompositionspunkt und erzeugt `MatchSession`, Resolver und den datenbasierten Zellrechner aus den vorhandenen Setup-/Mapdaten. Die bestehenden Referenzen an `StartPhase`, `ApplyPhase`, `VoronoiPhase`, `EvaluatePhase`, `DeathPhase`, `Leader.prefab` und `Voronation.prefab` bleiben erhalten.

Nach einem Unity-Import müssen die neuen Assemblies `Voronation.Core`, `Voronation.Geometry` und `Voronation.Core.Tests` ohne Fehler erscheinen. Manuell von `Menu.unity` starten und Bewegung/Teilen nahe der Zellgrenze prüfen: Die Vorschau bleibt mindestens 0,05 Welteinheiten innerhalb und wird bei einem ungültigen Teilungsziel am Elternritter ausgeblendet. Anschließend eine Runde bis zur nächsten Planung, einen Ritterverlust, Ergebnisanzeige, Neustart und das vollständige Tutorial prüfen.

Keine Testsuite wird allein für eine Markdown- oder kosmetische Änderung aufgebaut. Änderungen an Geometrie, Wirtschaft und Rundenübergängen brauchen dagegen nachvollziehbare Regressionen. Die genaue lokale Prüfpolitik steht ausschließlich in der Testanleitung.

## Dokumentationszuständigkeit

- [README](README.md): Einstieg und dokumentierter Produktstand.
- [AGENTS](AGENTS.md): Leseauftrag und lokale Agent-Anweisungen.
- [Architecture](Architecture.md): tatsächliche Systemgrenzen und gekennzeichnete Migrationsziele.
- [Testing](Docs/Testing.md): Nachweise und Prüfwege.
- [Inkremente](Docs/Overhaul/README.md): Umfang, Abnahme und Umsetzungsstatus.
- [Entscheidungen](Docs/Overhaul/ENTSCHEIDUNGEN.md): dauerhafte Beschlüsse mit Folgen und Begründung.

Die Shared-Baseline bleibt außerhalb dieses Projekts. Projektarbeit ändert weder ihre Regeln noch globale Entwicklungswerkzeuge automatisch.
