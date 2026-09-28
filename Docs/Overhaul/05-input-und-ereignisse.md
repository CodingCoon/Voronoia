# 05 – Neues Input System und klare Eingabezustände

Status: geplant. Voraussetzungen: 02, 04. Umfang: mittel.

## Ergebnis

Maus und Tastatur bedienen das Spiel über einheitliche Aktionen. UI, Karte, Menü und laufende Animationen erhalten Eingaben nur im passenden Zustand.

## Schritte

- [ ] Zur fixierten Unity-Version passendes Input-System-Paket einbinden und Paketversion festhalten. Aktuell steht Active Input Handling auf Legacy Input Manager.
- [ ] Input Action Asset mit Auswahl, Kontextmenü, Bestätigen, Abbrechen/ESC, Runde abschließen und Tempo einführen. UI und Gameplay über getrennte Action Maps steuern.
- [ ] Bestehendes uGUI mit EventSystem und InputSystemUIInputModule anbinden. Weltobjekte über geeigneten 2D-Raycaster/Pointer-Handler oder zentrale Kartenabfrage anbinden.
- [ ] `Input.GetMouseButtonDown`, OnMouse-Callbacks und eigenen Hover-Pollingpfad schrittweise ersetzen; pro Klick genau ein verantwortlicher Weg.
- [ ] UI-Verbrauch von Eingaben respektieren: Klick auf Panel, Menü oder Tooltip darf keine Karteaktion auslösen.
- [ ] Zustände definieren: freie Auswahl, Aktionswahl, Zielwahl, bestätigter Plan, Auflösung, Pause und Spielende. ESC bricht zuerst eine Zielwahl ab, öffnet sonst das Menü.
- [ ] Listener und Action Maps bei Szenenwechsel, Deaktivierung und Neustart sauber an-/abmelden.
- [ ] Gameplay-Ereignisse aus 02 von Geräteereignissen trennen. UI reagiert auf Auswahl-/Plan-/Rundenergebnis statt Geld und Labels überall in Update neu zusammenzubauen.
- [ ] „Both“ höchstens als Übergang verwenden; abschließend Legacy-Eingabe entfernen, wenn alle Pfade umgestellt sind. UI Toolkit ist dafür keine Pflichtmigration.

## Abnahme

- [ ] Linksklick, Rechtsklick, ESC und Bestätigen funktionieren im Windows- und Browserbuild.
- [ ] Kein Doppelklickpfad, kein Durchklicken durch UI, keine Aktionen während Auflösung oder Spielende.
- [ ] Fokusverlust, Fenstergrößenwechsel, Vollbild, Menü und wiederholte Szenenwechsel verlieren keine Eingaben und vervielfachen keine Listener.
- [ ] Browser-Kontextmenü und vom Browser reservierte Tasten werden im echten Build geprüft; alternative sichtbare Bedienelemente bleiben erreichbar.

## Einstieg und Nachweis

Einstieg: `Assets/Scripts/Map/MouseEventManager.cs`, `Preacher`, `MainMenuButton.cs`, `SkipButton.cs`, `NextButton.cs`, Szenen und Prefabs.

Noch offen: Action-Maps, Paketversion, Zustandsdiagramm und Bedienungsprüfungen.
