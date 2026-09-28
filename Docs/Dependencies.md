# Projektabhängigkeiten

## DOTween – Inkrement 01

- Zweck: Codegesteuerte Animation von Ritterbewegung, Teilung, Ringmenü, Einkommensanzeige und Ritterverlust. Regeln und Geldbuchungen gehören weiterhin dem Spielmodell beziehungsweise der Phasensteuerung.
- Version: **1.3.030**, freie Ausgabe, offizielles Paket vom 23.06.2026. Kein DOTween Pro erforderlich.
- Quelle: [Offizieller Download](https://dotween.demigiant.com/download.php), [Versioniertes ZIP](https://dotween.demigiant.com/downloads/DOTween_1_3_030.zip).
- SHA-256 des heruntergeladenen ZIP: `62A0ECECD274E1587EB0DEA15F3AFAB392FBDA5A0F8CAC7287FBF7F64925A1BA`.
- Lizenz: herstellereigene DOTween/Artistic-Lizenz, siehe [Lizenztext](https://dotween.demigiant.com/license.php). Kommerzielle und nichtkommerzielle Nutzung erlaubt; Weitergabe modifizierter Paketversionen eingeschränkt. Originale Quellen, Copyrightvermerke und `readme.txt` bleiben erhalten. Keine Änderung am Bibliotheksquellcode.
- Einbindung: offizielles Unity-Paket unter `Assets/Plugins/Demigiant/DOTween`; Kern als DLL, Modulassembly `DOTween.Modules`. Die Ressourcen-Einstellungen liegen in `Assets/Resources/DOTweenSettings.asset`. DOTweens Editorintegration ergänzt das Define `DOTWEEN` in den Player Settings und erzeugt die Modulassembly. Das ist kein UPM-Paket.
- Module: Audio, Physics, Physics2D, Sprite und UI sind in den erzeugten Einstellungen aktiviert; UIToolkit und externe/Pro-Integrationen sind deaktiviert. Die neuen Animationen verwenden vor allem Transform-Tweens und `DOVirtual.Float`; TMP-Werte werden direkt gesetzt und benötigen kein Pro-Modul.
- Lebensdauer: Komponenten besitzen ihre Tweens, ersetzen konkurrierende Animationen und beenden sie beim Deaktivieren. Die übergeordnete Einkommenssequenz ist an die Phase gebunden. `TweenPlayback.Wait` unterscheidet Abbruch von erfolgreichem Abschluss.
- Zielplattformen: Windows x64/Mono und WebGL/IL2CPP. Unity- und Plattformnachweise stehen ausschließlich unter [01 – Umsetzungsnachweis](Overhaul/01-akute-fehler.md#umsetzungsnachweis); Herstellerangaben ersetzen keinen Projektbuild.

Für Updates das offizielle Paket und dessen Setup-Anleitung verwenden. Version, Lizenz, Settings und tatsächliche Plattformprüfung erneut dokumentieren.
