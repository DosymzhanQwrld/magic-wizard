# DECISIONS.md — Magic Wizard

What I asked the AI agent, what it got wrong, what I changed and why.

## Week 10 — Three enemy types

2026-11-05  Asked Claude Code for three enemies with state machines. The first version was one big change with no commits, so I threw it away and asked for it step by step: player, fireball, then one enemy per commit.
2026-11-05  I could not make the on-screen joystick work on my iPhone through Unity Remote by myself, so I asked the agent for help with the scripts. It first used the Input System OnScreenStick, which did not react; it then wrote the MobileJoystick and MobileButton scripts and we set Active Input Handling to Both.
2026-11-11  Fair attacks: enemies and the player pass through each other, damage comes only from an attack with a wind-up (slime 0.5 s, bat 0.6 s, archer 0.6 s) and only when the enemy is on screen.
2026-11-11  The archer stood in the middle of its ledge and the ledge blocked its own line of sight, so it never saw the player. Moved it to the edge of the ledge and raised view distance 8 → 9.
2026-11-12  Replaced the placeholder shapes with sprites from Wizard Pack and Monsters Creatures Fantasy (LuizMelo). There was no slime or bat in the same style, so the patroller is now a Goblin and the diver is a Flying Eye; the state machines did not change. Added lava in the pit that kills the player.
2026-11-12  Playtest on a phone: Maksat beat the level in 4 tries, Daulet in 2 tries. Both are inside the 2–4 tries target.

## Week 11 — Data and saves

2026-11-17  Asked the agent to move every balance number into ScriptableObjects: PlayerStats, GoblinStats, FlyingEyeStats, ArcherStats and BossStats in Assets/Data/Stats. Scripts now only read stats.xxx; changing a number in the asset changes the game with no code edits.
2026-11-18  Levels are LevelData assets (blocks, lava, enemies, boss, time, ground and background). The first version loaded all levels into one scene, so the Scene view looked empty; I asked for separate Level1-3 scenes that use the same LevelLoader code and show an editor preview.
2026-11-18  Level 2 got harder (more enemies, two lava pits) and Level 3 ends with a Flying Demon boss: HP bar at the top, below 50% HP it shoots 3 fireballs and everything is 2x faster.
2026-11-19  Save uses JsonUtility to save.json in persistentDataPath with try/catch: a missing or broken file falls back to defaults. It saves after a level, when a setting changes and in OnApplicationPause(true). An editor self-check passed 9 of 9 cases (missing file, broken file, bad values, unlock, best score, reset).
2026-11-19  The agent's menu builder made the red RESET buttons wrap their text off the button; fixed with auto-sized text. Volume sliders are saved but not applied yet, because the game has no audio until week 12.
2026-11-19  How we worked: the agent wrote most of the code; I decided what to build (boss phases, harder Level 2, separate level scenes), chose and downloaded all art, tested every step in Unity and on the phone, reported bugs, and made the commits.

## Week 12 — Sound pass

2026-11-25  My first mixer had MusicTrack directly under Master and no SFX group. Final layout: Master → Music → MusicTrack, Master → SFX. The Music slider is an exposed parameter on Music and the Paused snapshot changes MusicTrack, because a value set from code overrides snapshots on the same group.
2026-11-25  One AudioManager is created before the first scene and kept with DontDestroyOnLoad. Music only changes when a scene needs another track, so dying and reloading a level does not restart it. The 1 s crossfade uses unscaled time, so it also works on the pause and victory screens (timeScale 0).
2026-11-25  Sliders stay 0–1 in save.json and are converted to dB with Log10 × 20 (0 → -80 dB). AudioMixer.SetFloat does not work in Awake, so volumes are applied in Start and on every scene load.
2026-11-25  The game had nothing to pick up, so killed enemies drop a heart (50%) that heals 1 HP; that is the pickup sound. Wind-up sounds start on the first frame of each wind-up (goblin, eye, archer aim, boss), so attacks are heard before they land. Repeated sounds get pitch 0.92–1.08.
2026-11-25  Sounds: all effects come from four Kenney packs (CC0) and the music is "Gaming" by SoundSurfer from Pixabay. Where no clip matched exactly I took the closest one: lowRandom for the goblin growl, lowDown for the boss, scratch for the bow draw, pluck for the arrow and a shortened laser for the fireball. The menu plays the quiet middle part of the song as a loop, the level plays the whole song, so the crossfade is still audible. All clips were matched in loudness; jump, player hit and enemy hit have 3 clips each.
