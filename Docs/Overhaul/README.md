# Voronation: Überarbeitungsplan

Stand: 28.09.2026. Die Roadmap wird schrittweise umgesetzt: 02 ist implementiert und sein reiner Kern gezielt geprüft; Unity-Test-Runner, PlayMode und Playerbuilds sind wegen des blockierten Batch-Lizenzkanals beziehungsweise offener manueller Prüfung noch nicht abgenommen. Die offenen Plattform-/Gesamtnachweise aus 00/01 bleiben bestehen. Maßgeblich ist jeweils der Status in der Inkrementdatei.

Projekteinstieg: [README](../../README.md). Arbeitsvorgaben: [AGENTS](../../AGENTS.md) und [Developer Guide](../../DeveloperGuide.md). Systemkontext: [Architecture](../../Architecture.md). Prüfnachweise: [Testing](../Testing.md). Allgemeine Engineering-Regeln bleiben in der [Shared-Baseline](../../../shared-engineering-guidelines/README.md).

Ziel: Ein stabiles, verständliches Strategiespiel mit spürbarer Gebietskontrolle, reaktiven Gegnern und wiederaufnehmbaren Partien. Die Voronoi-Idee und die vier Kernaktionen bleiben der Ausgangspunkt. Im Plan heißt die Spielfigur Ritter; im Bestand heißen zugehörige Klassen noch Leader oder Preacher.

## Ausgangslage und Planungsannahmen

- Der frühere Git-Stand nennt Unity 2023.2.3f1. Das aktuelle Arbeitsverzeichnis nennt bereits 6000.6.0f1 und enthält uncommittete Paket-/Projektänderungen. Diese Änderungen erhalten und zunächst zuordnen.
- Die Analyse beruht auf Quellcode, Szenenkonfiguration und Jam-Feedback. Ein vollständiger aktueller Spieltest und ein erfolgreicher Player-Build sind noch nicht nachgewiesen.
- Bewegung, Teilen, Stärke und Einkommen haben bereits Kosten im Code. Fehlende Transparenz und fehlerhafte Abrechnung/Budgetprüfung sind von einer neuen Kostenregel zu unterscheiden.
- Die Standard-KI benutzt zwei Upgrade-Taktiken. Sie bewegt und teilt sich dort nicht. Die gewünschte räumliche Reaktion ist daher eine Erweiterung, nicht nur das Umstellen bestehender Prioritäten.
- Zielversion ist Unity 6000.6.x mit festgehaltener Patchversion. Nicht während jedes Inkrements automatisch auf eine andere Versionslinie wechseln.
- Vorläufige Zielplattformen: Windows als Entwicklungsbuild und Browser/WebGL als bisheriger Veröffentlichungsweg. In Inkrement 00 verbindlich festhalten.
- Multiplayer, Völker und zusätzliche Siegbedingungen sind optionale Folgepakete. Die Einzelspieler-Überarbeitung soll ohne diese fertig werden können.

## Reihenfolge und lieferbare Zwischenstände

Die Nummern bilden den empfohlenen Arbeitsweg. Abhängigkeiten stehen zusätzlich in jeder Datei. Ein Inkrement kann über mehrere kleine Änderungen umgesetzt werden; sein Ende ist ein überprüfbares Ergebnis.

| ID | Inkrement | Ergebnis | Voraussetzung |
| --- | --- | --- | --- |
| 00 | [Unity-Basis und Bestandsaufnahme](00-unity-basis.md) | Reproduzierbarer Build auf 6000.6.x | Keine |
| 01 | [Akute Fehler](01-akute-fehler.md) | Bekannte Abbruchpfade abgesichert | 00 |
| 02 | [Spielkern und Rundenablauf](02-spielkern.md) | Berechnung unabhängig von Animation | 01 |
| 03 | [Geometrie und Stärke](03-geometrie-und-staerke.md) | Robuste, gewichtete Gebiete | 02 |
| 04 | [Wirtschaft und Aktionen](04-wirtschaft-und-aktionen.md) | Verbindliche Kosten und sinnvolles Warten | 03 |
| 05 | [Input und Ereignisse](05-input-und-ereignisse.md) | Einheitliche, zustandsabhängige Bedienung | 02, 04 |
| 06 | [Speichern und Fehlererholung](06-speichern-und-fehlererholung.md) | Partie nach Unterbrechung fortsetzen | 02–04 |
| 07 | [UI, Auswertung und Log](07-ui-auswertung-und-log.md) | Entscheidungen und Ergebnisse verstehen | 04, 05 |
| 08 | [Zellanimation und Spieltempo](08-zellanimation-und-spieltempo.md) | Sichtbare Gebietsänderungen, einstellbares Tempo | 03, 05, 07 |
| 09 | [Reaktive KI](09-reaktive-ki.md) | Unterschiedliche räumliche Strategien | 03, 04 |
| 10 | [Tutorial](10-tutorial.md) | Regeln durch kleine Aufgaben erlernen | 07–09 |
| 11 | [Sound](11-sound.md) | Klare akustische Rückmeldung | 05, 07, 08 |
| 12 | [Partieauswahl und Balancing](12-partieauswahl-und-balancing.md) | Konfigurierbare, ausgewogene Partien | 04, 07, 09 |
| 13 | [Releaseprüfung](13-releasepruefung.md) | Überarbeitete Einzelspieler-Version | 00–12 |
| 14 | [Lokaler Multiplayer und Hotseat](14-lokaler-multiplayer.md) | Mehrere Menschen an einem Rechner | 13 |
| 15 | [Online-Multiplayer](15-online-multiplayer.md) | Autoritative Online-Partie mit Wiederverbindung | 14 empfohlen; Spielkern erforderlich |
| 16 | [Völker und Siegbedingungen](16-voelker-und-siegbedingungen.md) | Optionale strategische Vielfalt | 12, 13 |

Meilenstein A nach 00–06: technisch verlässliche, wiederaufnehmbare Partie. Meilenstein B nach 07–12: verständliche und abwechslungsreiche Partie. Meilenstein C nach 13: veröffentlichungsfähige Einzelspieler-Version. Danach die Folgepakete einzeln auswählen.

## Arbeitsweise für die Umsetzung

1. Die jeweilige Inkrementdatei und ihre Abhängigkeiten lesen; Bestand vor jeder Änderung prüfen.
2. Status auf „in Arbeit“ setzen. Konkrete Regelentscheidungen in [Entscheidungen](ENTSCHEIDUNGEN.md) festhalten; Vorschläge nicht stillschweigend als bereits beschlossene Regeln behandeln.
3. Kleine, spielbare Änderungen bevorzugen. Bestehende Szenen und Assets weiterverwenden; kein pauschaler Komplettneubau.
4. Regeln mit gezielten EditMode-Tests prüfen, Szenen/Bedienung mit PlayMode- oder manuellen Prüfungen. Früh echte Player-Builds verwenden.
5. Erfüllte Abnahmepunkte, Testnachweise, verbleibende Risiken und geänderte Regeln in der Inkrementdatei dokumentieren.
6. Ein Inkrement erst dann als fertig markieren, wenn sein Ergebnis im vorgesehenen Build funktioniert. Nicht nur auf erfolgreiche Editor-Kompilierung verlassen.

Abhängigkeiten bedeuten keine parallele Agentenarbeit. Pro Aufgabe kann ein einzelnes Inkrement beauftragt werden, etwa: „Setze Docs/Overhaul/01-akute-fehler.md um und dokumentiere die Prüfungen.“

## Gemeinsame Qualitätsziele

- Mehr als fünf Ritter werden korrekt verwaltet und angezeigt; IDs sind unabhängig von der Darstellung.
- Nähe, identische Positionen, Kartenränder, Ecken und leere Zellen haben definierte Ergebnisse.
- Geldwerte und Flächen sind gültig und nachvollziehbar. Jede Abrechnung erfolgt genau einmal.
- Animation, Sound, Tempo und Eingabegerät verändern kein Rundenergebnis.
- Abbruchsicherheit heißt: bekannte Fehler vermeiden, unvollständige Runden nicht übernehmen und den letzten gültigen Stand wiederherstellen. Eine absolute Absturzfreiheit wird nicht versprochen.
- Zielgrößen für Ritterzahl, Rundendauer und Hardware werden gemessen und dokumentiert; keine unbelegten Leistungsversprechen.

## Referenzen

- [Jam-Seite mit Spielerfeedback](https://itch.io/jam/acerola-jam-0/rate/2582186)
- [Unity-Upgradereferenzen für 6000.6](https://docs.unity3d.com/6000.6/Documentation/Manual/UpgradeGuides.html)
- [Input System für Unity 6000.6](https://docs.unity3d.com/6000.6/Documentation/Manual/com.unity.inputsystem.html)
- [Input System: UI-Integration](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.17/manual/UISupport.html) – Paketdokumentation bei Umsetzung mit der tatsächlich installierten Version abgleichen.
- [NetTopologySuite VoronoiDiagramBuilder](https://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.Triangulate.VoronoiDiagramBuilder.html)
- [Clipper2](https://www.angusj.com/clipper2/Docs/Overview.htm) und [numerische Robustheit](https://www.angusj.com/clipper2/Docs/Robustness.htm)
- [Power-Diagramme](https://doc.cgal.org/4.9/Triangulation_2/index.html)

Die Bibliotheksquellen begründen die Optionen. Noch keine Geometriebibliothek wurde ausgewählt, installiert oder im Unity-Build erprobt.
