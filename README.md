# Voronation

Ein rundenbasiertes Strategiespiel: Ritter bestimmen durch Position und Stärke ihre Gebiete. Gebiet liefert Einkommen, Aktionen und Ritter verursachen Kosten. Das Projekt entstand als Beitrag zum Acerola Jam 0 und wird für eine stabilere und verständlichere Version überarbeitet.

Spielname: **Voronation**. Projektverzeichnis: **Voronoia**. Im bisherigen Code heißen Ritter `Leader` oder `Preacher`, Fraktionen `Voronation` oder `Religion`.

## Dokumentation

| Datei | Zweck |
| --- | --- |
| [AGENTS.md](AGENTS.md) | Einstieg und lokale Arbeitsanweisungen für Coding Agents |
| [Architecture.md](Architecture.md) | Bestehende Systeme, Datenfluss, technische Grenzen und geplantes Zielbild |
| [DeveloperGuide.md](DeveloperGuide.md) | Unity-Setup, lokale Entwicklungsentscheidungen und Änderungsablauf |
| [Docs/Testing.md](Docs/Testing.md) | Prüfstrategie, Buildnachweise und bekannte Regressionen |
| [Überarbeitungsplan](Docs/Overhaul/README.md) | Reihenfolge der 17 Inkremente und lieferbare Zwischenstände |
| [Entscheidungen](Docs/Overhaul/ENTSCHEIDUNGEN.md) | Offene Regel-/Technikfragen und spätere Beschlüsse |

## Gemeinsame Engineering-Guidelines

Die allgemeine Baseline bleibt im Geschwister-Repository [shared-engineering-guidelines](../shared-engineering-guidelines/README.md). Wichtig sind [Engineering](../shared-engineering-guidelines/engineering.md), [C#](../shared-engineering-guidelines/csharp.md), [Unity](../shared-engineering-guidelines/unity.md), [Agent-Workflow](../shared-engineering-guidelines/agents.md) und [Tooling](../shared-engineering-guidelines/tooling.md).

Diese Regeln werden hier verlinkt, nicht kopiert. Auf einem anderen Rechner den Guidelines-Checkout neben das Projekt legen oder die Verweise bewusst anpassen.

## Projekt öffnen

1. Die in [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt) eingetragene Unity-Version mit den benötigten Buildmodulen verwenden. Dokumentierter Stand am 26.09.2026: **6000.6.0f1**; frühere Git-Version: 2023.2.3f1.
2. Den Projektordner über Unity Hub öffnen und Paket-/Assetimport abschließen lassen.
3. [Menu.unity](Assets/Scenes/Menu.unity) öffnen und von dort Spiel oder Tutorial starten. Direkter Einstieg in GameScene ist ein zusätzlicher Diagnosepfad, kein bereits bestätigter Startvertrag.
4. Setup- und Buildprüfung gemäß [Developer Guide](DeveloperGuide.md) durchführen.

## Aktueller Stand

Die Roadmap und Projektleitfäden sind angelegt. Inkrement 00 (Unity-Basis) ist in Arbeit; unter anderem sind bekannte Fehler in Geometrie, Wirtschaft und Ritter-/Fraktionsverwaltung offen. Der Build- und Spielteststand steht in [00](Docs/Overhaul/00-unity-basis.md).

Windows x64 als Entwicklungsbuild und Desktop-WebGL in Edge und Firefox sind die Prüfziele für Inkrement 00. Speichern, reaktive KI, animierte Zelländerungen und Multiplayer sind geplant, nicht als vorhandene Funktionen zugesagt.

[Veröffentlichtes Jam-Spiel](https://codingcoon.itch.io/voronation) · [Jam-Eintrag und Feedback](https://itch.io/jam/acerola-jam-0/rate/2582186)
