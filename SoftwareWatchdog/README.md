Software Watchdog
===============
Usage: 
````softwarewatchdog.exe <escaped json string>````

Description:
The Software Watchdog will launch the game, minimize all other windows and then wait for the game to close before re-launching the front end. The software takes an argument string in the form of an escaped json configuration file. This was chosen to ensure a flexible and easily extendable set of command arguments if needed. 

Json Arguments
===============
launcherPath - Full filepath of launcher to open when the game closes. Will be launched right before the Watchdog closes.
gamePath - Full filepath of game's executable to open when the Watchdog starts.
launchParams - (Optional) Parameters to pass to the executable if required.

Note: Ensure you use a valid Json string. This can be validated by [Online Json Parsers](http://json.parser.online.fr/). 


Example Json String
===============
````
{\"launcherPath\" : \"C:/My Launcher.exe\",	\"gameData\" : {\"gamePath\" : \"C:/Game1/My Game.exe\", \"launchParams\" : \"-nosound -noborder\"}}
````
Expanded this Json string looks as follows:

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
Please note that the Json string passed to the program must be properly escaped. This means that file paths should use "/" instead of "\", and any time there is a quote it must be prefixed with a "\" char. Most Json libraries will automatically escape the Json output anyways so this should not present a significant issue unless you are crafting the json argument by hand.