# 02 – Spielkern und Rundenablauf

Status: implementiert; Kern gezielt geprüft (28.09.2026), Unity-Test-Runner, PlayMode und Playerbuilds offen. Voraussetzung: 01. Umfang: groß.

Codeanalyse und damaliger Migrationsplan vom 28.09.2026: [Umsetzungsplan 02](02-spielkern-plan.md). Der Plan dokumentiert den Stand vor der Umsetzung; der aktuelle Status und die Nachweise stehen in diesem Dokument. Dauerhafte Regelbeschlüsse stehen in [ENTSCHEIDUNGEN](ENTSCHEIDUNGEN.md).

## Ergebnis

Eine Runde wird aus einem Spielzustand und gültigen Befehlen berechnet. Unity zeigt das Ergebnis und seine Übergänge an; Animationen bestimmen keine Spielregeln.

## Schritte

- [x] 02a: Einfache C#-Daten für Match, Fraktion, Ritter und stabile IDs einführen. Position, Stärke, Einkommen, Alter, Geld und Rundennummer gehören in den Zustand, nicht ausschließlich in Transform/MonoBehaviour.
- [x] 02b: Aktionen als Datenbefehle abbilden: Besitzer, Ritter-ID, Aktion, optional Zielposition. Gemeinsame Validierung mit strukturiertem Ablehnungsgrund vorsehen.
- [x] 02c: Rundenauflöser aufbauen: Pläne prüfen → Aktionen auf Arbeitskopie anwenden → Zellen berechnen → Wirtschaft auswerten → Verluste und Sieg prüfen → Ergebnis übernehmen.
- [x] Neue Ritter, gleichzeitige Zielpositionen und entfernte Fraktionen über stabile Regeln behandeln. Ein neu erzeugter Ritter bekommt in derselben Planungsrunde keinen zusätzlichen Zug.
- [x] 02d: `RoundResult` mit Vorher-/Nachherzustand, Kostenbuchungen, Gebietsdeltas und Ereignissen erzeugen. UI, Sound und Log lesen dasselbe Ergebnis.
- [x] Fachereignisse wie ActionPlanned, RoundResolved, KnightRemoved und MatchEnded explizit definieren. Kein allgemeines Event-Bus-Framework ohne konkreten Bedarf.
- [x] Phasen und erlaubte Übergänge zentralisieren. Berechnung genau einmal übernehmen; fehlerhafte Arbeitskopie verwerfen. Anzeige kann abgeschlossen oder übersprungen werden.
- [x] Zufallsquelle, Seed und stabile Verarbeitungsreihenfolge injizierbar machen. Reproduzierbarkeit innerhalb derselben Version testen; noch keine plattformübergreifende Bitgleichheit behaupten.

## Abnahme

- [x] Derselbe Zustand plus dieselben Befehle liefert bei festem Seed dasselbe fachliche Ergebnis.
- [x] Eine Runde lässt sich ohne Kamera, Animation und Audio berechnen.
- [x] Ein Fehler während der Berechnung verändert den zuvor gültigen Zustand nicht.
- [x] Doppelte Übergänge/Bestätigungen werden abgewiesen.
- [x] Ritter- oder Fraktionslisten dürfen während ihrer Auswertung nicht unkontrolliert mutieren.

## Abgrenzung und Einstiegspunkte

Bestehende Ansichten mit Adaptern weiterverwenden. Keine ECS-Umstellung und kein Netzwerkpaket nötig. So klein refaktorieren, dass nach jedem Teilstück das Spiel noch startet.

Einstieg: `Assets/Scripts/Data Model`, `Actions`, `Phases`, `Preacher`, `voronoi/VoronoiController.cs`.

## Umsetzungsnachweis

Umgesetzt am 28.09.2026 auf dem vorhandenen stark uncommitteten Stand aus 01:

- `Voronation.Core` besitzt Zustand, IDs, Befehle, Validator, Sitzung, injizierbare Zufallsquelle, `RoundResolver` und unveränderliche Ergebnisdaten. Die Verarbeitung erfolgt nach Fraktions-/Ritter-ID auf einer tiefen Arbeitskopie. Erst ein vollständig gültiges Ergebnis ersetzt den Sitzungszustand.
- `Voronation.Geometry` enthält einen datenbasierten Rechner für die bisherige gewichtete Trennregel. Der mathematische Austausch bleibt 03. Abrechnungszellen werden vor Verlusten, endgültige Planungszellen gegebenenfalls nach Verlusten berechnet.
- Unitys bestehende Phasen stellen das bereits berechnete Ergebnis dar. Aktionen, VFX, Sound, SpriteShape und Collider schreiben keine Spielregel mehr. Setup, KI und menschliche Eingabe erzeugen Befehle über denselben Plan-/Validierungspfad.
- Bewegung und Teilen verwenden das zu Rundenbeginn gültige eigene Gebiet. Die Zielvorschau hält 0,05 Welteinheiten Abstand zur Grenze; Teilen zusätzlich zum Elternritter. Kosten werden weiterhin gemeinsam mit dem Rundeneinkommen abgerechnet, ohne Reservierung aus 04 vorzuziehen.
- `RoundResult` enthält Vorher-/Nachherzustand, Abrechnungszellen, Buchungen, Gebietsdeltas und geordnete Ereignisse. Auswertung und Sound lesen dieses Ergebnis. `ActionPlanned` und `RoundResolved` werden direkt von der Sitzung veröffentlicht; kein allgemeiner Event-Bus.

Tatsächlich ausgeführt:

| Prüfung | Ergebnis |
| --- | --- |
| Unity-Roslyn-Kompilierung der Kern-/Geometriequellen, `Assembly-CSharp` und `Assembly-CSharp-Editor` mit den vom Projekt erzeugten Response-Dateien | **Bestanden**, keine Compilerfehler. Bestehende Warnungen betreffen unter anderem historische `new`-Felder, Singleton-Suche und Development-Build-Präprozessoren. |
| Zehn gezielte Kernregressionen, mit Unitys Compiler gebaut und unter Unitys Mono ohne Szene/Kamera/Audio ausgeführt | **Bestanden**: Reproduzierbarkeit, Teilung/Kindzug, Zielrand/Elternabstand, Konfliktablehnung während der Planung, Fehler vor und nach Abrechnung, Insolvenz/Gleichstand, genau ein ältester Verlust, monotone IDs, kanonische Reihenfolge und doppelte Auflösung. |
| Unity Test Runner in isolierter Projektkopie | **Nicht ausführbar**: Der Batch-Editor verlor wiederholt die Verbindung zum lokalen Unity-Lizenzkanal und blieb während des initialen Asset-Refresh hängen. Der Prozess wurde jeweils gezielt beendet; kein Testergebnis wurde erzeugt. |

Noch offen sind der automatisierte PlayMode-Ablauf, vollständiges Tutorial, manuelle Bedienung der Zielgrenze sowie Windows-x64/Mono- und WebGL/IL2CPP-Development-Builds. Diese offenen Integrations-/Plattformnachweise ändern die bestandenen reinen Kernregressionen nicht, verhindern aber eine vollständige Unity-/Playerabnahme des Inkrements.
