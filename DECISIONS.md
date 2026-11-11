# DECISIONS.md — Magic Wizard

What I asked the AI agent, what it got wrong, what I changed and why.

## Week 10 — Three enemy types

2026-11-05  Asked Claude Code for three enemies with state machines. The first version was one big change with no commits, so I threw it away and asked for it step by step: player, fireball, then one enemy per commit.
2026-11-05  I could not make the on-screen joystick work on my iPhone through Unity Remote by myself, so I asked the agent for help with the scripts. It first used the Input System OnScreenStick, which did not react; it then wrote the MobileJoystick and MobileButton scripts and we set Active Input Handling to Both.
2026-11-11  Fair attacks: enemies and the player pass through each other, damage comes only from an attack with a wind-up (slime 0.5 s, bat 0.6 s, archer 0.6 s) and only when the enemy is on screen.
