# 04 – Wirtschaft, Aktionskosten und Warten

Status: geplant. Voraussetzung: 03. Umfang: mittel.

## Ergebnis

Kosten, Einnahmen und Unterhalt folgen einer einzigen Regelquelle. Warten ist eine bewusste, wirtschaftlich sinnvolle Option; ungültige Aktionen haben einen verständlichen Ablehnungsgrund.

## Ausgangslage

Aktionen sind im Code bereits kostenpflichtig: Bewegung kostet 10 pro Entfernungseinheit, Teilen 25, Stärke/Einkommensupgrade jeweils 40 mal dem nächsten Faktor. Unterhalt beträgt derzeit 40 mal Ritteralter je Runde. Die Werte sind Bestandsaufnahme, keine Zielbalance. Die UI und KI behandeln Kosten nicht überall konsistent.

## Schritte

- [ ] Kosten als positive Beträge führen; Buchungen getrennt als positive Einnahmen und negative Ausgaben erfassen. Zentral konfigurierbare Regeln statt verteilter Konstanten verwenden.
- [ ] Festlegen, ob Kosten vorab bezahlt oder bei der Abrechnung abgezogen werden. Empfehlung: vorhandenes Geld bei Planung reservieren; genau einmal bei Ausführung abbuchen, keine verdeckte Finanzierung mit ungewissen Einnahmen.
- [ ] Ändern/Abbrechen einer Aktion gibt ihre Reservierung frei. KI und Mensch nutzen denselben Validator und dieselbe Budgetberechnung.
- [ ] Explizite Aktion „Warten“ anbieten: keine Aktionskosten, ein geplanter Zug, normaler Unterhalt/Einkommen. „Noch nicht geplant“ davon unterscheiden und vor Rundenabschluss verständlich behandeln.
- [ ] Unmögliche Aktionen deaktivieren und Grund anzeigen: Geldmangel, falsche Phase, ungültiges Ziel, Platzierungskollision oder begründetes Limit.
- [ ] Nur die betroffene Aktion blockieren; Ritter nie durch eine UI-Sperre unbedienbar machen. Nach Regeländerung oder Laden Pläne erneut prüfen.
- [ ] Die in [Entscheidungen](ENTSCHEIDUNGEN.md) für 01 beschlossene Insolvenzregel in die zentrale Wirtschaft übernehmen und Zeitpunkt der Gebietsneuverteilung festlegen. Schuldenerlass als ausdrückliche Sonderbuchung berücksichtigen.
- [ ] Einnahmen einmal aus dem gültigen Rundenergebnis berechnen; Verlust und Gebietsumverteilung dürfen keine zweite Einkommensrunde auslösen.
- [ ] Stärke-/Einkommenszuwachs und Teilung verifizieren: geerbte Werte, Ritteralter, erster Unterhalt und Zahl der Aktionen eines neuen Ritters dokumentieren.

## Abnahme

- [ ] Für jede Fraktion gilt: Endgeld = Startgeld + Einnahmen − Aktionskosten − Unterhalt + ausdrücklich definierte Sonderbuchungen.
- [ ] Angezeigte Kosten, reserviertes Budget und spätere Buchung stimmen überein.
- [ ] Zwei Ritter können nicht dasselbe verfügbare Geld mehrfach verplanen.
- [ ] Warten erhält Geld gegenüber einer kostenpflichtigen Alternative, pausiert aber nicht den Unterhalt.
- [ ] Tests für exakt ausreichendes Geld, 0 Geld, Schulden, Planwechsel, Ritterverlust und gleichzeitige Verluste bestehen.

## Einstieg und Nachweis

Einstieg: `Assets/Scripts/Actions`, `Data Model/impl`, `AI`, `EvaluationPanel.cs`.

Noch offen: verbindliche Wirtschaftsregeln, Konfiguration und Abrechnungstests.
