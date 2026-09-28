# 00 – Unity-Basis und Bestandsaufnahme

Status: in Arbeit (27.09.2026). Voraussetzung: keine. Umfang: klein bis mittel, abhängig von Buildproblemen.

## Ergebnis

Ein reproduzierbarer Ausgangsstand auf Unity 6000.6.x mit dokumentierten Altfehlern und getesteten Zielplattformen.

## Schritte

- [x] Vorhandene Änderungen an Packages, ProjectSettings und Sprite-Metadaten erfassen und erhalten. Alten Git-Stand und aktuellen Arbeitsstand unterscheidbar sichern.
- [x] Vorhandene Version 6000.6.0f1 prüfen; gewünschte Patchversion, Buildmodule und API-Kompatibilität dokumentieren. Keine ungeprüfte Rückmigration auf 2023.
- [x] Paketbestand und tatsächliche Nutzung von URP/2D, SpriteShape, TextMeshPro und Test Framework prüfen. Kompatibilität in Player-Builds bleibt zu bestätigen.
- [x] Runtime-Code auf Editor-Abhängigkeiten prüfen; den ungenutzten `using UnityEditor.Search` in TutorialManager entfernen.
- [ ] Menü, Spielszene, Tutorial, Materialien, Fonts, Kamera und Audio auf Import-/Upgradeprobleme prüfen.
- [ ] Windows-Development-Build und frühen WebGL-Build erstellen. Browserbuild über lokalen HTTP-Server prüfen, nicht als lokale HTML-Datei.
- [ ] Reproduktionsliste anlegen: neue Partie, vier Aktionen, fünfter Ritter, Fraktionsverlust, Ränder/Ecken, Szenenwechsel und ESC.
- [ ] Startmessung für Rundenzeit, Geometriezeit und Speicherverbrauch erfassen; Gerät und Build angeben.

## Abnahme

- [ ] Ein zweiter Start aus dem dokumentierten Stand liefert denselben Buildweg ohne versteckte Editor-Konfiguration.
- [ ] Menü und Spiel laden in den gewählten Player-Builds. Bekannte Gameplayfehler werden mit Reproduktion separat erfasst.
- [ ] Keine ungeklärten Compilerfehler oder fehlenden Szenen-/Prefab-Referenzen.
- [ ] Unity-/Paketversionen und verbleibende Warnungen sind dokumentiert.

## Abgrenzung und Einstiegspunkte

Hier wird die Migration vervollständigt, noch keine neue UI gebaut. Die Eingabeumstellung folgt in 05, damit Enginewechsel und Bedienungsänderungen getrennt überprüfbar bleiben.

Einstieg: `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json`, `Packages/packages-lock.json`, `Assets/Scenes`, `Assets/Scripts/TutorialManager.cs`.

## Umsetzungsnachweis

### Gesicherter Stand und Konfiguration, 27.09.2026

- Historischer Referenzstand: Git-Commit `2ccffd0a5d7b4490bf29974ec0c182e5544c8206`, ohne Zusage, dass er dem veröffentlichten Jam-Build entspricht. Im ignorierten `Build/Increment00/Backup-2026-09-27` liegen ein ZIP dieses Commits, ein binärer Patch der zuvor vorhandenen Änderungen und Kopien der unversionierten Projektdateien. `.idea` wurde als lokale IDE-Konfiguration nicht in den Projektsnapshot übernommen.
- Bestehende Upgrade-Änderungen: Paketmanifest und Lockdatei, Player-/PackageManager-/VersionControl-Settings, ProjectVersion sowie zwei Sprite-Importmetadaten waren schon vor Beginn geändert. Die Sprite-GUIDs blieben erhalten; die neue Importserialisierung und `nameFileIdTable` werden nicht zurückgesetzt. Diese Änderungen wurden nicht als Umsetzung von 00 ausgegeben.
- Festgelegte Version: Unity **6000.6.0f1** (`f7f8ed4d1e24`). WindowsStandaloneSupport und WebGLSupport liegen unter den installierten PlaybackEngines. `ProjectSettings.asset` enthält `apiCompatibilityLevel: 6` und `activeInputHandler: 0` (Legacy-Input); Backend und effektive API-Einstellung werden beim Build vom Editor ausgelesen. Keine Input-Umstellung in 00.
- Pakete laut Lockdatei: 2D Feature 2.0.2, SpriteShape 16.0.0 transitiv; uGUI 2.6.0 enthält TextMeshPro (im Code verwendet); Test Framework 1.8.0. Das Universal Render Pipeline-Paket fehlt. `GraphicsSettings` und alle Quality-Stufen haben keinen Renderpipeline-Verweis. `UniversalRP.asset` und `Renderer2D.asset` bleiben als derzeit nicht zugewiesene Bestandsassets erhalten.
- Quellcodeprüfung: Unter `Assets/Scripts` war `TutorialManager.cs` der einzige Treffer für `UnityEditor`. Der Import wurde entfernt. Ein Editor-Buildhelfer liegt in `Assets/Editor`; Diagnoseausgaben sind auf Editor/Development beschränkt. Bekannte Editorwarnungen betreffen vier unnötige `new`-Member (`collider`), veraltetes `FindObjectOfType` im Singleton und unerreichbaren Code im SoundManager; sie sind in 00 keine Gameplayänderung.
- SpriteShape-Warnung: Beim Öffnen von `Menu.unity` und `GameScene.unity` wurde jeweils dreimal „Fill tessellation (C# Job) encountered errors“ protokolliert. Das Menü enthält drei `CCButton`-Instanzen; die Spielszene hat drei direkt gespeicherte statische SpriteShapes (`Background`, `TextField`, `SkipButton`). Deren `m_UTess2D` ist jetzt deaktiviert, sodass das Paket den Standardgenerator direkt nutzt. Die Zuordnung ist aus Häufigkeit und Szenenbestand abgeleitet, nicht über den Console-Kontext bestätigt. Nach erneutem Öffnen beider Szenen Warnungen und Darstellung prüfen. Tritt sie weiter auf, den betroffenen GameObject-Kontext im Unity-Console-Eintrag ermitteln und dessen Spline gesondert prüfen; dynamische Gebietsformen nicht pauschal umstellen.

### Build- und Spielnachweis

Ausführung gemäß [Developer Guide](../../DeveloperGuide.md#basisbuild-00): explizite Szenenfolge Menu → GameScene; Windows x64/Mono/Development, WebGL IL2CPP/Development. Frische Kopie enthält nur `Assets`, `Packages`, `ProjectSettings`; Unity importiert `Library` selbst. Ergebnisse für beide Plattformen, Browser und zweiten Kaltstart folgen nach tatsächlicher Ausführung.

Prüfprotokoll: je Plattform Menü → neue Partie → vier Aktionen und Abbruch/Planänderung → mindestens drei Runden → Tutorial → Menü → neue Partie; zusätzlich ESC, Material/Font/Kamera/Audio und Rand-/Eckpositionen. Fünfter Ritter und Fraktionsverlust mit Ausgangslage, Zugfolge, erwartetem und tatsächlichem Ergebnis gesondert dokumentieren. Bei Abbruch frische Partie beginnen und verbleibende Fälle unabhängig prüfen. Drei identische Zugfolgen im Standardsetup mit sechs Fraktionen liefern `Basis00`-Logwerte für automatische Rundenauflösung, Geometrie, Zellzuweisung und reservierten Unity-Speicher; Rechner, Build und Ritterzahl festhalten. Fehlgeschlagene Runden nicht mitteln.

Bekannter Bestandsbefund aus [01](01-akute-fehler.md): Die Spritezuordnung kann bei Ritter-ID 5 scheitern; die Fraktionsentfernung fügt im bestehenden Code eine Fraktion hinzu. Das ist bislang eine Codebeobachtung, kein auf 6000.6.0f1 reproduzierter Player-Fehler.
