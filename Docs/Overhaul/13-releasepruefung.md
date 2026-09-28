# 13 – Einzelspieler-Release absichern

Status: geplant. Voraussetzungen: 00–12. Umfang: mittel; Fehlerkorrekturen nach Befund.

## Ergebnis

Ein nachvollziehbar geprüfter Releasekandidat der überarbeiteten Einzelspieler-Version. Veröffentlichung ist ein eigener, ausdrücklich beauftragter Schritt.

## Schritte

- [ ] Regressionen aus allen Inkrementen bündeln: Ritter-IDs >5, Nähe/Gleichheit, Rand/Ecke, starke Gewichte, leere Zellen, Wirtschaft, Fraktionsverlust, Sieg/Niederlage und Szenenwechsel.
- [ ] Automatisierte gültige Zugfolgen über viele Seeds und Runden bis Spielende oder dokumentiertes Rundenlimit ausführen. Fehlerhafte Seeds reproduzierbar sichern.
- [ ] Mehrere vollständige manuelle Partien mit verschiedenen Profilen und Teilnehmerzahlen testen, einschließlich Tutorial und Wiederaufnahme.
- [ ] Windows und WebGL separat prüfen. Browsermatrix vorab festlegen, etwa aktuelle Desktopversionen von Firefox und Chromium, mit tatsächlichen Versionsnummern im Bericht.
- [ ] Kleine Fenster, Vollbild, Fokusverlust, mehrfacher Neustart, Speedwechsel, fehlendes Audio und nicht verfügbarer Speicher prüfen.
- [ ] CPU-Zeit für Geometrie/KI, Bildrate während Zellanimation, GC-Spitzen und Speicherentwicklung messen. Langes Spielen und häufiges Teilen/Entfernen dürfen keine fortlaufende Objektansammlung erzeugen.
- [ ] Entwicklungsexport gegen Releasebuild abgleichen: Loggingumfang, Debuganzeigen, Assetlizenzen, Spielanleitung und Versionskennung.
- [ ] Historischen Jam-Build separat erhalten. Änderungen und verbleibende Einschränkungen für die neue Version beschreiben.

## Freigabekriterien

- [ ] Keine bekannten reproduzierbaren Blocker oder fehlerhaften Geld-/Gebietsberechnungen in unterstützten Konfigurationen.
- [ ] Ein erzwungener Fehler lässt den letzten gültigen Stand wiederherstellen.
- [ ] Alle beworbenen Modi, Teilnehmerzahlen und Bedienelemente wurden im Player-Build geprüft.
- [ ] Testbericht nennt Umfang und Grenzen; keine absolute Garantie „absturzfrei“.
- [ ] Optionales Feedback mit Diagnoseexport funktioniert, ohne automatisch Daten zu übertragen.

## Nachweis

Noch offen: Buildversionen, Testmatrix, Messwerte, bekannte Probleme und Releasehinweise.
