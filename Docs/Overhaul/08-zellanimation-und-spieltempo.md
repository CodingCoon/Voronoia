# 08 – Animierte Zelländerungen und Spieltempo

Status: geplant. Voraussetzungen: 03, 05, 07. Umfang: groß; technischer Prototyp zuerst.

## Ergebnis

Beim Auflösen einer Runde verschieben sich Ritter und Grenzen nachvollziehbar. Spieler können das Tempo ändern oder direkt zum Ergebnis springen, ohne andere Regeln zu bekommen.

## Schritte

- [ ] 08a: Darstellung aus Vorher-/Nachherzustand animieren. Positionen und Gewichte interpolieren und daraus reine Vorschaupolygone berechnen.
- [ ] Nicht Polygonvertex Nummer i blind auf Vertex i des Zielpolygons interpolieren: Zellnachbarn und Punktzahlen können wechseln, Zellen entstehen oder verschwinden.
- [ ] Gemeinsame Grenzen kohärent berechnen. Teilen als klaren Entstehungsübergang inszenieren; gleiche Startpositionen nach definierter Geometrieregel behandeln. Verlust/Verdrängung sichtbar auflösen.
- [ ] Vorschaugeometrie strikt von abrechnungsrelevanten Flächen und aktiven Collidern trennen. Während Auflösung keine Zielwahl auf Zwischenformen erlauben.
- [ ] Leistungsbudget messen: Vorschau mit begrenzter Aktualisierungsrate und wiederverwendeten Puffern berechnen. Bei großen Partien qualitätsreduzierten Übergang vorsehen; gültiges Endbild immer exakt aus Endzustand setzen.
- [ ] 08b: Tempo 1×/2×/4× und „Ergebnis sofort“ anbieten, Einstellung merken. Zentraler Präsentationstakt statt verteilte feste WaitForSeconds-Zeiten.
- [ ] Tempo während einer Animation ändern; Überspringen beendet alle betroffenen visuellen Übergänge sauber und zeigt vollständige Auswertung.
- [ ] Tutorialhinweise und Lesen von Tooltips bleiben in normaler Zeit. Weniger Animation als Komfortoption anbieten.
- [ ] Audio bei hohem Tempo bündeln, statt dieselben Geräusche unkontrolliert zu stapeln.

## Abnahme

- [ ] Bewegung, Stärke, Teilung und Verlust zeigen verständliche Übergänge ohne ungültige Meshes oder dauerhafte Lücken.
- [ ] Normal, schnell und übersprungen ergeben identische Zustände, Logs und Buchungen.
- [ ] Wechsel des Tempos oder Überspringen in jeder Übergangsphase erzeugt keine Restobjekte oder gesperrte Eingabe.
- [ ] Grenzfälle mit wechselnder Eckpunktzahl, verschwindender Zelle und mehreren Teilungen sind geprüft.
- [ ] Profiling im Browserbuild bestätigt das festgelegte Leistungsbudget oder aktiviert den dokumentierten Darstellungsfallback.

## Einstieg und Nachweis

Einstieg: `Phases/ApplyPhase.cs`, `Phases/VoronoiPhase.cs`, `Preacher/PreacherArea.cs`, `Actions`, `ScreenBlend.cs` sowie RoundResult.

Noch offen: Animationsprototyp, Qualitätsstufen, Leistungswerte und Endzustandsvergleich.
