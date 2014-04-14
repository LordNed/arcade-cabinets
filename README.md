Arcade Cabinet Software
===============

Software created for the local University Game Development club to run student made games on custom built arcade cabinets. This works similar to the [Winnitron](http://winnitron.ca/) project except open source and allows you to deploy your own games. 

Overview
===============
There's several parts to this, most of which are optional. The first part is the "Front End". This is the visual component to the arcade cabinet software and lets the user browse games loaded onto the cabinet and launch them. This is generally referred to as the "Launcher".

The second component is the "Software Watchdog". This is the application that takes the request from the Front End to launch a game and then tracks the progress of the game (ie: waits for the game to quit). Once the game quits it then re-opens the launcher so the user can choose a new game.

The third component is the web-server backend. This is a NodeJS server running a web-server backend as well as some misc. things about whether the machine is in coin-op mode or free to play mode (more on this later).

The fourth component is the "Hardware Watchdog". This is an Audrino located inside our machines that has two primary tasks: It ensures the computer is running when there is power, and it translates the serial communication from the coin-op machine into key-presses on our XArcade control board (which are then fed into the computers).

All four of these components are ideally designed to work without each other but that is untested as they were all written together!

Front End / Launcher
===============
//ToDo

Software Watchdog
===============
//ToDo

NodeJS Webserver
===============
//ToDo

Hardware Watchdog
===============
//ToDo
