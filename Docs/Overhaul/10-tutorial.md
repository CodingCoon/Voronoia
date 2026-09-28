# 10 – Verständliches Tutorial

Status: geplant. Voraussetzungen: 07–09. Umfang: mittel.

## Ergebnis

Neue Spieler verstehen nicht nur die Mausbedienung, sondern auch Gebietskontrolle, Kosten und das Ziel wirtschaftlicher Verdrängung.

## Schritte

- [ ] Kleines, reproduzierbares Tutorialszenario mit festen Gegnerplänen erstellen. Keine zufällige KI darf eine Erklärung vorzeitig ungültig machen.
- [ ] Kurze Aufgabenfolge: Ritter wählen → bewegen → animierte Grenzänderung beobachten → Auswertung lesen → Stärke und Einkommen vergleichen → teilen → Unterhalt verstehen → warten → Sieg/Niederlage.
- [ ] Je Schritt eine konkrete Handlung oder Erkenntnis verlangen. Fortschritt an Spielereignisse koppeln, nicht an feste Animationsdauer.
- [ ] Vergleich Stärke/Einkommen mit sichtbaren Vorher-/Nachherwerten erklären. „Mehr Gebiet“ und „mehr Ertrag pro Fläche“ müssen unterscheidbar sein.
- [ ] Einen Gegenangriff über Außenräume zeigen: Zentrum muss nicht das beste Bewegungsziel sein.
- [ ] Markierungen direkt an Ritter, Aktion und Auswertung setzen; kurze Texte und dieselben Icons/Begriffe wie im normalen Spiel verwenden.
- [ ] Aufgaben robust gegen Abbrechen, neue Auswahl und falschen Klick machen. „Schritt wiederholen“, Überspringen und späteres Wiederöffnen ermöglichen.
- [ ] Tempo und Audio dürfen den Fortschritt nicht beeinflussen; Hilfen auch nach Tutorialabschluss erreichbar lassen.

## Abnahme

- [ ] Tutorial funktioniert von frischem Start bis Abschluss ohne Sackgasse.
- [ ] Mindestens zwei unvorbereitete Testpersonen können danach die vier Aktionen, Warten, Unterhalt und Siegbedingung in eigenen Worten erklären; Missverständnisse dokumentieren.
- [ ] Falsche Eingaben, schnelles Weiterklicken und Überspringen führen nicht zu blockierten Buttons.
- [ ] Texte und angezeigte Werte entsprechen der aktuellen Balance und werden aus gemeinsamen Daten gespeist, wo sinnvoll.

## Einstieg und Nachweis

Einstieg: `TutorialManager.cs`, `TutorialOverlayUI.cs`, `Tutorial/TutorialHelper.cs`, `Setups/TutorialSetup.cs`.

Noch offen: Ablauf, Texte, Szenario und Spieltestnotizen.
