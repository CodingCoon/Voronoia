# Voronation – Architektur

Stand: 28.09.2026 nach Implementierung von Inkrement 02. Grundlage: Quellcode, Projektkonfiguration, gezielte Kernregressionen und statische Integrationsprüfung; PlayMode und aktuelle Playerbuilds sind noch nicht nachgewiesen. Nachweise stehen in den Inkrementdateien.

Allgemeine Gestaltungsregeln stehen in [Engineering](../shared-engineering-guidelines/engineering.md), [C#](../shared-engineering-guidelines/csharp.md) und [Unity](../shared-engineering-guidelines/unity.md). Dieses Dokument beschreibt ausschließlich Projektzusammenhänge. Arbeitsablauf: [Developer Guide](DeveloperGuide.md); Prüfungen: [Testing](Docs/Testing.md).

## 1. Fachlicher Kern

Eine Fraktion besitzt Geld und Ritter. Der Laufzeitzustand unter `Voronation.Core` hält Position, Stärke, Einkommensfaktor, Alter, Geld, Rundennummer, Kartenmaß und stabile IDs. Planbefehle liegen getrennt vom letzten gültigen Zustand. Vier aktive Züge verändern Position, Stärke, Einkommensfaktor oder Ritterzahl. Ein fehlender Befehl wird weiterhin als keine Aktion normalisiert; eine ausdrücklich wählbare Warten-Aktion ist für 04 geplant.

Die Gebietszellen werden aus allen Rittern berechnet. Flächen und Einkommensfaktor bestimmen Einnahmen; Aktionskosten und altersabhängiger Unterhalt gehen in die Abrechnung ein. Insolvenz führt zu Ritterverlust; Ausscheiden aller gegnerischen Fraktionen ist die beabsichtigte Siegbedingung. Die vorhandenen Implementierungsfehler sind keine alternativen Spielregeln.

## 2. Bestehende Szenen und Einstieg

- [Menu.unity](Assets/Scenes/Menu.unity) und [GameScene.unity](Assets/Scenes/GameScene.unity) stehen in dieser Reihenfolge in [EditorBuildSettings.asset](ProjectSettings/EditorBuildSettings.asset).
- [GameManager](<Assets/Scripts/Makro Layer/GameManager.cs>) koordiniert Spiel/Tutorial und Szenenwechsel über `ScreenBlend`.
- [Singleton](<Assets/Scripts/Makro Layer/Singleton.cs>) hält einen persistenten Manager und besitzt historische Such-/Erzeugungsfallbacks. Das ist ein Bestandsbefund, keine Empfehlung für weitere Systeme.
- [Game](<Assets/Scripts/Data Model/Game.cs>) registriert sich in Awake, komponiert nach dem Setup eine `MatchSession` samt Resolver und startet seine Präsentationsphasen in Start.
- [StartPhase](Assets/Scripts/Phases/StartPhase.cs) erzeugt die Fraktionen über TutorialSetup oder Random6PlayerSetup. Setup-Assets und Phasen sind über serialisierte Szenenreferenzen verbunden.
- `Game` bleibt der Unity-Kompositionspunkt. Die darin erzeugte Sitzung und der Rundenauflöser sind normale C#-Objekte; Szene, Prefabs und Eingabe bleiben lifecycle-abhängige Adapter.

## 3. Systemkarte des Bestands

Alle folgenden Pfade sind relativ zu `Assets/Scripts`.

| Bereich | Verantwortlichkeit und aktueller Zustand |
| --- | --- |
| `Makro Layer/` | Szenenwechsel, Menükoordination und persistenter GameManager |
| `Core/` | Match-/Fraktions-/Ritterzustand, stabile IDs, Befehle, Validierung, Sitzung, atomarer Resolver, Ereignisse und `RoundResult` |
| `Data Model/Game.cs` | Unity-Komposition, Zuordnung von IDs zu Views und Präsentationsphasen |
| `Data Model/impl/Voronation.cs` | Fraktionsview, KI-Zuordnung und Erzeugung/Entfernung von Ritterviews |
| `Data Model/impl/Leader.cs` | Ritterview, Auswahl, Polygonübergabe und Animation; Spiegelwerte für die sichtbare Übergangsphase |
| `Actions/` | UI-Beschreibungen der vier Aktionen; bestätigte Regeln werden als `ActionCommand` verarbeitet |
| `Phases/` | Sichtbare Abfolge des bereits berechneten `RoundResult`; keine Geld- oder Regelbuchung |
| `voronoi/` | Eigene gewichtete Trennlinien-/Schnittpunktberechnung und Übergabe der Polygone |
| `Preacher/` | Ritterdarstellung, Gebiet, Aktionsrad, Vorschau und Mausinteraktion |
| `Map/` | Zentrale Mausabfrage und aktuelle Ritterauswahl |
| `AI/` | Upgrade-Taktiken; im Standardsetup keine räumliche Bewegung oder Teilung |
| `Setups/` | Erzeugung von Fraktionen und Startpositionen; Standardsetup mit sechs Teilnehmern |
| UI-Skripte im Script-Root | Geld, Ritterdetails, Auswertung und Rundenschaltflächen |
| `Tutorial*`, `Tutorial/` | Geführte Aufgaben und Freischaltung der Bedienung |
| `SFX/`, `VFX/` | Soundzuordnung und visuelle Effekte |

Wichtige konfigurierte Assets: [Leader.prefab](Assets/Prefabs/Leader.prefab), [Voronation.prefab](Assets/Prefabs/Voronation.prefab), [VoronoiFade.prefab](Assets/Prefabs/VoronoiFade.prefab), [CCButton.prefab](Assets/Prefabs/CCButton.prefab).

Der eigene Laufzeitcode liegt überwiegend im globalen Namespace und in Unitys Standardassembly. Der neue Kern liegt in `Voronation.Core` unter dem Namespace `VoronationCore`; die datenbasierte Zellberechnung und Polygonhilfen liegen in `Voronation.Geometry`. Beide besitzen kleine EditMode-Testassemblys. DOTween besitzt seine Modulassembly; Version und Lizenz stehen in [Dependencies](Docs/Dependencies.md). Die übrigen Spielskripte wurden nicht pauschal in Assemblies verschoben.

## 4. Runden- und Datenfluss heute

```text
START: Setupdaten und Views erzeugen → MatchState bilden → Anfangszellen berechnen
  → ACTION: Mensch und KI schreiben validierte Befehle in denselben RoundPlan
  → APPLY-Eintritt: Resolver prüft den eingefrorenen Plan und berechnet auf Arbeitskopie
       Aktionen → Abrechnungszellen → Wirtschaft → Verluste/Ergebnis
       → gegebenenfalls endgültige Planungszellen → atomare Übernahme als RoundResult
  → APPLY/VORONOI/EVALUATION/DEATH: dasselbe Ergebnis animieren und anzeigen
  → ACTION oder GAME_OVER
```

Positionen, Werte, Flächen und Geld stammen fachlich aus `MatchState`. Der Zellrechner erhält nur IDs, Positionen, Stärke und Kartengrenze und liefert validierte Polygone samt Fläche. `Leader`, `Voronation`, SpriteShape und Collider bilden den Zustand beziehungsweise die zwei Gebietszeitpunkte aus `RoundResult` ab. Ein Fehler der ersten oder der nach Verlusten nötigen zweiten Zellberechnung verwirft die gesamte Arbeitskopie.

Eingabe erfolgt weiterhin über Legacy-Input, OnMouse-Callbacks und den eigenen MouseEventManager. Bewegung und Teilen werden auf die eigene Zelle am Planungsbeginn mit 0,05 Welteinheiten Innenabstand begrenzt. Mensch und KI verwenden `MatchSession.PlanAction` und denselben strukturierten Validator. `Game` sperrt Planung und Weiterschalten außerdem über Sitzungszustand, Rundenversion und Präsentationskennung.

Bewegung, Teilung, Aktionsrad, Einkommens- und Verlustanimationen verwenden komponenten- beziehungsweise phaseneigene DOTween-Tweens. Animationen lesen Befehle und Ereignisse, ändern aber keine Regelwerte. Ein unerwarteter Präsentationsabbruch löst keine zweite Berechnung oder Buchung aus. `MatchStatusPanel` zeigt Sieg, Niederlage, Unentschieden und kontrollierte Fehler weiterhin mit Neustart beziehungsweise Menüwechsel.

## 5. Bekannte technische Grenzen

- Ritter verwenden arabische TMP-Nummern und monoton steigende IDs innerhalb ihrer Fraktion. Fraktionsentfernung und Insolvenz sind gemäß Entscheidungsprotokoll korrigiert.
- Flächenformel, Colliderpunkte und vorläufige Geometriesicherungen sind korrigiert. Identische Positionen oder ungültige Polygone führen zu einem kontrollierten Fehler; vollständige geometrische Korrektheit bleibt Gegenstand von 03.
- Die Stärkeverhältnis-Grenzen können unabhängig von Implementierungsfehlern Lücken erzeugen. Ein Austausch der Polygonbibliothek allein repariert die Gewichtungsregel nicht.
- `RoundResult` ist ein verlässlicher abgeschlossener Rundenvertrag; persistente Savegames und ein benutzbares Rundenlog folgen dennoch erst in 06/07.
- Die Unity-Versionsdatei ist bereits aktualisiert. Der Erfolg der Migration folgt daraus nicht automatisch.

Für Inkrement 00 liegt der Buildzugang ausschließlich im Editor unter `Assets/Editor/Increment00Build.cs`. Die Messpunkte `Increment00Diagnostics` sind auf Editor- und Development-Builds begrenzt; sie lesen Phasenwechsel, Geometriearbeit und reservierten Speicher, ohne die Spielregeln zu verändern. GraphicsSettings und Quality-Stufen verwenden derzeit Built-in-Rendering. Die vorhandenen `UniversalRP`-/`Renderer2D`-Assets sind keinem aktiven Renderpipeline-Slot zugewiesen.

Konkrete Maßnahmen und Regressionen: [01 – Akute Fehler](Docs/Overhaul/01-akute-fehler.md) und [03 – Geometrie](Docs/Overhaul/03-geometrie-und-staerke.md).

## 6. Umgesetzte Kern- und Darstellungsgrenze

Die folgenden Grenzen wurden in [02 – Spielkern](Docs/Overhaul/02-spielkern.md) eingeführt. Spätere Inkremente erweitern Geometrie, Wirtschaft, Speicherung und Darstellung auf dieser Basis.

```text
Menschliche Eingabe / KI / später Netzwerk
                 │ Befehle
                 ▼
        Validierung und Rundenauflöser
          │ Geometrie / Wirtschaftsregeln
          ▼
  gültiger Spielzustand + RoundResult
          │                      │
          ▼                      ▼
   Speichern/Diagnose     UI, Zellanimation, SFX
```

- **Zustandsbesitz:** Ein Match-Zustand besitzt Fraktionen, Ritter, Geld, Rundennummer und stabile IDs. Views lesen ihn und senden Absichten; sie buchen kein Geld.
- **Auflösung:** Validierte Pläne werden auf einer Arbeitskopie verarbeitet. Erst ein gültiges Gesamtergebnis ersetzt den vorherigen Stand. Gleichzeitige Planung und Reihenfolge der Abrechnung werden ausdrücklich festgelegt.
- **Geometrie:** Positionen/Gewichte und Kartengrenze hinein, gültige oder explizit leere Zellen samt Flächen heraus. Keine Scene-/Prefab-Erzeugung im Rechner.
- **Darstellung:** Vorher-/Nachherzustand und Ereignisse bestimmen Animationen. Tempo und Überspringen ändern nicht die Regeln. Animierte Vorschauflächen werden nicht abgerechnet.
- **Konfiguration:** Authoring-Daten und live veränderlicher Match-Zustand bleiben getrennt. ScriptableObjects sind eine mögliche Quelle für Regelwerte, nicht der Speicher einer laufenden Partie.
- **Integration:** Bestehende Szenen/Prefabs dienen weiter als Unity-Komposition. Explizite Referenzen und Lifecycle-Cleanup statt zusätzlicher globaler Manager.

Reine C#-Regeln bedeuten hier keine pauschale Pflicht zu einer UnityEngine-freien Assembly. Werttypen können verbleiben, solange Regeln ohne Szene, Transform, Coroutine, Audio und Rendering prüfbar sind.

## 7. Noch offene Architekturentscheidungen

Power-Diagramm, Clipper2 versus Halbebenen-Clipper, Stärkeabbildung und Verdrängungsregel sind Vorschläge. Auch Saveformat, konkrete Input-Paketversion und Online-Transport sind noch nicht entschieden. Die kanonische Liste steht in [ENTSCHEIDUNGEN.md](Docs/Overhaul/ENTSCHEIDUNGEN.md).

Reproduzierbarkeit innerhalb einer festgelegten Version ist ein geplantes Testziel. Plattformübergreifende Bitgleichheit wird nicht zugesagt. Netzwerk, Völker und alternative Siegbedingungen rechtfertigen jetzt keine vorsorglichen Frameworks; ihre Inkremente folgen erst nach dem stabilen Einzelspielerstand.
