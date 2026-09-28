# 07 – Klarere UI, Auswertung und Ereignislog

Status: geplant. Voraussetzungen: 04, 05. Umfang: groß, in 07a–c umsetzen.

## Ergebnis

Spieler erkennen ihre geplanten Aktionen, deren Kosten und den Grund für Gewinne oder Verluste. Das Aktionsrad und die Voronoi-Gestaltung bleiben zunächst erhalten.

## Schritte

- [ ] 07a: Einheitliche Aktionssymbole mit Text erstellen: Bewegen, Stärke, Einkommen, Teilen und Warten. Begriffe in Tutorial, Tooltip, Panel und Log vereinheitlichen.
- [ ] Tooltip/Infokarte zeigt Name, Wirkung, Kosten, Wert vorher/nachher sowie Ablehnungsgrund. Stärke darf nicht versehentlich den Einkommenswert anzeigen.
- [ ] Gewählte Aktion dauerhaft am Ritter markieren; Ritterliste oder Auswahlpanel zeigt offene/gesetzte Züge. Plan ändern und abbrechen ermöglichen.
- [ ] Zielwahl klar markieren: erlaubter Bereich, ungültige Position und Bestätigungshinweis. Hinweise dürfen Zielklicks nicht abfangen.
- [ ] 07b: Evaluation Panel aus dem RoundResult erzeugen. Pro Ritter: Fläche vorher/nachher, Einnahmen, Aktionskosten, Unterhalt und Saldo; darunter Fraktionssumme und neues Guthaben.
- [ ] Entfernte Ritter mit ihrer abschließenden Buchung weiter in dieser Auswertung zeigen. Keine nachträgliche Rekonstruktion aus bereits gelöschten Objekten.
- [ ] Plus und Minus durch Vorzeichen, Farbe und Symbol unterscheiden; nicht allein über Rot/Grün. Zahlen und Währung einheitlich runden, intern nicht vorzeitig Genauigkeit verlieren.
- [ ] 07c: Kompaktes Textlog aus strukturierten Ereignissen, nach Runde gruppiert. Beispiele: „Ritter 7: Stärke 1,2 → 1,3“, „Unterhalt −80“, „Ritter 3 wegen Zahlungsunfähigkeit verloren“.
- [ ] Ereignisse anklickbar mit Kartenfokus, sofern der Ritter noch existiert; begrenzte Historie und optionaler vollständiger Diagnoseexport.
- [ ] Optionale Gebietsvorschau prototypisch beurteilen. Annahme „Gegner unverändert“ sichtbar machen und keine verborgenen gegnerischen Pläne verraten.

## Abnahme

- [ ] Nach jeder Aktion kann ein Testspieler sagen, was geplant ist und was es kostet.
- [ ] Panel, Tooltip, Log und Spielzustand stimmen in derselben Runde überein.
- [ ] Fraktionssaldo entspricht der Summe der ausgewiesenen Einzelbuchungen einschließlich ausgeschiedener Ritter.
- [ ] Kleine Fenster, Vollbild, lange Texte, mehrstellige Ritter-IDs und farblich ähnliche Fraktionen bleiben lesbar.
- [ ] Große Änderungen und Verluste sind erkennbar, ohne das vollständige Log lesen zu müssen.

## Einstieg und Nachweis

Einstieg: `LeaderPanel.cs`, `EvaluationPanel.cs` (in aktueller Spielszene deaktiviert), `MoneyLabel.cs`, `Preacher/RingMenu.cs`, `MenuButton.cs`, `PlannedActionController.cs`.

Noch offen: UI-Entwürfe, Iconset, Informationshierarchie und geprüfte Auflösungen.
