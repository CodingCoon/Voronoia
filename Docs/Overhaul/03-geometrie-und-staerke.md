# 03 – Robuste Geometrie und wirksame Stärke

Status: geplant. Voraussetzung: 02. Umfang: groß; Prototyp vor Integration.

## Ergebnis

Gewichtete Gebiete decken die Karte verlässlich ab. Stärke hat eine messbare, verständliche Wirkung. Nähe, Ränder und verschwindende Zellen sind gültige Fälle.

## Entscheidung und Schritte

- [ ] 03a: Prototyp mit gewöhnlichem Voronoi und Power-Diagramm erstellen. Empfehlung: Gebiet nach kleinstem `Abstand² − Gewicht`; gleiche Gewichte ergeben gewöhnliches Voronoi.
- [ ] Bisherige paarweise Stärkeverhältnis-Grenzen als Vergleich darstellen. Sie können bereits mathematisch Lücken erzeugen; Clipper2 allein korrigiert diese Regel nicht.
- [ ] 03b: Zwei Implementierungsoptionen prüfen: konvexes Kartenpolygon sukzessive an Halbebenen abschneiden oder diese Schnitte mit Clipper2 ausführen. Clipper2 ist ein Polygonwerkzeug, kein fertiger gewichteter Voronoi-Generator.
- [ ] NetTopologySuite nur für gewöhnliches Voronoi oder als Testreferenz einsetzen, falls der Nutzen die zusätzliche Abhängigkeit rechtfertigt. Nicht vorsorglich beide Bibliotheken übernehmen.
- [ ] Paketversion, Lizenz, Einbindung und Unity-/WebGL-Buildfähigkeit der gewählten Lösung dokumentieren. Interne Präzision, Clipper-Skalierung und gemeinsame Grenzberechnung festlegen.
- [ ] 03c: Spielstärke über eine konfigurierbare Kurve in Gewicht umrechnen; Kartengröße berücksichtigen. Drei Referenzsituationen für schwachen/mittleren/starken Effekt festhalten.
- [ ] Identische Positionen und gleiche Gewichte eindeutig behandeln. Nähe darf nicht durch zufällige Verschiebungen „repariert“ werden, die Replays verändern.
- [ ] Leere Zellen, Ritter außerhalb der eigenen Zelle und vollständige Verdrängung als Regelentscheidung festlegen. Power-Diagramme erlauben diese Fälle.
- [ ] 03d: Polygonreinigung, korrekte Fläche und atomare Übergabe an Renderer/Collider integrieren. Nur bereinigte gültige Formen oder explizit leere Zellen weitergeben.

## Prüfungen und Abnahme

- [ ] 0/1/2 Ritter, viele Ritter, identische und sehr nahe Positionen, Rand, Ecke, kollineare Punkte sowie stark unterschiedliche Gewichte prüfen.
- [ ] Für nicht leere Teilnehmermenge: Zellvereinigung entspricht der Karte innerhalb dokumentierter Toleranz; keine flächigen Überlappungen. Zusätzlich Einzelzellgültigkeit prüfen – Flächensumme allein genügt nicht.
- [ ] Translation aller Punkte samt Karte ändert keine Flächen. Symmetrische Anordnungen mit gleichen Gewichten liefern symmetrische Ergebnisse.
- [ ] Bei festen übrigen Parametern wird die eigene Fläche durch höheres Gewicht nicht kleiner. Referenzsituationen zeigen einen wahrnehmbaren Zuwachs pro Upgrade; volle Kartenkontrolle darf sättigen.
- [ ] Zufallsfälle mit gespeichertem Seed und gezielte Grenzfälle laufen im Player-Build ohne NaN, Infinity, Hänger oder ungefangene Fehler.
- [ ] Messreihen etwa mit 6, 20, 50 und 100 Rittern erfassen; daraus unterstützte Größen und Leistungsbudget ableiten, keine Freigabe aller Größen vorwegnehmen.

## Einstieg und Nachweis

Einstieg: `Assets/Scripts/voronoi`, `Preacher/PreacherArea.cs`, `Data Model/impl/Leader.cs`.

Noch offen: Bibliotheksentscheidung, Stärkeabbildung, Kollisions-/Verdrängungsregel, Toleranzen, Benchmarks und Testseeds.
