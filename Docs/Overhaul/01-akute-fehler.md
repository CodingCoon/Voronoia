# 01 – Akute Fehler beheben

Status: implementiert und gezielt im Editor geprüft (28.09.2026); Plattform- und manuelle Gesamtabnahme offen. Voraussetzung: 00 mit unten dokumentierter Ausnahme. Umfang: mittel.

Planungsstand 27.09.2026: Laut Nutzerentscheidung dürfen die offenen Playerbuilds und Spieltests aus 00 vorerst zurückgestellt werden; sie blockieren den Beginn von 01 nicht. 00 wird dadurch nicht als abgenommen geführt. Die Regelbeschlüsse stehen in [Entscheidungen](ENTSCHEIDUNGEN.md).

## Ergebnis

Die eindeutig bekannten Abbruch- und Rechenfehler sind korrigiert und durch konkrete Reproduktionen abgesichert.

## Schritte

- [x] Ritter-ID von fünf fest verdrahteten Zahlensprites lösen. TextMeshPro-Zahl oder korrekt erzeugte römische Zahl verwenden; keine Wiederverwendung von IDs.
- [x] `Game.RemoveReligion()` tatsächlich entfernen lassen. Referenzen, menschliche Fraktion, Sieger/Verlierer und den Fall ohne verbleibende Fraktion korrigieren.
- [x] Einfaches Ergebnis-Panel in GameScene für Sieg, Niederlage und Unentschieden mit neuer Partie und Rückkehr ins Menü ergänzen. Bestandsprüfung: GameOverPhase ist vorhanden, zeigt beim Eintritt aber kein Ergebnis an.
- [x] `PreacherArea.SetBounds()` mit echten Colliderpunkten befüllen. Nicht initialisierte, leere und degenerierte Polygone kontrolliert behandeln.
- [x] Flächenformel korrigieren: vorzeichenbehaftete Kreuzprodukte summieren und erst die gesamte Summe absolut nehmen.
- [x] Insolvenz gemäß Entscheidungsprotokoll umsetzen. Schuldenerlass ausdrücklich ausweisen und Kontostand nicht auf den Unterhaltswert überschreiben. 04 auf den Beschluss verweisen lassen.
- [x] Stärke-Tooltip mit Power statt Income befüllen; tatsächliche Aktionskosten in der Auswertung erhalten, obwohl Action bereits zurückgesetzt wurde.
- [x] Vorläufige Geometriesicherungen einbauen: endliche Zahlen, parallele Linien, doppelte Randtreffer, identische Positionen, gültige Punktzahl. Keine stillen Teilaktualisierungen oder endloses Wiederholen fehlgeschlagener Neuberechnung.
- [x] Phasenwechsel während laufender Auflösung verhindern; entfernte Ritter, Auswahl und Vorschauen sauber freigeben. Fehler kontrolliert anzeigen und Neustart anbieten, bis 06 Wiederaufnahme ergänzt.

## Abnahme und Regressionen

- [x] Ritter mit IDs 5, 6, 10 und 20 können erzeugt und angezeigt werden.
- [x] Ein Quadrat behält bei Verschiebung und umgekehrter Punktreihenfolge seine Fläche.
- [x] Erste und letzte ausscheidende Fraktion sowie gleichzeitiges Ausscheiden führen zu einem definierten Zustand.
- [x] Beschlossene Insolvenzfälle und Schuldenerlass sind durch gezielte Regressionen abgesichert; die Ergebnisanzeige stimmt mit dem abschließend bestimmten Spielzustand überein.
- [x] Rand-/Eckfälle und gleiche Positionen erzeugen keine unbehandelte Exception oder blockierte Endlosschleife.
- [x] Wiederholtes Klicken auf Weiter kann Einkommen nicht doppelt auszahlen oder Phasen überspringen.

## Abgrenzung und Einstiegspunkte

### DOTween-Einführung

DOTween **1.3.030** ist als offizielles Paket eingebunden. Version, Lizenz, Module, Herkunft und Lebensdauerverwaltung sind in [Dependencies](../Dependencies.md) dokumentiert. Die DLL-Importmetadaten wurden durch Unity reserialisiert; die GUIDs bleiben erhalten.

- Die visuellen Schleifen für Ringmenü, Bewegung/Teilung, Einkommensanzeige und Todesanimation sind ersetzt. Komponenten beziehungsweise Phasen besitzen ihre Tweens; Ersetzen, Abbruch und Objektzerstörung beenden sie. ScreenBlend behält seine Coroutine und sperrt jetzt wiederholte Szenenwechsel.
- Die Phasensteuerung bleibt für zulässige Übergänge und genau einmalige Ausführung verantwortlich. Geometrieabschluss durch ein echtes Ergebnis signalisieren; feste Wartezeit nicht bloß durch einen Tween-Timer ersetzen.
- Regeländerungen nicht in neue Tween-Abschlusscallbacks verlagern. Die vollständige Trennung von Zustand, Aktionsausführung und Darstellung einschließlich der IEnumerator-Verträge folgt in 02; Tempo und Überspringen bleiben in 08.
- Die freie DOTween-Ausgabe genügt für die verwendeten Animationen. Windows-/WebGL-Kompatibilität ist bis zu echten Playerbuilds und deren Ausführung offen.

### Geometrie

Die bisherigen Gewichtungsregeln werden hier noch nicht als mathematisch korrekt erklärt. Lücken und Stärkeverhalten werden in 03 grundsätzlich gelöst. Keine umfassende Reparatur eines Algorithmus, der anschließend ersetzt wird.

Einstieg: `Data Model/impl/Leader.cs`, `Data Model/impl/Voronation.cs`, `Data Model/Game.cs`, `Preacher/PreacherArea.cs`, `Phases/DeathPhase.cs`, `voronoi/` unter `Assets/Scripts`.

## Umsetzungsnachweis

Geprüft am 28.09.2026 mit Unity **6000.6.0f1** in einer isolierten Projektkopie unter `Build/Increment01/Validation`; der bereits geöffnete Arbeitseditor wurde nicht geschlossen. Die Logs und Resultate liegen lokal im ignorierten Build-Verzeichnis.

| Prüfung | Ergebnis und Nachweis |
| --- | --- |
| Geometrie-NUnit/EditMode | **8/8 bestanden**, keine übersprungenen Tests. Translation, Windung, konkave Fläche, leere Daten, ungültige Zahlen, degenerierte Polygone und exakte Zielbegrenzung. `Build/Increment01/geometry-results.xml`, `geometry-tests.log`. |
| Bestandsintegration/EditMode | **84 Assertions bestanden**, Unity-Exitcode 0. IDs 5/6/10/20 und keine Wiederverwendung, echte Colliderpunkte, Aktionskosten und genau einmalige Buchung, Geld 0, ältester Ritter mit ID als Gleichstandsentscheidung, Schuldenerlass, Fraktionsentfernung und alle Spielausgänge, gültige Rand-/Eck-/kollineare Fälle, kontrolliert verworfene identische Positionen, serialisierte Pflichtreferenzen. `Build/Increment01/editmode-final.log`. |
| Automatisierter PlayMode-Ablauf | **PASS**: alle automatischen Phasen, wiederholte Weiter-Eingabe, genau einmaliges Einkommen, gemeinsames Ausscheiden aller sechs Fraktionen mit Unentschieden, Neustart und absichtlich ausgelöster Geometriefehler mit Eingabesperre. `Build/Increment01/playmode-3.log` und `Validation/Build/Increment01/playmode-integration.txt`. |
| Szene/Prefab | Über Unity authoriert und gespeichert, anschließend Pflichtreferenzen im Integrationstest geprüft. Keine manuell nachzutragenden Referenzen. Spiel-, Unentschieden- und Fehleransicht zusätzlich als Editor-Kamerabilder angesehen; abschließende Schriftgrößen-/Sorting-Layer-Anpassung gespeichert, aber kein vollständiger manueller Layouttest. |

Während der Reproduktionen gefundene Fehler wurden korrigiert: Ecktreffer wurden zuvor verworfen, Collider-Zielbegrenzung lag wegen Physics-Skin außerhalb der Karte, und der deaktivierte bestehende ScreenBlend verhinderte einen Neustart. Die betreffenden Regressionen bestanden danach. Ein früherer grafischer Prüflauf mit Bildaufnahme hing beim Abschluss; der anschließende Ablauf ohne Bildaufnahme bestand. Dies ist kein Nachweis für ausgeführte Player.

Offen bleiben Windows-x64/Mono- und WebGL/IL2CPP-Development-Builds samt Ausführung, WebGL in Edge/Firefox, vollständiges Tutorial sowie manuelle Bedienungs-/Layoutprüfung einschließlich Sieg, Niederlage und Menübutton. Die neue Abhängigkeit ist damit im Editor bestätigt, aber noch nicht auf den Zielplattformen abgenommen. Die zurückgestellten Prüfungen aus 00 bleiben separat offen. Konkrete Szenen-/Prefab-Zuordnungen und manueller Prüfschritt stehen im [Developer Guide](../../DeveloperGuide.md#unity-authoring-aus-01).

Die gewichtete Gebietsberechnung bleibt der Bestandsalgorithmus; mathematische Flächendeckung und Stärkeabbildung folgen in 03. Die vollständige Regel-/Darstellungstrennung und Rücknahme einer ganzen fehlerhaften Runde sind nicht Teil dieses Inkrements.
