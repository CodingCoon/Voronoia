# 12 – Partieauswahl und Balancing

Status: geplant. Voraussetzungen: 04, 07, 09. Umfang: mittel bis groß.

## Ergebnis

Spieler wählen Gegnerzahl, Farbe und Schwierigkeit. Kernaktionen und KI-Profile bieten nachvollziehbare Alternativen über eine überschaubare Partiedauer.

## Schritte

- [ ] 12a: MatchConfig einführen: Teilnehmer, menschlich/KI, KI-Profil/Schwierigkeit, Farbe, Startaufstellung, Seed und Regelkonfiguration. Setup-Code nicht weiter auf sechs feste Einträge aufbauen.
- [ ] Startmenü zunächst für 1–5 KI-Gegner, eigene Farbe und Schwierigkeitsgrad anbieten. Andere Zahlen erst nach Prüfung freigeben.
- [ ] Farben auf Kontrast und Unterscheidbarkeit prüfen; zusätzliche Symbole/Umrisse verwenden. Ungültige Konfigurationen mit Grund ablehnen.
- [ ] Faire Ausgangslagen für unterschiedliche Teilnehmerzahlen schaffen; Kreisaufstellung auf quadratischer Karte nicht automatisch als gleichwertig ansehen.
- [ ] 12b: Balancewerte zentral konfigurieren: Startgeld, Einkommen pro Fläche, Stärkeabbildung, Upgrade-/Bewegungs-/Teilungskosten, Altersunterhalt und mögliche Grenzen.
- [ ] Referenzsituationen vergleichen: Stärke gegen Einkommen, Bewegung gegen Upgrade, Teilung gegen Unterhalt, Warten gegen Investieren, Zentrum gegen Außenraum.
- [ ] Viele kurze Simulationen mit festen Seeds und anschließend menschliche Spieltests durchführen. Siege nach Startposition/Profil, Rundenzahl, Geldentwicklung und Aktionshäufigkeit erfassen.
- [ ] Eine dominante Strategie zunächst erklären und messen, dann gezielt einen Parameter ändern. Niedrige Nutzung einer Aktion allein beweist keine Nutzlosigkeit.
- [ ] Vorläufiges Ziel 10–20 Minuten pro Partie prüfen; Tempooption und Zahl der Ritter berücksichtigen. Ziel bei Bedarf bewusst ändern.

## Abnahme

- [ ] Jede angebotene Teilnehmerzahl und Farbe startet ein gültiges Spiel; Speichern/Laden erhält die Konfiguration.
- [ ] Mindestens eine Referenzsituation begründet jede Kernaktion sowie Warten als sinnvolle Wahl.
- [ ] Stärke hat einen sichtbaren Nutzen, ohne jede Positionierung zu verdrängen; Teilung ist weder zwingende Dauerschleife noch wirtschaftlich immer schlecht.
- [ ] Balancebericht nennt Testumfang, Seeds, Annahmen und offene Ausreißer. „Ausgewogen“ wird nicht nur aus einzelnen Partien abgeleitet.

## Einstieg und Nachweis

Einstieg: `Setups`, Menüszene, Regelkonfiguration, KI und Simulationsprüfungen.

Noch offen: Startmenü, freigegebene Konfigurationen und Balancebericht.
