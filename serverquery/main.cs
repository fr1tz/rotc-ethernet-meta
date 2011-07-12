//------------------------------------------------------------------------------
// Revenge Of The Cats: Ethernet Server Query Tools
// Copyright (C) 2011, mEthLab Interactive
//------------------------------------------------------------------------------

$Pref::backgroundSleepTime = 0;

// List of master servers to query, each one 
// is tried in order until one responds...
$Pref::Master[0] = "2:master.garagegames.com:28002";
$Pref::Master[1] = "2:spica.dyndns.info:28002";

$GameNameString = "rotc-ethernet";
$GameVersionString = "serverwatcher";

// Game information used to query the master server
$Client::GameTypeQuery = $GameNameString;
$Client::MissionTypeQuery = "Any";

function printServers()
{
	%sc = getServerCount();
	for (%i = 0; %i < %sc; %i++) {
		setServerInfo(%i);
        %prefix = "\n" @ $ServerInfo::Address;
		echo(%prefix SPC "arena" SPC $ServerInfo::MissionType SPC
			$ServerInfo::MissionName);
		echo(%prefix SPC "ping" SPC $ServerInfo::Ping);
		echo(%prefix SPC "player_count" SPC $ServerInfo::PlayerCount @ " of " @ $ServerInfo::MaxPlayers);
		echo(%prefix SPC "hoster" SPC $ServerInfo::Name);
		echo(%prefix SPC "description" SPC strreplace($ServerInfo::Info, "\n", "<br>"));
	}
}

function queryServers()
{
	if($lan)
	{
		queryLANServers(
			28000,		// lanPort for local queries
			0,			 // Query flags
			$Client::GameTypeQuery,		 // gameTypes
			$Client::MissionTypeQuery,	 // missionType
			0,			 // minPlayers
			100,		  // maxPlayers
			0,			 // maxBots
			2,			 // regionMask
			0,			 // maxPing
			100,		  // minCPU
			0			  // filterFlags
		);
	}
	else
	{
		queryMasterServer(
			0,			 // Query flags
			$Client::GameTypeQuery,		 // gameTypes
			$Client::MissionTypeQuery,	 // missionType
			0,			 // minPlayers
			100,		  // maxPlayers
			0,			 // maxBots
			2,			 // regionMask
			0,			 // maxPing
			100,		  // minCPU
			0			  // filterFlags
		);
	}
}

function onServerQueryStatus(%status, %msg, %value)
{
    if(%status $= "done")
    {
        printServers();
        quit();
    }
}

function onExit()
{
	quit();
}


$lan = false;

for (%i = 1; %i < $Game::argc ; %i++)
{
	%arg = $Game::argv[%i];
	%nextArg = $Game::argv[%i+1];
	%hasNextArg = $Game::argc - %i > 1;
	
	switch$ (%arg)
	{
		case "-lan":
			$lan = true;
	}
}

setLogLevel(0); // no log file
enableWinConsole(true);
setNetPort(0);
schedule(0, 0, queryServers);
