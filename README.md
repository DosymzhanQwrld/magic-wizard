# Magic Wizard

A 2D mobile platformer made in Unity 6 for Game Development Fundamentals (Narxoz University).
A wizard jumps across platforms, burns monsters with fireballs and reaches the portal.

## Controls

| Input | Action |
|---|---|
| Joystick / A, D | Move |
| JUMP / Space | Jump |
| FIRE / J | Fireball |

## Enemies

| Enemy | Script | States |
|---|---|---|
| Goblin | `GoblinEnemy.cs` | Patrol, Chase, WindUp, Lunge, Cooldown |
| Flying Eye | `FlyingEyeEnemy.cs` | Hover, WindUp, Dive, Return |
| Skeleton Archer | `ArcherEnemy.cs` | Idle, Aim, Shoot, Cooldown |
| Flying Demon (boss) | `BossEnemy.cs` | Sleep, Fly, WindUp, Shoot, Dead; below 50% HP it shoots 3 fireballs, 2x faster |

## Assets

- Wizard Pack by LuizMelo: https://luizmelo.itch.io/wizard-pack
- Monsters Creatures Fantasy by LuizMelo: https://luizmelo.itch.io/monsters-creatures-fantasy
- Dark Forest Platform Pack by Atari Boy (ground tiles and backgrounds): https://atari-boy.itch.io/dark-forest-platform-pack
- Flying Demon 2D Pixel Art (boss and boss fireball), free asset pack from itch.io; its License.txt allows use in any game project.
- Cave Platformer Tileset [16x16][FREE] by RottingPixels (Level 2 ground and background): https://rottingpixels.itch.io/cave-platformer-tileset-16x16free
- Free Lava Platform Tileset by Franco Giachetti, LudicArts.com, licensed under CC BY 3.0 (Level 3 ground and background): https://ludicarts.itch.io/volcano-free-platform-tileset
- Heart icon, button frame and lava are generated in the project.

## Audio

| Event | File (`Assets/Audio`) | Source clip | Played from |
|---|---|---|---|
| Jump | `SFX/jump.wav`, `jump_2.wav`, `jump_3.wav` | Kenney Digital Audio: phaseJump2, phaseJump1, phaseJump3 | `PlayerController` |
| Fireball | `SFX/fireball.wav` | Kenney Digital Audio: laser4 (shortened) | `PlayerShooter`, `BossEnemy.Fire` (lower pitch) |
| Arrow | `SFX/arrow.wav` | Kenney Interface Sounds: pluck_002 | `ArcherEnemy` (Shoot) |
| Hit (player) | `SFX/hit_player.wav`, `hit_player_2.wav`, `hit_player_3.wav` | Kenney Impact Sounds: impactPunch_heavy_000, 002, 003 | `PlayerHealth.TakeDamage` |
| Hit (enemy) | `SFX/hit_enemy_1.wav`, `hit_enemy_2.wav`, `hit_enemy_3.wav` | Kenney Impact Sounds: impactPunch_medium_000, 001, 003 | `EnemyHealth.TakeDamage` |
| Pickup | `SFX/pickup.wav` | Kenney Digital Audio: powerUp7 | `HeartPickup` (heart dropped by enemies) |
| Goblin wind-up | `SFX/windup_goblin.wav` | Kenney Digital Audio: lowRandom | `GoblinEnemy` (WindUp) |
| Flying Eye wind-up | `SFX/windup_eye.wav` | Kenney Digital Audio: phaserUp3 | `FlyingEyeEnemy` (WindUp) |
| Archer wind-up | `SFX/windup_archer.wav` | Kenney Interface Sounds: scratch_004 | `ArcherEnemy` (Aim) |
| Boss wind-up | `SFX/windup_boss.wav` | Kenney Digital Audio: lowDown | `BossEnemy` (WindUp) |
| Death (player) | `SFX/death_player.wav` | Kenney Digital Audio: lowThreeTone | `PlayerHealth.Die` |
| Death (enemy) | `SFX/death_enemy.wav`, `death_enemy_2.wav` | Kenney Digital Audio: phaserDown3, phaserDown1 | `EnemyHealth` |
| UI click | `SFX/ui_click.wav` | Kenney Interface Sounds: select_001 | every Button, added by `AudioManager` |
| Win | `SFX/win.wav` | Kenney Music Jingles: jingles_NES12 | `GameSession.LevelComplete` |
| Lose | `SFX/lose.wav` | Kenney Music Jingles: jingles_NES11 | `GameSession.PlayerDied` |
| Menu music | `Music/menu_music.wav` | "Gaming" by SoundSurfer (quiet middle part, looped) | `MusicManager` (1 s crossfade) |
| Level music | `Music/level_music.wav` | "Gaming" by SoundSurfer (whole song) | `MusicManager` (1 s crossfade) |

Mixer `Assets/Audio/MainMixer.mixer`: Master → Music → MusicTrack, Master → SFX. Settings sliders set `MusicVol` and `SfxVol` in dB (Log10 × 20) and are saved in `save.json`. The Paused snapshot lowers MusicTrack by 10 dB and adds a 800 Hz lowpass.

### Sources and licenses

- **Kenney** (Kenney Vleugels, https://kenney.nl), license **CC0 1.0** (public domain): Digital Audio (https://kenney.nl/assets/digital-audio), Impact Sounds (https://kenney.nl/assets/impact-sounds), Interface Sounds (https://kenney.nl/assets/interface-sounds), Music Jingles (https://kenney.nl/assets/music-jingles). Converted from .ogg to .wav and matched in loudness.
- **Music:** "Gaming" by SoundSurfer from Pixabay (track ID 331807, https://pixabay.com/users/soundsurfer/), **Pixabay Content License** (free for use in games, no attribution required). Converted to .wav; the menu version is the quiet middle part of the song made into a seamless loop.
