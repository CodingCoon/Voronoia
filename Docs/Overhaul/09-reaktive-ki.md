# 09 – Unterschiedliche und reaktive KI

Status: geplant. Voraussetzungen: 03, 04. Umfang: groß; einfacher gemeinsamer Entscheider vor komplexer Suche.

## Ergebnis

Gegner wählen zwischen allen Kernaktionen und reagieren auf Gebiet, Wirtschaft und Bedrohungen. Außenräume sind echte strategische Kandidaten.

## Schritte

- [ ] 09a: Kandidaten erzeugen für Warten, Stärke, Einkommen, Teilen und mehrere Bewegungsziele. Gemeinsame Validierung und Budgetreservierung aus 04 nutzen.
- [ ] Bewegungskandidaten aus eigenem Gebiet, Nachbargrenzen, großen schwach besetzten Außenräumen und defensiven Rückzugspositionen ableiten. Ziele im erlaubten Bereich halten; keine feste Bevorzugung des Kartenursprungs.
- [ ] Kandidaten auf Zustandskopien mit derselben Geometrie/Wirtschaft wie das Spiel bewerten: Nettogewinn, Gebiet, gegnerischer Einkommensverlust, eigener Unterhalt, Risiko und mögliche Folgezüge.
- [ ] Pläne einer Fraktion gemeinsam budgetieren und gegenseitige Verdrängung eigener Ritter berücksichtigen.
- [ ] 09b: Profile als unterschiedliche Bewertungsgewichte: Expansiv (Raum/Teilung), Wirtschaftlich (Ertrag/Reserve), Defensiv (Verluste abfangen), Opportunistisch (ungeschützte Randgebiete/Gegenangriff).
- [ ] Profile bleiben reaktiv: wirtschaftlicher Gegner darf fliehen, expansiver darf warten. Keine starre Aktionsfolge je Profil.
- [ ] Schwierigkeit über Kandidatenzahl, Suchtiefe und nachvollziehbare Auswahlstreuung steuern. Keine Kenntnis verborgener menschlicher Zugpläne und keine kostenlosen Aktionen.
- [ ] 09c: Zeitbudget und begrenzte Sucharbeit einführen; nach Budgetende besten bislang gültigen Plan wählen. Nicht bei jeder Kandidatenprüfung Unity-Objekte erzeugen.
- [ ] Entwickleransicht mit gewählter Aktion und Bewertungsgründen sowie reproduzierbarem Seed ergänzen.

## Abnahme

- [ ] Szenariotest „stark besetztes Zentrum, lohnender Außenraum“ führt bei geeignetem Profil zu einer wirtschaftlich sinnvollen Außenbewegung oder Teilung.
- [ ] Weitere Szenarien: drohende Insolvenz, verlorene Grenze, große sichere Fläche, Bedrohung durch stärkeren Nachbarn und knappe Aktionsbudgets.
- [ ] Auf passenden Referenzkarten zeigen mindestens drei Profile unterschiedliche Entscheidungen; Unterschiede werden über Aktions-/Gebietsstatistik belegt.
- [ ] KI überschreitet weder Budget noch Zugzahl und beendet ihre Planung im festgelegten Zeitbudget.
- [ ] Wiederholung mit gleichem Zustand, Konfiguration und Seed ist innerhalb derselben Version reproduzierbar.

## Einstieg und Nachweis

Einstieg: `Assets/Scripts/AI`, `Setups/Random6PlayerSetup.cs`, neuer Befehlsvalidator und Rundenauflöser.

Noch offen: Profile, Referenzszenarien, Entscheidungsprotokolle und Laufzeiten.
