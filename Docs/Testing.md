# Voronation – Tests und Verifikation

Stand: 28.09.2026. Diese lokale Prüfpolitik ergänzt [Shared A4](../../shared-engineering-guidelines/agents.md). Einstieg: [Developer Guide](../DeveloperGuide.md), Systemkontext: [Architecture](../Architecture.md).

## Aktueller Prüfstand

- Unity Test Framework steht im Paketmanifest. `Assets/Tests/EditMode` enthält acht Geometrietests in `Voronation.Geometry.Tests`. `Assets/Tests/CoreEditMode` enthält zehn Spielkerntests in `Voronation.Core.Tests`; sie referenzieren `Voronation.Core` und `Voronation.Geometry`.
- In 01 sind diese acht Tests, 84 gezielte EditMode-Integrationsprüfungen und ein automatisierter PlayMode-Ablauf erfolgreich ausgeführt worden. Genaue Fälle und Grenzen stehen in [01](Overhaul/01-akute-fehler.md#umsetzungsnachweis).
- Für 02 wurden die zehn Kernregressionen mit Unitys Roslyn-Argumenten gebaut und unter Unitys Mono ohne Szene ausgeführt; alle bestanden. Der reguläre Unity-Test-Runner blieb wegen wiederholtem Verlust des lokalen Lizenzkanals im initialen Asset-Refresh hängen und ist deshalb ausdrücklich noch offen. Details stehen in [02](Overhaul/02-spielkern.md#umsetzungsnachweis).
- Der reproduzierbare Windows-/WebGL-Buildweg für Inkrement 00 ist im [Developer Guide](../DeveloperGuide.md) beschrieben. Tatsächlich erfolgreiche Builds und Spieltests werden ausschließlich in [00](Overhaul/00-unity-basis.md) nachgetragen.
- Editor-Helfer für Builds existieren als `Increment00Build` und `Increment01Build`; ein aktueller erfolgreicher Playerbuild und CI sind weiterhin nicht nachgewiesen.

## Welche Prüfung passt zur Änderung?

| Änderung | Erwartete Verifikation |
| --- | --- |
| Markdown/Verweise | Lokale Links, Dateipfade, Konsistenz zwischen Ist-Zustand und Plan prüfen; kein Unity-Build erforderlich. |
| Icon, Text, Panel-Layout | Gezielter visueller/Bedienungscheck in betroffener Szene und repräsentativer Auflösung; keine rein implementationstreuen Tests. |
| Geometrie, Geld, Kosten, Sieg-/Verlustregeln | Reproduzierbaren Fall als gezielten EditMode-Test ergänzen, anschließend betroffene Integration prüfen. |
| Lifecycle, Eingabe, Phasen-/Szenenwechsel | PlayMode-Tests, wo sinnvoll, und kurzer manueller Szenenablauf; bei Eingabeänderung relevanten Player-Build testen. |
| Neue Bibliothek, Pakete, Serialisierung oder Unity-Version | Echten Build auf betroffenen Zielplattformen prüfen; Editor-Kompilierung allein genügt nicht. |
| Speichern/WebGL/Online | Spezifische Plattform- und Fehlerfälle aus dem betreffenden Inkrement; keine Desktop-Ergebnisse auf Browser übertragen. |

Umfang nach Risiko wählen. Erfolgreiche Prüfungen nur bei neuen Änderungen oder offenen Befunden wiederholen. Blockierte Prüfungen mit Ursache und konkretem nächstem Prüfschritt dokumentieren, nicht als bestanden markieren.

## Testinfrastruktur und Ausführung

Geometrietests liegen unter `Assets/Tests/EditMode`. Die Bestandsintegration prüft `Assets/Editor/Increment01Verification.cs` in Assembly-CSharp-Editor; `Increment01PlayVerification.cs` startet einen kontrollierten Ablauf im echten Unity-PlayMode. Letzteres ist ein Editor-Prüfhelfer, keine NUnit-PlayMode-Testassembly.

Testassemblies müssen den tatsächlich extrahierten Runtime-Code referenzieren können. Eine asmdef-Testassembly kann nicht einfach auf beliebigen Code in der vordefinierten Assembly-CSharp verweisen. Deshalb die kleinste passende Runtime-/Test-Assembly-Grenze im Spielkern-Inkrement einrichten oder den geeigneten Unity-Prüfweg verwenden; keine ungeprüften asmdefs über das gesamte Assets-Verzeichnis legen. Assemblies und Scene-/Prefab-Kompatibilität anschließend im Build prüfen.

Tests über den Unity Test Runner ausführen. Verifizierte Batch-Einstiege mit Unity 6000.6.0f1 auf einer isolierten Projektkopie (siehe Developer Guide):

```text
-batchmode -nographics -projectPath <Kopie> -runTests -testPlatform EditMode -assemblyNames Voronation.Geometry.Tests -testResults <absoluter-Pfad>/geometry-results.xml -logFile <absoluter-Pfad>/geometry-tests.log
-batchmode -nographics -projectPath <Kopie> -runTests -testPlatform EditMode -assemblyNames Voronation.Core.Tests -testResults <absoluter-Pfad>/core-results.xml -logFile <absoluter-Pfad>/core-tests.log
-batchmode -nographics -projectPath <Kopie> -executeMethod Increment01Verification.RunEditMode -logFile <absoluter-Pfad>/editmode.log -quit
-batchmode -nographics -projectPath <Kopie> -executeMethod Increment01PlayVerification.Run -logFile <absoluter-Pfad>/playmode.log
```

Beim Test Runner und PlayMode-Helfer kein `-quit` ergänzen: Sie beenden den Prozess selbst nach Abschluss. Die Integrationshelfer schreiben Ergebnisdateien unter `Build/Increment01` in der geprüften Kopie. Der PlayMode-Helfer ersetzt für seine Reproduktion vorübergehend Setupwerte; ausschließlich in einer Projektkopie ausführen. Für explizite Neuerstellung der authorierten Assets existiert `Increment01Verification.PrepareAndRun`; normale Testwiederholungen verwenden `RunEditMode`.

Die Build-Einstiege für 01 heißen `Increment01Build.Windows` und `Increment01Build.WebGL`, mit den BuildTargets `StandaloneWindows64` beziehungsweise `WebGL` und `-quit`. Sie schreiben einen BuildReport-Kurzbefund und die Development-Player unter `Build/Increment01`. Ihre Existenz ist kein erfolgreicher Plattformnachweis.

## Zentrale Regressionen

- Ritter-IDs 5, 6, 10 und 20; wiederholtes Teilen und Entfernen.
- Identische/nahe Positionen, Kartenrand, Ecke, kollineare Punkte und stark unterschiedliche Gewichte.
- Flächentreue bei Translation; gültige Polygone, Kartenabdeckung und keine flächigen Überlappungen. Nur die Summe der Flächen zu prüfen ist unzureichend.
- Kosten genau einmal buchen; Budget nicht mehrfach reservieren; Einnahmen/Unterhalt und ausgewiesener Saldo stimmen überein.
- Erste/letzte ausscheidende Fraktion, gleichzeitiger Verlust und definierter Sieg/Gleichstand.
- Doppelte Bestätigung, Abbruch der Zielwahl, Menü/Szenenwechsel und verworfene fehlerhafte Runde.
- Später: identisches Ergebnis mit normaler, schneller und übersprungener Animation; gültige Savegame-Wiederherstellung.

Die verbindlichen Detailkriterien stehen in den jeweiligen [Inkrementen](Overhaul/README.md); hier bleibt nur der gemeinsame Prüfeinstieg.

## Manuelle Basisprüfung und Builds

1. Von `Assets/Scenes/Menu.unity` starten, neue Partie öffnen und einen Ritter auswählen.
2. Vier Aktionen sowie Planänderung/Abbruch prüfen; geplantes und ausgeführtes Ergebnis vergleichen.
3. Mehrere Runden einschließlich Ritter-/Fraktionsverlust durchspielen. Bei einem bekannten Altfehler Reproduktion und Phase dokumentieren.
4. Tutorial und Rückkehr ins Menü prüfen; Spiel erneut starten.
5. Nach Umsetzung der entsprechenden Funktionen Tempo, Auswertung, Logs und Wiederaufnahme ergänzen.

Windows-x64-Development-Build und Desktop-WebGL in Edge und Firefox sind für Inkrement 00 beschlossen. Szenenliste, Development-Option und Ausgabeordner stehen im [Developer Guide](../DeveloperGuide.md); BuildReport und Exitcode bei jeder Prüfung festhalten. WebGL über einen HTTP-Server öffnen und Browsername/-version dokumentieren. Ein Desktoptest ersetzt weder Browserpersistenz- noch Browserinputprüfung.

## Nachweisformat

Ergebnisse in der betroffenen Inkrementdatei unter Umsetzungsnachweis festhalten:

```text
Datum / geprüfter Code- oder Buildstand:
Unity-Version / Plattform / Browser / Gerät, soweit relevant:
Prüfung und Ausgangssituation, Seed oder Zugfolge:
Erwartetes Ergebnis:
Tatsächliches Ergebnis:
Bestanden / fehlgeschlagen / nicht ausführbar:
Offene Einschränkung und nächster Schritt:
```

Für Releasefreigabe gilt zusätzlich [13 – Releaseprüfung](Overhaul/13-releasepruefung.md). Ein Bericht über statisch geprüfte Dokumente darf keine Aussage „Spiel getestet“ enthalten.
