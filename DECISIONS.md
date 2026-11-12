# DECISIONS.md — Magic Wizard

What I asked the AI agent, what it got wrong, what I changed and why.

## Week 10 — Three enemy types

2026-11-05  Asked Claude Code for three enemies with state machines. The first version was one big change with no commits, so I threw it away and asked for it step by step: player, fireball, then one enemy per commit.
2026-11-05  I could not make the on-screen joystick work on my iPhone through Unity Remote by myself, so I asked the agent for help with the scripts. It first used the Input System OnScreenStick, which did not react; it then wrote the MobileJoystick and MobileButton scripts and we set Active Input Handling to Both.
2026-11-11  Fair attacks: enemies and the player pass through each other, damage comes only from an attack with a wind-up (slime 0.5 s, bat 0.6 s, archer 0.6 s) and only when the enemy is on screen.
2026-11-11  The archer stood in the middle of its ledge and the ledge blocked its own line of sight, so it never saw the player. Moved it to the edge of the ledge and raised view distance 8 → 9.
2026-11-12  Replaced the placeholder shapes with sprites from Wizard Pack and Monsters Creatures Fantasy (LuizMelo). There was no slime or bat in the same style, so the patroller is now a Goblin and the diver is a Flying Eye; the state machines did not change. Added lava in the pit that kills the player.
2026-11-12  Playtest on a phone: Maksat beat the level in 4 tries, Daulet in 2 tries. Both are inside the 2–4 tries target.
