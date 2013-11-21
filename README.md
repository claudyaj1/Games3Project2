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
- [x] The game must be a shooter, racing, or platformer.
- [x] The game shall only use primative 3D geometry
- [X] There must be two levels
- [x] Game shall support Windows and Xbox 360 platforms.
- [ ] Game must have cheat codes labeled in the code.(e.g. infinte lives, infinite bullets)
- [ ] The two levels shall be thematically connected so game flow is preserved.
- [X] Game must have a defined color pallete and theme.
- [X] Game shall be immersive world.
- [x] Game shall have 3D Graphics.
- [X] Background music shall be appropriate to the game.
- [X] Sound Fx shall be appropriate to the game.
- [X] Game shall use Xbox 360 controller.
- [X] Game shall gracefully handle controllers being disconnected.
- [X] Game use of keyboard is optional, however experience shows it can be useful for debugging.
```
```
- [X] Game must have good performance for multiplayer local play
- [ ] Game must have good performance for multiplayer system-link network play.
```
```
- [ ] Game shall be compelling and exhibit flow.
- [X] Game shall be fun and enjoyable.
- [X] Game shall be well-tuned.
- [X] Game shall generate a heat map.
```
```
- [X] All members shall participate in coding, sound FX, and graphics development.
- [ ] All members shall submit a peer evaluation.
```
```
Team shall submit a description of the tradeoffs considered during the game design such as:
- [ ] How state was divided among machines? (networked machines, I beleive)
- [ ] How various algorithms where implemented and why?
- [ ] How was collision detection handled?
- [ ] Describe the information architecture of the game and bandwidth calculation.
- [ ] Does the game support in-game voice chat? Does voice chat contribute to networked gameplay?
```
^^ This report shall be 20% of the grade ^^ 

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
- [X] Each round will end when an abritrary hard-coded score limit is reached.
- [ ] A new round will load the other map and spawn the players.
- [ ] Scores shall be reset to zero for all players at the start of a new round.
```
