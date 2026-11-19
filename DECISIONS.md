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
