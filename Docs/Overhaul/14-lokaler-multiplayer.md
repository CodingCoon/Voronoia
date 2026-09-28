# 14 – Optional: lokaler Multiplayer und Hotseat

Status: optionales Backlog. Voraussetzung: 13. Umfang: mittel für Hotseat; getrennte Geräte/UI zusätzlich.

## Ergebnis

Mehrere Menschen können an einer Partie teilnehmen. Hotseat ist der erste konkrete lokale Modus; gleichzeitige lokale Bedienung folgt nur bei Bedarf.

## Schritte

- [ ] Alle Annahmen „Fraktion 0 ist der einzige Mensch“ entfernen. Teilnehmer können Mensch oder KI sein; MatchConfig, Auswahl und Auswertung nutzen explizite Besitzer-IDs.
- [ ] Pro Runde planen Menschen nacheinander auf demselben Ausgangszustand. Erst wenn alle Pläne feststehen, wird gemeinsam aufgelöst.
- [ ] Übergabebildschirm zeigt „Weitergeben an …“; vorherige Pläne und sensitive UI-Auswahl verbergen. Nach Bestätigung darf der nächste Spieler planen.
- [ ] Festlegen, ob Geld, Gegnerwerte und vergangene Pläne öffentlich sind. Hotseat kann Bildschirme verbergen, aber keine Geheimhaltung gegen Zuschauer garantieren.
- [ ] Ausscheidende Menschen überspringen, KI-Teilnehmer integrieren und Siegeransicht neutral formulieren.
- [ ] Savegame enthält aktive Planungsposition und bereits abgegebene Pläne; Wiederaufnahme stellt Privatsphäre her, bevor ein Plan angezeigt wird.
- [ ] Optional danach: mehrere Controller mit getrenntem Fokus/PlayerInput und passenden UI-Kontexten. Das ist ein eigener Bedienungsmodus, kein Voraussetzung für Hotseat.

## Abnahme

- [ ] Zwei Menschen sowie zwei Menschen plus KI spielen vom Start bis zum Ende.
- [ ] Kein Spieler kann fremde Ritter steuern, Budget verändern oder einen bereits abgegebenen fremden Plan sehen.
- [ ] Teilnehmerwechsel, ESC, Speichern/Laden und Ausscheiden mitten in einer Partie funktionieren.
- [ ] Derselbe vollständige Befehlssatz ergibt unabhängig von der menschlichen Eingabereihenfolge dasselbe Rundenergebnis.

## Abgrenzung und Nachweis

Kein Netzwerk, keine Konten, keine Online-Lobby. Noch offen: Sichtbarkeitsregeln, Übergabeablauf und Testpartien.
