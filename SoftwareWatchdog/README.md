Software Watchdog
===============
Usage: 
````softwarewatchdog.exe <json arg string>````

Description:
The Software Watchdog will launch the game, minimize all other windows and then wait for the game to close before re-launching the front end.

Json Arguments
===============
launcherPath - Full filepath of launcher to open when the game closes. Will be launched right before the Watchdog closes.
gamePath - Full filepath of game's executable to open when the Watchdog starts.
launchParams - (Optional) Parameters to pass to the executable if required.

Note: Ensure you use a valid Json string. This can be validated by [Online Json Parsers](http://json.parser.online.fr/). 


Example Json String
===============

````
{
	"launcherPath" : "C:/My Launcher.exe",
	"gameData" :
	{
		"gamePath" : "C:/Game1/My Game.exe",
		"launchParams" : "-nosound -noborder"
	}
}
````
