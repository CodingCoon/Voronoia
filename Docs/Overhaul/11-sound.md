# 11 – Mehr und klarere Soundeffekte

Status: geplant. Voraussetzungen: 05, 07, 08. Umfang: klein bis mittel.

## Ergebnis

Sound bestätigt Bedienung und wichtige Spielereignisse. Aktionen sind akustisch unterscheidbar; hohe Geschwindigkeit bleibt angenehm.

## Schritte

- [ ] Bestehende Clips inventarisieren und Lizenzen/Herkunft neuer Assets dokumentieren.
- [ ] Getrennte Signale für Auswahl, bestätigten Plan, Abbruch, ungültige Aktion, Bewegung, Stärke, Einkommen, Teilung, Rundenende, Ritterverlust, Sieg und Niederlage zuordnen.
- [ ] Prioritäten festlegen: Verlust/Sieg deutlich, häufige Hover-/Auswahlgeräusche dezent. Wiederholungsgrenzen und maximale gleichzeitige Stimmen vorsehen.
- [ ] Fachereignisse aus 02 verwenden; einmalige Ereignisverarbeitung verhindert doppelte Sounds nach UI-Refresh oder Wiederaufnahme.
- [ ] Ungeprüfte String-Schlüssel durch typisierte Sound-IDs ersetzen. Fehlende optionale Clips dürfen das Spiel nicht abbrechen.
- [ ] Getrennte Lautstärken für Master, Musik und Effekte speichern. Bestehende deaktivierte Musiklogik nur reparieren, wenn Musik gewünscht und vorhanden ist.
- [ ] Bei 2×/4×/Überspringen kurze Zusammenfassung statt Soundflut abspielen; Pitch nicht automatisch an Spieltempo binden.
- [ ] Browser-Audiostart nach Nutzerinteraktion, Fokuswechsel und Stummschaltung prüfen.

## Abnahme

- [ ] Stärke und Einkommen sind auch akustisch unterscheidbar; wesentliche Informationen bleiben zusätzlich visuell erkennbar.
- [ ] Viele gleichzeitige Aktionen clippen nicht hörbar und überschreiten die festgelegte Stimmenzahl nicht.
- [ ] Mute und Lautstärke bleiben nach Szenenwechsel und Neustart erhalten.
- [ ] Fehlender Clip, schnelles Überspringen und Browser-Fokusverlust verursachen keine Fehler.

## Einstieg und Nachweis

Einstieg: `Assets/Scripts/SFX`, `Assets/SFX`, Aktions- und UI-Ereignisse.

Noch offen: Soundliste, Assetnachweise, Mixerwerte und Hörtest.
