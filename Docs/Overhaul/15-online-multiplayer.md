# 15 – Optional: Online-Multiplayer

Status: optionales Backlog. Voraussetzungen: stabiler Spielkern, Savegame und Mehrteilnehmermodell; 14 empfohlen. Umfang: groß, als eigenes Projektpaket behandeln.

## Ergebnis

Eine kleine Online-Partie mit validierten Befehlen und Wiederverbindung. Netzwerkfehler dürfen nicht zu unterschiedlichen Spielständen oder verlorenen Runden führen.

## Schritte

- [ ] 15a: Zielmodus festlegen: private Räume mit Code und rundenweiser Planung als erster Umfang. Asynchrones Spielen über Tage wäre ein separates Persistenz-/Benachrichtigungsprodukt.
- [ ] Transport-/Hostingoption anhand Windows/WebGL, Betriebskosten und Reconnect vergleichen. Erst danach Bibliothek und Dienst auswählen; keine voreilige Kopplung an ein Netzwerkpaket.
- [ ] Autoritativen Host oder Server festlegen. Clients schicken Befehle; Autorität prüft Besitz, Runde, Budget, Ziele und Zugzahl und berechnet den gültigen Folgezustand.
- [ ] Zustände/Ergebnisse synchronisieren, nicht jeden Animationsframe. Floatberechnungen auf verschiedenen Plattformen nicht ungeprüft als bitgleich voraussetzen.
- [ ] 15b: Lobby, Beitreten/Verlassen, Bereitschaft und gemeinsame Regel-/Buildversion implementieren. Geheime Pläne nicht vor Rundenabschluss an andere Clients senden.
- [ ] Round-ID und Command-ID nutzen, damit Wiederholungen und verspätete Nachrichten keine Aktion doppelt ausführen. Autoritativen Snapshot und Diagnosevergleich vorsehen.
- [ ] 15c: Reconnect mit vollständigem gültigem Zustand; Zeitlimit, Aufgabe und Ersatz durch KI ausdrücklich regeln.
- [ ] Hostverlust bewusst behandeln: zunächst definierter Abbruch mit fortsetzbarem Stand oder dedizierter Server. Hostmigration erst nach eigenem Aufwandentscheid.
- [ ] Speicher-/Netzwerkgrenzen validieren und missgebildete Befehle abweisen. Hostingkosten und benötigte Zugangsdaten erst bei konkreter Dienstwahl klären.

## Abnahme

- [ ] Zwei reale Clients spielen auf den unterstützten Plattformen eine vollständige Partie.
- [ ] Verzögerung, Verbindungsabbruch, Wiederverbindung, doppelte und veraltete Befehle sowie Versionskonflikte sind geprüft.
- [ ] Client kann fremde Ritter, Geld oder Phasen nicht durch manipulierte Befehle ändern.
- [ ] Nach Reconnect stimmen Round-ID und autoritativer Zustand überein; Animationen dürfen lokal anders weit sein.
- [ ] Ausfall der Autorität hat einen sichtbaren, dokumentierten Ablauf.

## Abgrenzung und Nachweis

Matchmaking, Ranglisten, Chat, Konten und Hostmigration gehören nicht automatisch zum ersten Online-Inkrement. Noch offen: Architekturentscheidung, Kosten, Protokoll und Netzwerktests.
