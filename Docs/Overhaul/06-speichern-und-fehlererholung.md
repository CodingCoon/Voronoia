# 06 – Speichern, Wiederaufnahme und Fehlererholung

Status: geplant. Voraussetzungen: 02–04. Umfang: mittel bis groß.

## Ergebnis

Nach Schließen, Neuladen oder einem Fehler kann die letzte vollständig gültige Runde wiederhergestellt werden. Ein defektes Zwischenergebnis überschreibt keinen guten Spielstand.

## Schritte

- [ ] Versioniertes Savegame für Spielzustand, Regeln/Balanceversion, stabile IDs, Rundennummer, Seed und erforderlichen Zufallszustand definieren. Unity-Objektreferenzen nicht serialisieren.
- [ ] Autosave zum Start einer gültigen Planungsrunde sowie nach vollständig übernommener Auflösung schreiben. Optionale gespeicherte Zugpläne als getrennte Erweiterung behandeln.
- [ ] Windows: temporären Stand schreiben, validieren und sicher ersetzen; vorherigen gültigen Stand behalten.
- [ ] WebGL: Browserpersistenz und tatsächlichen Abschluss des Speicherns prüfen. Nicht auf Dateisystemsemantik oder einen garantiert ausgeführten Tab-Schließen-Callback vertrauen.
- [ ] Laden validiert Schema, endliche Zahlen, IDs, Besitzer, Limits und Spielregeln. Unbekannte Version oder beschädigte Datei gibt eine verständliche Meldung; Rückfallstand anbieten.
- [ ] Fehler während Rundenberechnung abfangen: Arbeitskopie verwerfen, letzten gültigen Stand erhalten, Wiederladen und Menü anbieten. Keine Exceptions nur unterdrücken und mit inkonsistentem Zustand fortfahren.
- [ ] Manuellen Diagnoseexport mit Buildversion, Seed, Befehlen, letztem gültigen Zustand und Fehler ergänzen. Keine automatische Übertragung und keine Konten erforderlich.
- [ ] Einstellungen wie Tempo und Lautstärke getrennt vom Partiestand speichern.

## Abnahme

- [ ] Speichern/Laden erhält den fachlichen Zustand und die nächste Rundenberechnung.
- [ ] Erzwungener Fehler vor, während und nach Berechnung führt zu einem gültigen Wiederaufnahmepunkt ohne Doppelbuchung.
- [ ] Beschädigter oder inkompatibler Spielstand blockiert kein neues Spiel.
- [ ] Browser-Reload und Windows-Neustart sind getestet; nicht verfügbarer Browserspeicher wird gemeldet, nicht als erfolgreicher Save ausgegeben.
- [ ] Speichern bei ausgeschiedener Fraktion, leerer Zelle und vielen Rittern funktioniert.

## Einstieg und Nachweis

Einstieg: neuer Spielkern aus 02, GameManager, Menü. Ein Storage-Adapter pro Plattform ist ausreichend.

Noch offen: Format, Plattformadapter, Wiederherstellungstests und Diagnosebeispiel.
