Games3Project2
==============

XNA Game - Networked, 3D, First Person Shooter 

Contributors (Alphabetical Order):
JT Broad
Andrew Claudy
Clayton Sandham
Cole Stoltzfus

Requirements we must meet:
=============
```
- [X] The game must be a shooter, racing, or platformer.
- [X] The game shall only use primitive 3D geometry.
- [X] There must be three levels.
- [X] Game shall support Windows and Xbox 360 platforms.
- [X] Game must have cheat codes labelled in the code.(e.g. infinite lives, infinite bullets)
- [ ] The two levels shall be thematically connected so game flow is preserved.
- [X] Game must have a defined color pallet and theme.
- [X] Game shall be immersive world.
- [X] Game shall have 3D Graphics.
- [X] Background music shall be appropriate to the game.
- [X] Sound Fx shall be appropriate to the game.
- [X] Game shall use Xbox 360 controller.
- [X] Game shall gracefully handle controllers being disconnected.
- [X] Game use of keyboard is optional, however experience shows it can be useful for debugging.
```
```
- [X] Game must have good performance for multiplayer local play
- [X] Game must have good performance for multiplayer system-link network play.
```
```
- [ ] Game shall be compelling and exhibit flow.
- [X] Game shall be fun and enjoyable.
- [X] Game shall be well-tuned.
- [X] Game shall generate a heat map.
- [ ] A class of NPC must have a learning component
```
```
- [X] All members shall participate in coding, sound FX, and graphics development.
- [ ] All members shall submit a peer evaluation.
- [ ] The group must create a video recording to upload to tumblr
```
```
Team shall submit a description of:
- [ ] The network messaging system with calculations of bandwidth use.
- [ ] Description of network smoothing implementation.
- [ ] Description of AI NPC, which high-level algorithms of the learning methods.
```
^^ This report shall be part of the grade ^^ 

Requirements we have created for our game:
=============
```
- [X] The camera shall be a first person camera.
- [X] The player controls shall be similar to Halo conventions. 
- [X] The player can jetpack for 4-5 seconds before the player runs out of jetpack energy. 
- [X] The player cannot jump.
- [X] The jetpack energy shall recharge as time the jetpack is not used elapses.
- [X] The player cannot fall through platforms. The player can walk on platforms.
- [X] The game shall support up to four players locally.
- [ ] The game shall support up to 4 total players.
- [ ] The player who kills the juggernaut shall become the new juggernaut.
- [ ] A dead player shall respawn after a designated amount of seconds (3?)
- [ ] The only way to score is by killing the juggernaut or killing enemies while being the juggernaut. 
- [X] Decrease in health will occur when player is hit by gunfire.
- [ ] Fire between non-juggernaut players shall be disabled.
```
Menu
```
- [X] The main menu will consist of an option to create a lobby, to join an existing lobby, a control for adjusting the number of local players, an option to view the help menu.
- [ ] On the main menu, a message saying to press back to quit shall be visible. 
- [ ] Pressing back during a game shall end the current game session and go back to the main menu. 
- [ ] Pressing back during the help screen view shall return to the main menu screen.
```
Flow
```
- [X] Each round will end when an arbitrary hard-coded score limit is reached.
- [ ] A new round will load the other map and spawn the players.
- [ ] Scores shall be reset to zero for all players at the start of a new round.
```
