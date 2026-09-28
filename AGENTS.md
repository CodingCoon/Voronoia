# Voronation – Anweisungen für Coding Agents

Diese Datei gilt für das gesamte Repository. Sie ergänzt die gemeinsamen Engineering-Guidelines um den lokalen Arbeitskontext. Sie ist kein Auftrag, alle geplanten Inkremente umzusetzen.

## Vor der Arbeit lesen

Die gemeinsame Baseline liegt als Geschwister-Checkout unter `../shared-engineering-guidelines` (auf diesem Rechner `E:/6. Unity/shared-engineering-guidelines`). Links sind kein automatisches Include: Die einschlägigen Dateien ausdrücklich öffnen und lesen.

1. [Shared README](../shared-engineering-guidelines/README.md) für Geltung und Einbindung.
2. [Engineering](../shared-engineering-guidelines/engineering.md), bei C#-Arbeit zusätzlich [C#](../shared-engineering-guidelines/csharp.md), bei Unity-Arbeit zusätzlich [Unity](../shared-engineering-guidelines/unity.md).
3. [Agent-Workflow](../shared-engineering-guidelines/agents.md).
4. Diese Datei, [Developer Guide](DeveloperGuide.md) und die relevanten Teile von [Architecture](Architecture.md).
5. Das beauftragte [Inkrement](Docs/Overhaul/README.md) und seine Einträge in [Entscheidungen](Docs/Overhaul/ENTSCHEIDUNGEN.md).

Bei Formatierungs-/Analyzerarbeit zusätzlich [Tooling](../shared-engineering-guidelines/tooling.md) lesen. Die Analyseunterlagen der Baseline begründen die Regeln, sind keine zusätzliche Pflichtlektüre pro Aufgabe. Fehlt der Checkout, die Einschränkung melden; keine vermeintlich gelesenen Regeln behaupten. Unabhängige Arbeit mit vorhandenen Vorgaben fortsetzen.

## Quellen und Status

- Höher priorisierte Plattform- und aktuelle Nutzeranweisungen bleiben maßgeblich. Bewusste lokale Vorgaben konkretisieren Shared Defaults; Subsystemvorgaben gelten nur in ihrem Bereich.
- `Architecture.md` beschreibt Bestand und ausdrücklich gekennzeichnetes Zielbild. Inkremente beschreiben zukünftige Arbeit, keine bereits vorhandenen Funktionen.
- Beschlossene Gameplay-/Technikentscheidungen gehören in `Docs/Overhaul/ENTSCHEIDUNGEN.md`; dessen offene Vorschläge nicht als Beschluss behandeln.
- Bei einem Widerspruch mit wesentlichen Auswirkungen auf Spielregeln oder Architektur die konkrete Entscheidung klären und unabhängige Schritte weiterbearbeiten. Kleine Implementierungsentscheidungen innerhalb des Auftrags selbst treffen.

## Lokaler Arbeitsrahmen

- Zu Beginn Git-Status und tatsächliche Unity-/Paketversionen prüfen. Bestehende Projekt-, Paket- und Assetänderungen erhalten; keine Upgrade-Diffs pauschal zurücksetzen.
- Nur das beauftragte Verhalten oder Inkrement bearbeiten. Keine vorgezogenen Multiplayer-, Völker- oder Frameworkarbeiten und keine projektweiten Umbenennungen von Leader/Preacher/Religion.
- Ausgangspunkt sind die vorhandenen Szenen, Prefabs und Systeme. Die Ablösung der Geometrie und die Entkopplung der Regeln erfolgen in den dafür vorgesehenen Inkrementen.
- Clipper2, NetTopologySuite und das neue Input System sind derzeit Optionen beziehungsweise geplante Ergänzungen. Installation und Migration nicht aus einer Erwähnung in der Roadmap ableiten.
- Neue Abhängigkeiten im betroffenen Auftrag mit Zweck, Version, Lizenz und Zielplattformprüfung dokumentieren. Es gibt hier keine zusätzliche pauschale Freigabepflicht für jede Bibliothek.
- Keine allgemeine Formattermigration auslösen: Voronoia hat bisher nicht in den Shared-EditorConfig-Export optiert. Betroffenen lokalen Stil erhalten.
- Projektbezogene Verifikation richtet sich nach [Testing](Docs/Testing.md). Einen Textreview nicht als Build oder Spieltest ausgeben.

## Abschluss einer Änderung

Betroffene Verträge und Ist-Beschreibung nachziehen; Regeln nur an ihrer zuständigen Quelle pflegen. Bei Inkrementarbeit Status, erfüllte Abnahmepunkte und Nachweise in der Inkrementdatei aktualisieren.

Bericht: geändertes Verhalten, tatsächlich ausgeführte Prüfungen, verbleibende Grenzen. Falls Unity-Handarbeit offen bleibt, Szene/Prefab, GameObject, Komponente, Inspector-Feld und konkreten Prüfschritt nennen. Fehlende Referenzen nicht durch neue Such-/Reparaturpfade verbergen; siehe Shared U2/U3.
