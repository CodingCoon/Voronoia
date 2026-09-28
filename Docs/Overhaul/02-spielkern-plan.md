# 02 – Codeanalyse und Umsetzungsplan

Stand der Analyse: 28.09.2026. Dieses Dokument hält den Planungsstand vor der Umsetzung fest. Den aktuellen Implementierungsstatus und die Nachweise führt [02 – Spielkern](02-spielkern.md); dauerhafte Beschlüsse stehen in [ENTSCHEIDUNGEN](ENTSCHEIDUNGEN.md).

## Ausgangspunkt und Empfehlung

02 ist eine Migration des Zustandsbesitzes und der Rundenberechnung. Ein einzelner neuer `RoundResolver` genügt nicht: Auch Aktionen, Geometrie, Setup, KI und die sichtbare Phasenkette müssen ihre Unity-Abhängigkeiten an der richtigen Grenze verlieren. Bestehende Szenen, Prefabs und DOTween-Darstellung weiterverwenden.

Empfohlene Reihenfolge: **02a Zustand → 02b Befehle → 02c Geometrieextraktion und atomare Berechnung → 02d Ergebnisdarstellung und zentrale Übergänge**. Den minimalen Ergebnisvertrag bereits in 02c einführen; ohne ihn wären Berechnung und Darstellung erneut gekoppelt. Jeder Abschnitt endet mit einem startbaren Spiel und gezielten Prüfungen. Atomare Runden sind erst nach 02c gewährleistet.

Analysiert wurde der aktuelle, stark uncommittete Arbeitsstand einschließlich der Änderungen aus 01. Tatsächliche Basis: Unity 6000.6.0f1, Test Framework 1.8.0, uGUI 2.6.0 und SpriteShape 16.0.0 laut Projekt-/Paketdateien; DOTween 1.3.030 laut Abhängigkeitsdokumentation. Keine neue Bibliothek ist für 02 erforderlich.

01 ist laut dokumentiertem Nachweis implementiert und gezielt im Editor geprüft. Seine Playerbuilds und manuelle Gesamtabnahme bleiben offen. Die Ausnahme zu den Prüfungen aus 00 ist keine pauschale Abnahme von 01 oder 02. Für diese Planung wurde weder Unity gestartet noch ein Build/Test ausgeführt.

## Befunde im Code

Die Pfade in dieser Tabelle liegen unter `Assets/Scripts/`.

| Bereich | Tatsächlicher Befund | Konsequenz für 02 |
| --- | --- | --- |
| `Data Model/Game.cs`, `Round.cs` | `Game` besitzt Unity-Fraktionen und Coroutine-/Phasenschutz. `Round` enthält nur ein ungenutztes Feld; `curRound` wird nicht initialisiert. | Match-Zustand, Rundennummer und Übernahmevertrag neu einführen; vorhandene Schutzmechanismen gezielt ablösen. |
| `Data Model/impl/Leader.cs` | Werte und Buchungen liegen im MonoBehaviour; Position kommt aus `knob.transform`, Fläche aus `PreacherArea`. `ApplyAction` bucht, erhöht das Alter und wartet danach auf die Aktion. | Werte und Fläche in Daten halten; Animationen dürfen weder Regelquelle noch Schreibpfad sein. |
| `Actions/` | `IAction.Execute()` ist eine Coroutine. Bewegung verändert nur den Transform; Upgrades werden erst nach Sound/VFX wirksam. Teilen erzeugt sofort ein Prefab. | Befehle beschreiben Absichten; der Resolver verändert Daten, die Darstellung erzeugt/animiert Views. |
| `Data Model/impl/Voronation.cs` | Ritter-IDs steigen je Fraktion monoton; Fraktionen haben keine stabile ID. Geld, Rittererzeugung, Insolvenz und `Destroy` sind gekoppelt. | Bestehende sichtbare Nummern erhalten, Fraktions-ID ergänzen und Laufzeitlisten dem Kern zuordnen. |
| `Phases/` | Aktionen, Geometrie, Kontobuchung und Verluste passieren nacheinander am aktiven Zustand. `DeathPhase` berechnet bei fortgesetzter Partie nach Verlusten erneut Gebiete. | Beide Geometrieschritte müssen zur gleichen Transaktion gehören. Fehler der zweiten Berechnung dürfen bereits gebuchte Verluste nicht zurücklassen. |
| `voronoi/VoronoiController.cs` | Validiert sämtliche Zellen vor Zuweisung und versucht bei Darstellungsfehlern Polygon-Rollback. Das ist kein Rollback von Aktionen, Alter, Geld oder Rittererzeugung. | Geometrie vollständig auf Daten berechnen; fachliche Übernahme und Unity-Zuweisung getrennt behandeln. |
| `voronoi/VoronoiCalculator.cs`, `IVoronoiCellOwner.cs` | Rechner ist ein MonoBehaviour mit `Map`-Referenz. Owner-Vertrag mischt Position/Stärke mit `UpdateVoronoi`. Rechenhilfen sind weitgehend normale C#-Objekte. | Bestehenden Algorithmus extrahieren, Eingabe/Ausgabe von Owner-Callbacks lösen; mathematischer Austausch bleibt 03. |
| `Preacher/`, `Actions/PlannedActionController.cs` | Zielwahl begrenzt auf das aktuelle eigene Polygon; UI erzeugt Aktionsobjekte direkt. Vorschau und bestätigter Plan sind unterschiedliche Objekte. | Vorschauentwurf von bestätigten Befehlen trennen, denselben Validator für UI und KI verwenden. |
| `AI/` | Standardtaktiken wählen nur Upgrades. Negative Preise werden wie positive Kosten verglichen und auf `blockedMoney` addiert. `Clear()` hat keinen Aufrufer. | Fehler explizit festhalten; Migration nicht als Gelegenheit für eine unbeschlossene Budgetregel oder räumliche KI nutzen. |
| `Setups/`, `Map.cs` | Standard- und Tutorialsetup sind deterministisch; `RandomReligionSetup` nutzt globale Unity-Zufallswerte. `RandomTactic` würfelt trotz Namen nicht. Tutorial startet eine KI mit −50 Geld. | Setupdaten vor Views erzeugen; negative Startkonten nicht pauschal ungültig erklären. Seed/Zufall am Kompositionspunkt bereitstellen. |
| `EvaluationPanel.cs`, `MoneyLabel.cs`, `LeaderPanel.cs` | Anzeigen lesen veränderliche MonoBehaviours; Schuldenerlass wird später an einen Text angehängt. | Abgeschlossene Buchungen samt ausgeschiedenen Rittern aus einem unveränderlichen Ergebnis lesen. |
| `Tutorial/TutorialHelper.cs`, `TutorialManager.cs`, `NextButton.cs` | Tutorial und Weiter-Button beobachten `APPLY`, `VORONOI`, `EVALUATION`, `DEATH`. | Sichtbare Präsentationsschritte erhalten, aber von fachlichen Transaktionen trennen; sofortiges Anzeigen darf Tutorialfortschritt nicht verschlucken. |

Die Listen in `ApplyPhase` und `DeathPhase` werden bereits über Kopien abgefragt. Das verhindert einzelne Enumerationsfehler und erklärt, warum ein neu geteilter Ritter aktuell keinen eigenen Zug erhält. Es ersetzt keinen vollständigen, eingefrorenen Planungssnapshot.

## Vorgeschlagene Verträge und Grenzen

Neue Kernklassen unter `Assets/Scripts/Core/`, Assembly `Voronation.Core`, Namespace beispielsweise `VoronationCore`. Der Namespace vermeidet eine Kollision mit der bestehenden globalen Klasse `Voronation`. Bestehende MonoBehaviours behalten Namen, Script-GUIDs und soweit möglich serialisierte Referenzen.

| Vertrag | Inhalt und Verantwortung |
| --- | --- |
| `MatchState` | Abgeschlossene Rundennummer, Kartenmaße, Seed, menschliche Fraktions-ID, Matchausgang, Fraktionen, Ritter und gültige Zellen. Rundennummer 0 ist der initial berechnete Stand. |
| `FactionState` | Stabile Fraktions-ID, Kontostand, nächste freie Ritternummer; Name/Farbe und Steuerungsart als Setupdaten, keine Referenz auf `ITactic` oder GameObjects. |
| `KnightId`, `KnightState` | Identität als Paar aus Fraktions-ID und monotoner lokaler Nummer; Position, Stärke, Einkommensfaktor und Alter. Sichtbare Nummern bleiben kompatibel mit 01. |
| `RoundPlan`, `ActionCommand` | Plan getrennt vom abgeschlossenen Zustand. Befehl enthält Besitzer, Ritter-ID, Aktion, optional Ziel; der Plan trägt die erwartete Rundenversion. Pro bereits vorhandenem Ritter höchstens ein bestätigter Befehl. |
| `CommandValidator` | Gemeinsame, nebenwirkungsfreie Prüfung mit Fehlercode, betroffenen IDs und gegebenenfalls Ziel. Keine übersetzten UI-Texte als fachlicher Fehlervertrag. |
| `RoundResolver` | Berechnet eine Arbeitskopie aus Zustand und eingefrorenem Plan. Kennt weder Szene noch Audio, Coroutines, Prefabs oder globale Singletons. Liefert Erfolg mit Ergebnis oder strukturierten Fehler. |
| `RoundResult` | Unveränderliche Vorher-/Nachherdaten, akzeptierte Befehle, typisierte Buchungen, Gebietsdeltas, geordnete Ereignisse und Rundenversion. Keine Referenzen auf mutable View-Listen. |
| `MatchSession` | Besitzt gültigen Zustand, aktuellen Plan und erlaubte Übergänge; übernimmt ein passendes Ergebnis genau einmal. `Game` komponiert diese C#-Instanz und bindet Unity an. |

Zustandslisten und Polygonarrays werden beim Kopieren tatsächlich unabhängig; ein `IReadOnlyList` allein macht die darunterliegenden Objekte nicht unveränderlich. Änderungen erfolgen innerhalb des Kerns. Alte Ergebnisse bleiben auch nach weiteren Runden lesbar. Eine kleine Arbeitskopie ist zunächst angemessen; kein Copy-on-write-, ECS- oder Event-Sourcing-System.

`Vector2`, `Vector3`, `Mathf` und gegebenenfalls `Color` dürfen als Unity-Werttypen erhalten bleiben. Das Testziel ist Berechnung ohne Szene, Kamera, Transform und Lifecycle, nicht eine zusätzliche eigenständige .NET-Portierung.

### Geometriegrenze

- Den bisherigen Algorithmus in einen normalen C#-Rechner unter der bestehenden Geometrieassembly extrahieren. Eingaben: stabile Site-Zuordnung, Position, Stärke, Kartengrenze. Ausgaben: Site-Zuordnung, Polygon, Fläche oder expliziter Fehler.
- `VoronoiCalculator` zunächst als schmalen Komponentenadapter für vorhandene Szenenreferenzen erhalten. Die Kernberechnung ruft direkt den datenbasierten Rechner auf, nicht diesen Adapter.
- Betroffene `CC*`-Hilfstypen samt `.meta` gezielt in die Geometrieassembly verschieben, wenn sie Teil des extrahierten Rechners bleiben. Abhängigkeit auf `DictionaryExtension` aus Assembly-CSharp durch lokale konkrete Dictionary-Operationen ablösen; keine Abhängigkeit der Geometrie auf die Standardassembly.
- Der Kern referenziert `Voronation.Geometry`. Eine kleine injizierbare Zellberechnungs-Schnittstelle am Resolver erlaubt gezielte Fehlerfälle; sie ist durch den bevorstehenden Austausch in 03 und Transaktionstests begründet. Geometrie referenziert den Kern nicht: die Zuordnung zur `KnightId` geschieht am Adapter.
- Validierungen aus 01 erhalten: endliche Werte, vollständige eindeutige Zuordnung, Kartengrenzen und darstellbare Polygone. Leere berechnete Zellen bleiben bis zur Regelarbeit in 03 explizit nicht unterstützt. Kein neues Versprechen über lückenlose Kartenabdeckung.

## Rundenvertrag für die Umsetzung

Die folgende Reihenfolge erhält zunächst das nachvollziehbare Bestandsverhalten. Die unten genannten offenen Produktentscheidungen sind vor den betroffenen Umsetzungsschritten zu klären.

1. **Planung abschließen:** Mensch und KI planen gegen denselben gültigen Zustand. Befehle und Planversion einfrieren; doppelte Bestätigung sofort sperren. Fehlende Befehle vorläufig wie bisher zu `NoAction` normalisieren. Noch keine ausdrücklich wählbare Warten-Aktion aus 04.
2. **Vollständig validieren:** Version, aktive Fraktion, Besitz, existierende Ausgangsritter, Aktionsart, notwendiges/unerlaubtes Ziel, endliche Koordinaten und zulässigen Zielbereich prüfen. Mehrfachbefehle im eingereichten Paket ablehnen; ein ausdrücklicher Planwechsel ersetzt dagegen den vorherigen Plan. Abgelehnte Absichten werden nicht still in andere Aktionen umgewandelt.
3. **Arbeitskopie anlegen:** Alle veränderlichen Zustandsdaten einschließlich ID-Zähler kopieren. Stabile Verarbeitung nach Fraktions-ID und lokaler Ritternummer; keine fachlichen Entscheidungen anhand Dictionary-Iteration oder Unity-Objektreihenfolge.
4. **Aktionen anwenden:** Kosten aus dem Ausgangszustand ermitteln; vorhandene Ritter altern genau einmal, auch bei `NoAction`. Bewegung setzt Datenposition; Upgrade erhöht den jeweiligen Faktor um 0,1. Teilen erzeugt einen neuen Datensatz mit neuer ID. Nur Ritter aus dem Planungssnapshot dürfen handeln.
5. **Abrechnungsgebiete berechnen:** Alle endgültigen Aktionspositionen und Stärken gemeinsam verwenden; Ergebnisse und Flächen vollständig prüfen. Keine Berechnung aus animierten Zwischenpositionen.
6. **Wirtschaft auswerten:** Aktionsbuchung, Flächeneinkommen und Unterhalt für jede Fraktion sammeln; alle Summen auf Gültigkeit prüfen. Erst innerhalb der Arbeitskopie die neuen Konten setzen.
7. **Verluste gemeinsam bestimmen:** Bei Geld < 0 genau den ältesten Ritter entfernen, bei Altersgleichheit die niedrigste lokale Nummer; Schuldenerlass als eigene Buchung, Konto auf 0. Erst Verlustliste ermitteln, dann entfernen. Leere Fraktionen gemeinsam entfernen und Ergebnis aus allen verbleibenden Fraktionen bestimmen; keine verbleibende Fraktion bedeutet Unentschieden.
8. **Planungsgebiete neu berechnen:** Nach Verlusten bei fortgesetzter Partie die Gebiete der Überlebenden erneut berechnen und prüfen. Kein zweites Einkommen und kein zweites Altern. Auch ein Fehler hier verwirft die gesamte Arbeitskopie. Bei Partieende ist keine erneute Verteilung erforderlich; entfernte Ritter besitzen im finalen aktiven Zustand keine Zelle.
9. **Ergebnis abschließen und übernehmen:** Finale Invarianten prüfen; `RoundResult` einschließlich Ergebnisdaten erstellen. `MatchSession` ersetzt nur bei passender Ausgangsversion den gültigen Zustand und erhöht die abgeschlossene Rundennummer genau einmal. Bei Ablehnung/Fehler bleiben Zustand, ID-Zähler und Zufallsfortschritt unverändert.
10. **Ergebnis präsentieren:** Views spielen den vorhandenen Ablauf ab oder setzen direkt das Endbild. UI, Audio und Diagnose verwenden dasselbe Ergebnis. Rückmeldungen über Darstellungsabschluss enthalten eine Ergebnis-/Präsentationskennung und können keine zweite Buchung auslösen.

### Wirtschaftliche Kompatibilität

Für 02 zunächst bestehende Zahlen und Buchungszeitpunkte an einer Regelquelle zusammenführen: Bewegung −10 × Entfernung, Teilen −25, Upgrade −40 × nächster Faktor; Einkommen Fläche × Einkommensfaktor, Unterhalt −40 × Alter nach dem Altern. Vorhandene signierte Buchungen lassen sich migrieren, ohne schon Budgetreservierung einzuführen.

Beim Teilen ergibt der Bestand für Elternteil und Kind jeweils `(alter Faktor + 1) / 2`, getrennt für Stärke und Einkommen. Das Kind startet mit Alter 0, erhält in dieser Runde bereits Gebiet und Einkommen, zahlt dadurch zunächst 0 Unterhalt und bekommt keinen eigenen Zug. Dies als Kompatibilitätsverhalten testen; die fachliche Neubewertung gehört nach 04. Der Elternteil altert und bezahlt die Teilung.

### Ergebnisdaten für zwei Gebietsstände

Vorher/Nachher allein genügt bei Verlusten nicht: Einkommen entsteht auf den Zellen **vor** dem Entfernen, die nächste Planung verwendet die Zellen **danach**. `RoundResult` muss deshalb die Abrechnungszellen bzw. deren vollständige Gebietsdeltas zusätzlich zum endgültigen Zustand tragen. Buchungen behalten Fraktions- und Ritter-IDs auch für ausgeschiedene Teilnehmer. Der Auswertungstext darf nicht aus der Liste der Überlebenden rekonstruiert werden.

`ActionPlanned` entsteht nach erfolgreicher Planannahme. Ereignisse wie Bewegung, Upgrade, Teilung, `KnightRemoved`, Fraktionsausscheiden und `MatchEnded` entstehen als geordnete Ergebnisdaten. `RoundResolved` wird nach erfolgreicher Übernahme einmalig bekanntgegeben; fehlgeschlagene Berechnungen veröffentlichen keine Erfolgsmeldungen. Direkte C#-Callbacks und das Ergebnisobjekt genügen; keine globale Event-Bus-Infrastruktur. Audio interpretiert Fachereignisse, der Kern kennt keine Soundnamen. Ein neues Logpanel bleibt 07, ein aus demselben Ergebnis erzeugbarer Diagnosetext gehört zur Verifikation.

### Übergänge, Fehler und Zufall

`MatchSession` führt beispielsweise `Planning → Resolving → Presenting → AwaitingContinue → Planning` bzw. nach der Darstellung `Ended`. Bei Eingabefehlern bleibt die Sitzung in Planung. Unerwartete Berechnungsfehler erhalten den letzten gültigen Zustand und führen zum kontrollierten Fehlerbildschirm aus 01. Persistentes Laden/Wiederherstellen bleibt 06.

`APPLY`, `VORONOI`, `EVALUATION` und `DEATH` können als sichtbare Präsentationsabschnitte bestehen bleiben. Sie ändern keine fachlichen Daten mehr. Erlaubte Übergänge liegen zentral und nicht länger in einer frei verdrahteten `GetNextPhase`-Kette. `Game.CanPlan`/`CanAdvance` bleiben Adapter für die bestehende Bedienung. Frame-Sperren allein reichen nicht: Rundenversion und Übergangszustand müssen auch Bestätigungen in verschiedenen Frames und veraltete Coroutine-Rückmeldungen abweisen.

Bei Präsentationsabbruch ist ein bereits gültig übernommenes Ergebnis weiterhin gültig. Ein ausdrücklich angefordertes Überspringen beendet Tweens, bereinigt Vorschauen/temporäre Views und setzt das vollständige Endbild. Unerwartete Darstellungsfehler sperren die Bedienung und zeigen einen Fehler; sie lösen weder Neubuchung noch Rücknahme des fachlichen Ergebnisses aus. Ein Szenenwechsel räumt Präsentation und Abonnements auf, ohne neue Phasen zu starten. Keine automatische Suche oder Ersatzkomponente zur Reparatur fehlender Inspectorreferenzen.

Seed und injizierbare Zufallsquelle am bestehenden Bootstrap festlegen. Setup und gegebenenfalls KI erhalten getrennte, nachvollziehbar aus Seed und Rundennummer abgeleitete Quellen; keine unkontrollierten `UnityEngine.Random`-Aufrufe im Regelpfad. Die Standard-KI benötigt derzeit keine Ziehungen. Für Berechnungen einen reproduzierbar neu erzeugbaren Rundengenerator verwenden, damit ein fehlgeschlagener Versuch keinen externen RNG fortschaltet. Ableitung explizit definieren, nicht auf instabile Hashcodes stützen. Gleiche Version und gleiche vollständige Eingaben müssen gleiche Zustände, Buchungen, IDs und Ereignisreihenfolgen liefern; keine plattformübergreifende Bitgleichheit zusagen.

## Lieferbare Teilstücke

| Teilstück | Konkrete Änderungen | Spielbarer Zwischenstand und Nachweis |
| --- | --- | --- |
| **02a: Zustand und Bindung** | Kernassembly und Datenobjekte einführen. `Game` erzeugt Sitzung; `AbstractSetup`/Standard-/Tutorialsetup liefern Daten vor View-Erzeugung. `Leader`/`Voronation` binden IDs und delegieren Werte an den Zustand. Position/Fläche kommen aus Daten. ID→View-Zuordnung am Unity-Adapter. Bestehende Methoden vorübergehend als delegierende Brücken erhalten. | Menü → Partie/Tutorial funktioniert. Bestehende Aktionen laufen noch über die Phasen, verändern aber die eine Datenquelle. Bewegung setzt Modellposition ausdrücklich und animiert nur die View. Tests: IDs, Kopierisolation, Setup, keine zweite Transform-Wahrheit. Noch keine Behauptung atomarer Auflösung. |
| **02b: Plan und gemeinsame Prüfung** | Datenbefehle, Planversion und Validator ergänzen. `RingMenu`, `PreacherKnob`, `PlannedActionController`, `LeaderPanel` auf Absichten/Abfragen umstellen. KI produziert Befehle gegen denselben Snapshot. Alte Aktionsobjekte nötigenfalls vorübergehend aus akzeptierten Befehlen adaptieren. | Vier Aktionen, Vorschau, Planwechsel/Abbruch und KI funktionieren über denselben Plan. Tests: Besitz, unbekannte/entfernte IDs, alte Version, fehlende/ungültige Ziele, doppelte Befehle und gleichwertige UI-/KI-Prüfung. |
| **02c.1: Datenbasierte Geometrie** | Rechner und notwendige Hilfstypen extrahieren; Controller auf Daten und Darstellungszuweisung begrenzen. Fläche außerhalb von `PreacherArea` berechnen. Schnittstelle für Resolver/Fehlerprüfung einführen. | Bisherige Geometriefälle liefern vergleichbare Ergebnisse ohne Scene/GameObject; Bestandsalgorithmus bleibt erhalten. Adapter versorgt weiter die laufende Szene. |
| **02c.2: Vollständige Transaktion** | Aktionen, Alter, Abrechnung, Insolvenz, Partieende und zweite Geometrie in Resolver übertragen; minimalen `RoundResult` gleichzeitig liefern. Beim Bestätigen genau einmal auflösen und übernehmen. Alte Regelaufrufe in allen Phasen im selben Teilstück entfernen; Phasen lesen danach Ergebnisdaten. | Vollständige Runde ohne Kamera, Sound und Animation berechenbar. Bestehende Darstellung startet mit dem übernommenen Ergebnis. Tests: Regeln, reproduzierbare Ergebnisse, Fehler vor und nach Abrechnung, keine Teilübernahme. |
| **02d: Ergebnis und Präsentation fertigstellen** | Buchungen, Gebietsstände und Ereignisse vervollständigen; Evaluation/Geld/Details/SFX daraus speisen. Zentralen Übergangsvertrag vervollständigen, Abschluss/Überspringen idempotent machen. `TweenPlayback`-Abbruch von fachlichem Scheitern trennen. Tutorial auf bestätigte Plan-/Präsentationsfortschritte anbinden. | Normal dargestellte und sofort angezeigte Runde haben denselben Endzustand und dieselbe Abrechnung. Weiter/Neustart/Menü und vollständiges Tutorial bleiben bedienbar. Keine Tempowahl oder animierte Zellinterpolation aus 08 vorziehen. |
| **Abschluss: Brücken und Nachweise** | Nicht mehr genutzte mutierende Aktions-/Modellverträge und den leeren `Round`-Platzhalter samt Aufrufern entfernen. Testhelfer migrieren. Architektur-Iststand, Developer Guide, Testing und Status in 02 aktualisieren. | Keine aktive Parallelberechnung im Altpfad. Szenen-/Prefab-Referenzen geprüft; Windows-/WebGL-Builds und festgelegte Bedienungsprüfungen dokumentiert. |

02c.2 ist die kritischste zusammenhängende Änderung: Ein Resolver zusätzlich zu weiterbuchenden Altphasen würde doppelte Einnahmen und widersprüchliche Zustände erzeugen. Eine kurze Übergangsbrücke ist sinnvoll, zwei dauerhafte Regelimplementierungen nicht.

## Entscheidungen für die Umsetzung

Zielbereich, Kollisionsbehandlung und Budgetumfang wurden nach der ersten Planung vom Nutzer bestätigt; maßgeblich ist das [Entscheidungsprotokoll](ENTSCHEIDUNGEN.md).

| Thema | Empfehlung für 02 | Zeitpunkt |
| --- | --- | --- |
| Gleichzeitige Zielkonflikte | Beschlossen: Zielvorschau und Validator verlangen 0,05 Welteinheiten Abstand zur Gebietsgrenze; Teilen zusätzlich zum Elternritter. Bei zu schmalem Gebiet wird kein Ziel bestätigt. Kein nachträgliches zufälliges Verschieben. Die vollständige Endpositionsprüfung bleibt als Absicherung bestehen. | In 02b/02c umsetzen. |
| Zielbereich | Beschlossen: Bewegung und Teilen nur im eigenen Rittergebiet am Planungsbeginn einschließlich der technisch nach innen versetzten Vorschau. | In 02b umsetzen. |
| Budget und KI-Vorzeichenfehler | Beschlossen: in 02 keine neue Vorabreservierung; Finanzierung durch Rundeneinkommen bleibt möglich. Preisformeln und Abrechnung zentralisieren. KI-Befehle verwenden den gemeinsamen Validator ohne die defekte zusätzliche Budgetheuristik. | In 02b/02c umsetzen; neue Budgetregeln bleiben 04. |

Die gemeinsame Planung auf einem Stand ist bereits das ausdrückliche Ziel von 02; sie braucht keinen neuen Produktentwurf. Kostenreservierung ist dagegen in 04 noch eine Empfehlung, kein bestehender Beschluss. Die Insolvenz- und Gleichstandsregeln aus 01 sind verbindlich und werden unverändert migriert.

## Verifikationsplan

Kleine neue EditMode-Testassembly `Voronation.Core.Tests` mit Referenzen auf den tatsächlich extrahierten Kern und gegebenenfalls Geometrie. Vorhandene `Voronation.Geometry.Tests` weiterverwenden. Keine asmdef pauschal über alle Skripte legen. Unity-Integration zunächst mit angepassten Editor-Prüfhelfern für die weiter in Assembly-CSharp liegenden Komponenten testen.

| Prüfgruppe | Wesentliche Fälle |
| --- | --- |
| Zustand/Identität | Gleiche lokale Nummer in zwei Fraktionen unterscheidbar; Nummern 5/6/10/20; Entfernen und erneutes Teilen verwendet keine ID wieder. Änderungen an Arbeitskopie, Plan oder späteren Ergebnissen ändern frühere Snapshots nicht. |
| Befehle | Falscher Besitzer, fehlende/entfernte Fraktion, unbekannter/neuer Ritter, unbekannte Aktion, doppelte Befehle, fehlende/überflüssige/NaN-Ziele, falsche Phase und veraltete Runde. Planwechsel ersetzt nur den gewählten Ritterplan. |
| Regeln | Jede der vier Aktionen sowie implizites Nichtstun; Bewegungsentfernung aus Ausgangsposition; Upgradekosten vor Mutation; Teilungsvererbung, Alter 0, erstes Einkommen/Unterhalt und kein zusätzlicher Kinderzug. |
| Geld/Verlust | Endgeld = Startgeld + Summe aller Buchungen einschließlich Schuldenerlass. Geld 0 behält Ritter; negativ entfernt genau einen. Ältester/Nummerngleichstand, menschliche Fraktion entfernt, mehrere Fraktionen gleichzeitig entfernt und Unentschieden. |
| Zwei Geometrieschritte | Einkommen aus Abrechnungszellen, nach Verlusten andere Planungsflächen ohne zweite Auszahlung. Ergebnis bewahrt Buchungen und Flächen ausgeschiedener Ritter. |
| Atomarität | Fehler bei erster Zellberechnung, ungültiger Zellausgabe, nicht endlicher Abrechnung und zweiter Zellberechnung nach Verlusten. Alle ursprünglichen Werte, Listen, Polygone, ID-Zähler und Zufallsdaten bleiben unverändert; kein Erfolgsereignis. Anschließender gültiger Versuch funktioniert. |
| Reproduzierbarkeit | Mehrere Runden aus festem Setup/Seed und Befehlsfolge; unterschiedliche Eingabe-Listenreihenfolge wird kanonisch verarbeitet. Zustände, Buchungen, neue IDs und Ereignisse vergleichen. Wiederholter Aufruf mit derselben Basiseingabe liefert gleiches Ergebnis; Sitzung kann dieses nur einmal übernehmen. |
| Darstellung/Lifecycle | Normaler Ablauf versus direkter Abschluss; doppelte Bestätigung im selben und in späteren Frames; veralteter Abschlusscallback; Abbruch in Bewegung/Teilung/Einkommen/Verlust; Szenenwechsel und Neustart. Darstellung schreibt nie Modellwerte zurück. |
| Integration | Menüstart und Direktstart diagnostisch prüfen, Standardsetup und vollständiges Tutorial, alle Aktionen/Planwechsel, Mehrfachteilung, Ergebnis- und Fehlerpanel. Inspectorreferenzen, TMP-IDs, Collider und View-Erzeugung/-Entfernung kontrollieren. |

Die Regressionen aus `Increment01Verification` und `Increment01PlayVerification` nach Verhalten erhalten und an die neuen Verträge anpassen. Insbesondere erwarten sie derzeit mutierende `Leader`-Methoden, Reflexionszugriff auf Alter und einen Geometriefehler erst in `VORONOI`. Diese Details passen nach der Migration nicht mehr; Checks dürfen nicht bloß gelöscht oder alte Regelmethoden nur für die Tests konserviert werden. Fehlerbehandlung anhand gezielt eingespeister Rechenfehler testen, wenn ein ungültiger Move künftig bereits am Validator scheitert.

Nach Einführung der Assemblygrenzen früh Windows-x64/Mono und WebGL/IL2CPP mit Unity 6000.6.0f1 bauen; Abschlussprüfung in Edge und Firefox über HTTP. Bestehende isolierte Projektkopie-/Buildwege aus dem [Developer Guide](../../DeveloperGuide.md) verwenden. Ein erfolgreicher Editorlauf ersetzt diese Nachweise nicht.

## Unity-Integration und spätere Übergabe

- `Assets/Scenes/GameScene.unity`, GameObject **Game**: `Game` bleibt Kompositionspunkt; vorhandene Referenzen `initialPhase`, `statusPanel`, `selection`, `plannedActions` bei der Umstellung bewusst migrieren. Phasenkomponenten auf Ergebnisdarstellung reduzieren; Nachfolgerreferenzen erst nach zentraler Umstellung entfernen.
- Dieselbe Szene, **Setup**: `AbstractSetup`-Ableitungen verwenden bestehende `game`, `religionPrefab`, `religionsFolder`; Setupdaten werden vor dem Erzeugen der Fraktions-/Ritterviews aufgebaut. **Map** liefert `Map.halfMapSize`; `VoronoiController.game/map/calculator` an den extrahierten Rechner/Adapter anbinden.
- `Assets/Prefabs/Voronation.prefab`, **Voronation**: `preacherPrefab` bleibt explizite Quelle für Ritterviews. Datenbasierte Teilung entscheidet über Erzeugung; der Adapter instanziiert die konfigurierte View.
- `Assets/Prefabs/Leader.prefab`, **Leader**, **Knob**, **Area**: `Leader.knob/area/numberLabel`, `PreacherKnob.preacher/area` sowie SpriteShape-/Colliderreferenzen erhalten. Diese Komponenten zeigen IDs, Positionen, Polygone und Ereignisse an; keine Ermittlung des fachlichen Zustands aus ihren animierten Werten.
- **ActionController**, **LeaderSelection**, **EvaluationPanel**, **MoneyLabel**, **LeaderPanel** und **TutorialManager** in GameScene auf Sitzung/Ergebnis anbinden. Pflichtreferenzen authorieren, nicht zur Laufzeit suchen. Konkrete neu eingeführte Inspectorfelder im Umsetzungsschritt dokumentieren.

Manueller Abschlussweg: `Menu.unity` öffnen → Standardspiel → Bewegung, beide Upgrades, Teilen und Planwechsel → mehrere Runden einschließlich Verlust → End-/Fehleranzeige → Neustart und Menü. Danach Tutorial vollständig durchlaufen und dieselbe Runde mit normaler sowie sofort abgeschlossener Darstellung vergleichen. IDs, Konten, Auswahl, Vorschau und Collider dürfen keine Reste früherer Zustände zeigen.

## Nachweis dieser Planung

Gelesen und statisch abgeglichen: lokale und gemeinsame Vorgaben, 01–04 und 08, Architektur/Testanleitung, Modell, Aktionen, Phasen, Geometrie, Eingabeansichten, KI, Setups, Tutorial, relevante Szenen-/Prefab-Referenzen und Testhelfer. Projekt- und Paketversionen sowie Git-Status geprüft. Nur diese Planungsdatei und ihr Verweis in 02 wurden für den Auftrag ergänzt. Umsetzung, Builds, automatische Tests und Spieltests sind nicht erfolgt; die Abnahme-Checkboxen in 02 bleiben offen.
