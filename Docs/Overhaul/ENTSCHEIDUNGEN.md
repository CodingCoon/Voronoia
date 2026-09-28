# Regel- und Technikentscheidungen

Status: teilweise entschieden. Nicht ausdrücklich beschlossene Vorschläge bleiben offen.

Projektkontext und Quellenzuständigkeit: [Architecture](../../Architecture.md), [Developer Guide](../../DeveloperGuide.md) und [Überarbeitungsplan](README.md). Allgemeine Engineering-Defaults werden in der [Shared-Baseline](../../../shared-engineering-guidelines/README.md) gepflegt; diese Datei enthält die lokalen Spiel- und Technikentscheidungen.

| Thema | Vorschlag | Wann entscheiden? |
| --- | --- | --- |
| Plattformen | Beschlossen: Windows-x64-Development-Build und Desktop-WebGL in Edge und Firefox | 00 |
| Unity | Beschlossen: 6000.6.0f1 als feste Basis; Wechsel nur bei nachgewiesenem Blocker | 00 |
| Begrifflichkeit | In UI „Ritter“; Leader/Preacher intern erst bei betroffenen Änderungen vereinheitlichen | 07 |
| Gebietssystem | Power-Diagramm anhand Prototyp bewerten | 03 |
| Bibliothek | Clipper2 gegen kleinen Halbebenen-Clipper prüfen; NTS nur bei begründetem Zusatznutzen | 03 |
| Stärke | Spielwert über konfigurierbare Kurve in geometrisches Gewicht abbilden | 03, 12 |
| Zellverlust | Zelle darf leer werden; Ritterverlust und Zeitpunkt ausdrücklich festlegen | 03, 04 |
| Gleichstände | Identische Positionen/Gewichte durch stabile ID eindeutig auflösen; alternative Platzierungsbeschränkung prüfen | 03 |
| Bewegung/Teilen | Beschlossen für 02: Ziele nur im eigenen Rittergebiet am Planungsbeginn, mit 0,05 Welteinheiten Grenzabstand; Teilen zusätzlich mit diesem Abstand zum Elternritter | 02, 03, 04 |
| Rundenmodell | Alle planen auf demselben Stand; Ergebnis gemeinsam berechnen | 02 |
| Kosten | Beschlossen für 02: gemeinsame Abrechnung ohne neue Budgetreservierung; Rundeneinkommen darf Kosten decken. Positive Kostenwerte und Reservierung vorhandenen Geldes bleiben Vorschläge für 04 | 02, 04 |
| Insolvenz | Beschlossen: nur bei Geld < 0 den ältesten Ritter entfernen, danach Fraktionskonto auf 0 (siehe Protokoll) | 01, 04 |
| Gleichzeitiges Ausscheiden | Beschlossen: Bleibt keine Fraktion übrig, endet die Partie unentschieden | 01 |
| Animation | Beschlossen: DOTween bereits in 01 für betroffene Animationen; Entkopplung der Aktionsverträge in 02, Tempo/Überspringen in 08 | 01, 02, 08 |
| Warten | Explizite Aktion ohne Aktionskosten; normale Einnahmen und Unterhalt bleiben | 04 |
| Vorschau | Erwartete Änderung bei unveränderten gegnerischen Aktionen, klar als Annahme markiert | 07 |
| Tempo | 1×, 2×, 4× und direktes Ergebnis; gleiche Berechnung | 08 |
| Partielänge | Vorläufiges Testziel 10–20 Minuten, durch Spieltests prüfen | 12 |
| Teilnehmerzahl | Zunächst 1–5 KI-Gegner; höhere Zahlen nur nach Last- und Lesbarkeitstests | 12 |
| Savegame | Automatisch an gültigen Rundengrenzen; versioniertes Format und Rückfallstand | 06 |
| Hotseat | Verdeckte Planung mit Übergabebildschirm | 14 |
| Online | Autoritativer Host/Server; kein plattformübergreifender Float-Lockstep als Voraussetzung | 15 |
| Völker | Je ein Vorteil für Bewegung, Stärke, Einkommen oder Teilen; zunächst nur Datenmodifikatoren | 16 |

## Entscheidungsprotokoll

### 28.09.2026 – Zielbereich und Budgetumfang für 02

- Entscheidung des Nutzers: Bewegung und Teilen bleiben auf dem eigenen Land. Für die Umsetzung von 02 gilt das eigene Rittergebiet am gemeinsamen Planungsbeginn als Zielbereich.
- Entscheidung des Nutzers: Budgetempfehlung für 02 übernehmen. Kosten und Buchungen zentralisieren, aber keine Vorabreservierung einführen; das Rundeneinkommen darf weiterhin die Aktionskosten decken. Die KI verwendet denselben Validator wie der Mensch und übernimmt ihre bisherige fehlerhafte zusätzliche Budgetprüfung nicht. Budgetreservierung bleibt Gegenstand von 04.
- Geprüfte Alternative: gemeinsame Budgetreservierung aus 04 vorziehen; nicht gewählt, damit 02 auf die Entkopplung und atomare Rundenberechnung begrenzt bleibt.
- Entscheidung des Nutzers: Zielkonflikte bereits bei der Platzierung verhindern. Bewegung und Teilen halten 0,05 Welteinheiten Abstand zur Zellgrenze; Teilen hält denselben Abstand zum Elternritter. Die Vorschau wird auf einen gültigen Innenpunkt begrenzt oder bei zu schmalem Gebiet ausgeblendet. Der Resolver prüft die Regel erneut und verwirft bei einem dennoch widersprüchlichen Befehl die noch nicht übernommene Arbeitskopie.
- Folgen: gemeinsame Zielvalidierung für UI und KI; Regressionen für Zellgrenze, Elternposition und Finanzierung durch Rundeneinkommen. Kein neuer Budget-Grenzwert in 02.
- Nachweis: Planungsentscheidung und statische Codeprüfung; noch keine Umsetzung oder Spielprüfung.

### 28.09.2026 – Umsetzung von 01 und DOTween

- Entscheidung des Nutzers: 01 einschließlich DOTween, arabischen TMP-Ritternummern und einfachem Ergebnis-Panel umsetzen. Die vollständige Umstellung der Aktionsschnittstellen folgt in 02.
- Technische Festlegung: Alter entscheidet bei Insolvenz, bei Gleichstand die niedrigste Ritternummer. Nummern laufen weiterhin je Fraktion monoton und werden nach Verlust nicht wiederverwendet.
- Darstellung: Sieg, Niederlage, Unentschieden und kontrollierter Fehlerzustand erhalten einen explizit zugewiesenen Szenenbildschirm mit neuer Partie beziehungsweise Menü. Nicht behandelbare Geometrie hält die Runde an; keine Wiederaufnahme eines Teilzustands.
- Gebietsverteilung: Nach abgeschlossenen Verlustanimationen werden Gebiete für eine weiterlaufende Partie vor der nächsten Planung neu berechnet. Dabei wird kein weiteres Einkommen gebucht. Das Tutorial beschreibt diesen Zeitpunkt entsprechend.
- Abhängigkeit: DOTween 1.3.030; Quelle, Lizenz und Einbindung in [Dependencies](../Dependencies.md). Kein vorgezogener Austausch der Geometrie oder des Inputsystems.
- Nachweis: Ausgeführte Prüfungen und offene Plattformgrenzen stehen in [01](01-akute-fehler.md).

### 27.09.2026 – Insolvenz und Partieende (01, 04)

- Entscheidung des Nutzers: Geld = 0 ist zahlungsfähig. Erst bei negativem Kontostand verliert die Fraktion einen Ritter; anschließend wird ihr Kontostand auf 0 gesetzt. Es werden nicht mehrere Ritter entfernt, bis die Schulden gedeckt wären.
- Präzisierung des Nutzers: Entfernt wird der älteste Ritter, gemessen an RoundsExist. Es werden keine individuellen Schuldenkonten eingeführt. Bei gleichem Alter ist die niedrigste Ritternummer als deterministische technische Festlegung vorgesehen.
- Entscheidung des Nutzers: Scheiden alle Fraktionen gleichzeitig aus, gilt Unentschieden. Das Ergebnis wird nach Verarbeitung aller Verluste bestimmt, nicht aus der Reihenfolge einzelner Entfernungen.
- Folgen: Schuldenerlass muss als ausdrückliche Sonderbuchung nachvollziehbar bleiben. Tests unterscheiden Kontostand 0 und negative Werte; Verlust des letzten Ritters und gleichzeitiges Ausscheiden brauchen eigene Fälle.
- Nachweis: Regeln beschlossen, noch nicht implementiert oder im Spiel geprüft.

### 27.09.2026 – Vorgehen nach 00

- Entscheidung des Nutzers: Die noch offenen Playerbuilds und Spieltests aus 00 dürfen vorerst ignoriert werden und blockieren den Beginn von 01 nicht.
- Folgen: 00 bleibt unabgenommen; die Ausnahme ist kein Build- oder Spieltestnachweis und keine pauschale Aufhebung der gezielten Verifikation von 01.

### 27.09.2026 – Basisversion und Zielplattformen (00)

- Entscheidung und Grund: Unity 6000.6.0f1 bleibt die feste Basis des ersten reproduzierbaren Builds. Windows x64 als Development-Build und Desktop-WebGL in Edge und Firefox sind die verbindlichen Prüfziele. Die Version und beide Buildmodule sind auf dem Arbeitsrechner vorhanden.
- Geprüfte Alternative: sofortiger Patchwechsel oder nur Windows; beides würde den bereits begonnenen Upgradezustand beziehungsweise den bisherigen Browserweg schlechter abdecken. Mobilbrowser werden in 00 nicht abgenommen.
- Folgen: Buildhelfer und Prüfnachweis verwenden genau diese Version und Plattformen. Gameplay- und Savegame-Regeln ändern sich nicht.
- Nachweis: Build- und Spieltestergebnisse stehen in [00](00-unity-basis.md); eine erfolgreiche Migration wird erst nach dessen Abnahme behauptet.

### 27.09.2026 – Rendering für den Basisbuild (00)

- Entscheidung und Grund: Built-in-Rendering bleibt für Inkrement 00 aktiv. GraphicsSettings und alle Quality-Stufen verweisen auf keine Scriptable Render Pipeline; die vorhandenen Assets `UniversalRP` und `Renderer2D` sind nicht als Pipeline aktiviert.
- Geprüfte Alternative: URP installieren und zuweisen; dafür gibt es im Basisschritt keinen nachgewiesenen Bedarf.
- Folgen: Material- und Darstellungstests erfolgen mit dem bestehenden Renderer. Die unreferenzierten Assets bleiben erhalten.
- Nachweis: Konfiguration statisch geprüft; visuelle Player-Prüfung in [00](00-unity-basis.md).

Für jede beschlossene Änderung hier ergänzen:

- Datum und betroffene Inkremente:
- Entscheidung und Grund:
- Geprüfte Alternative:
- Folgen für Spielregeln, Spielstände, Tests und UI:
- Nachweis oder offener Prüfpunkt:

## Noch benötigte Produktinformationen

- Vorrangige Zielplattform, verfügbare Testgeräte und historische Crashberichte/Builds.
- Gewünschte Partielänge und Bedeutung von Planbarkeit gegenüber Überraschung.
- Soll ein Ritter sein eigenes Gebiet immer enthalten müssen? Ein Power-Diagramm garantiert das nicht.
- Für 04: Finanzierung durch Rundeneinkommen beibehalten oder auf vorhandenes Geld mit Reservierung umstellen? Für 02 ist das Bestandsverhalten beschlossen.
- Welche Folgeerweiterung hat nach dem Einzelspieler-Release Vorrang?

Diese Fragen blockieren die Bestandsaufnahme und die eindeutigen Fehlerkorrekturen nicht.
