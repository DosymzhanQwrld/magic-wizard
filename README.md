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
