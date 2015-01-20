// Copyright information can be found in the file named COPYING
// located in the root directory of this distribution.

function GameCoreCTF::prepareMissionLoad(%game)
{
   //echo(%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::prepareMissionLoad");
   Parent::prepareMissionLoad(%game);
   ETH::createTeams(%game);
}

function GameCoreCTF::onMissionLoaded(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::onMissionLoaded");
   Parent::onMissionLoaded(%game);
}

function GameCoreCTF::initGameVars(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::initGameVars");

   //-----------------------------------------------------------------------------
   // What kind of "player" is spawned is either controlled directly by the
   // SpawnSphere or it defaults back to the values set here. This also controls
   // which SimGroups to attempt to select the spawn sphere's from by walking down
   // the list of SpawnGroups till it finds a valid spawn object.
   // These override the values set in core/scripts/server/spawn.cs
   //-----------------------------------------------------------------------------
   
   // Leave $Game::defaultPlayerClass and $Game::defaultPlayerDataBlock as empty strings ("")
   // to spawn a the $Game::defaultCameraClass as the control object.
   $Game::defaultPlayerClass = "Etherform";
   $Game::defaultPlayerDataBlock = "FrmEtherform";
   $Game::defaultPlayerSpawnGroups = "PlayerSpawnPoints PlayerDropPoints";

   //-----------------------------------------------------------------------------
   // What kind of "camera" is spawned is either controlled directly by the
   // SpawnSphere or it defaults back to the values set here. This also controls
   // which SimGroups to attempt to select the spawn sphere's from by walking down
   // the list of SpawnGroups till it finds a valid spawn object.
   // These override the values set in core/scripts/server/spawn.cs
   //-----------------------------------------------------------------------------
   $Game::defaultCameraClass = "Camera";
   $Game::defaultCameraDataBlock = "Observer";
   $Game::defaultCameraSpawnGroups = "CameraSpawnPoints PlayerSpawnPoints PlayerDropPoints";

   // Set the gameplay parameters
   %game.duration = 0;
   %game.endgameScore = 0;
   %game.endgamePause = 10;
   %game.allowCycling = true;   // Is mission cycling allowed?
}

function GameCoreCTF::startGame(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::startGame");

   Parent::startGame(%game);
   CTF::restartGame();
}

function GameCoreCTF::endGame(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::endGame");

   parent::endGame(%game);
}

function GameCoreCTF::onGameDurationEnd(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::onGameDurationEnd");

   parent::onGameDurationEnd(%game);
}

function GameCoreCTF::prepareClient(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::prepareClient");

   Parent::prepareClient(%game, %client);
}

function GameCoreCTF::onClientEnterGame(%game, %client)
{
   echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::onClientEnterGame");
   
   Parent::onClientEnterGame(%game, %client);
   
   // Setup loadouts
   %client.zActiveLoadout = 0;
   CTF::resetLoadout(%client);

   CTF::setupHud(%client);

   %team1playerCount = ETH::getTeamPlayerCount(1);
   %team2playerCount = ETH::getTeamPlayerCount(2);
   
   // Join team with less players.
   if(%team1playerCount > %team2playerCount)
      ETH::joinTeam(%client, 2);
   else
      ETH::joinTeam(%client, 1);

   if($Game::Duration)
   {
      %timeLeft = ($Game::StartTime + $Game::Duration) - $Sim::Time;
      commandToClient(%client, 'GameTimer', %timeLeft);
   }
}

function GameCoreCTF::onClientLeaveGame(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::onClientLeaveGame");

   parent::onClientLeaveGame(%game, %client);

}

function GameCoreCTF::queryClientSettings(%game, %client, %settings)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::queryClientSettings");

   Parent::queryClientSettings(%game, %client, %settings);
   
   %client.paletteColors[0] = "255 255 255 255";
   %client.paletteColors[1] = "255 255 255 255";
   
   commandToClient(%client, 'XaNotcSettings1_Query', "PlayerColor0");
   commandToClient(%client, 'XaNotcSettings1_Query', "PlayerColor1");
}

function GameCoreCTF::processClientSettingsReply(%game, %client, %setting, %value)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::processClientSettingsReply");
   
   %status = "Ignored";
   
   echo(%client.authenticated);

   if(%setting $= "PlayerColor0")
   {
      if(!%client.authenticated)
      {
         %status = "Ignored for unauthenticated players";
      }
      else if(isValidPlayerColor(%value))
      {
         %client.paletteColors[0] = %value SPC "255";
         %status = "Ok";
      }
      else
         %status = "Invalid";

   }
   else if(%setting $= "PlayerColor1")
   {
      if(!%client.authenticated)
      {
         %status = "Ignored for unauthenticated players";
      }
      else if(isValidPlayerColor(%value))
      {
         %client.paletteColors[1] = %value SPC "255";
         %status = "Ok";
      }
      else
         %status = "Invalid";
   }

   commandToClient(%client, 'XaNotcSettings1_Confirmation', %setting, %status);
}

function GameCoreCTF::clientRecordingDemo(%game, %client, %isRecording)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::clientRecordingDemo");
   
   if(!%isRecording)
      return;
      
   %client.zDemoRecordingSetupInProgress = true;
   ETH::setupHud(%client);
   %client.control(%client.player);
   %client.zDemoRecordingSetupInProgress = false;
}

function GameCoreCTF::updateClientPlayerList(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::updateClientPlayerList");
   Parent::updateClientPlayerList(%game, %client);
   ETH::updatePlayerList(%client);
}

function GameCoreCTF::loadOut(%game, %player)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::loadOut");
   
   Parent::loadOut(%game, %player);
   
   %team = %player.client.team;
   %player.setTeamId(%team.teamId);
   %teamColorF = %team.color;
   %teamColorI = mFloatLength(getWord(%teamColorF, 0)*255, 0) SPC
                 mFloatLength(getWord(%teamColorF, 1)*255, 0) SPC
                 mFloatLength(getWord(%teamColorF, 2)*255, 0) SPC
                 255;
   //echo(%teamColorF SPC "->" SPC %teamColorI);

   %player.paletteColors[0] = %teamColorI;
   %player.paletteColors[1] = %teamColorI;

   if(isObject(%player.light))
   {
      %colorI = %player.paletteColors[0];
      %colorF = getWord(%colorI, 0) / 255 SPC
                getWord(%colorI, 1) / 255 SPC
                getWord(%colorI, 2) / 255 SPC
                1;
      %player.light.color = %colorF;
   }

   // Setup ShapeBase HudInfo object team ID
   %player.zShapeBaseHudInfo.setDatasetType(0, $HudInfoDatasetType::TeamID);
   %player.zShapeBaseHudInfo.setDatasetIntField(0, %player.getTeamId());
   %player.zShapeBaseHudInfo.setDatasetType(1, $HudInfoDatasetType::Name);
   %player.zShapeBaseHudInfo.setDatasetStringField(1, %player.client.playerName);

   if(%player.getClassName() $= "Player" || %player.getClassName() $= "AiPlayer")
      CTF::loadoutPlayer(%player);
   else if(%player.getClassName() $= "Etherform")
      CTF::loadoutEtherform(%player);
}

function GameCoreCTF::onUnitDestroyed(%game, %obj)
{
   //echo(%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::onUnitDestroyed");
   
   Parent::onUnitDestroyed(%game, %obj);
   
   %client = %obj.client;
   if(isObject(%client) && %client.player == %obj)
   {
      ETH::switchToEtherform(%client);
   }
}

function GameCoreCTF::clientAction(%game, %client, %nr)
{
   echo(%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::clientAction");

   %obj = %client.getControlObject();
   if(!isObject(%obj))
      return;

   %obj.getDataBlock().clientAction(%obj, %nr);
}

function GameCoreCTF::etherformManifest(%game, %obj)
{
   echo(%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::etherformManifest");
   
   %client = %obj.client;
   
   if(!isObject(%client))
      return;
      
   if(%obj.getClassName() !$= "Etherform")
      return;
      
   %percent = %client.zLoadoutProgress[%client.zActiveLoadout];

   if(%percent < 0.5)
   {
      %client.BeepMsg("Class needs at least 50% health to manifest!");
      return;
   }

   if(%client.player.getEnergyLevel() < 50)
   {
      %client.BeepMsg("You need at least 50% energy to manifest!");
      return;
   }

   %ownTeamId = %client.player.getTeamId();

   %inOwnZone = false;
   %inOwnTerritory = false;
   %inEnemyZone = false;

   %pos = %obj.getPosition();
   InitContainerRadiusSearch(%pos, 0.0001, $TypeMasks::TacticalZoneObjectType);
   while((%srchObj = containerSearchNext()) != 0)
   {
      // object actually in this zone?
      %inSrchZone = false;
      for(%i = 0; %i < %srchObj.getNumObjects(); %i++)
      {
         if(%srchObj.getObject(%i) == %client.player)
         {
            %inSrchZone = true;
            break;
         }
      }
      if(!%inSrchZone)
         continue;

      %zoneTeamId = %srchObj.getTeamId();
      %zoneBlocked = %srchObj.zBlocked;

      if(%zoneTeamId != %ownTeamId && %zoneTeamId != 0)
      {
         %inEnemyZone = true;
         break;
      }
      else if(%zoneTeamId == %ownTeamId)
      {
         %inOwnZone = true;
         if(%srchObj.getDataBlock().getName() $= "TerritoryZone"
         || %srchObj.getDataBlock().isTerritoryZone)
            %inOwnTerritory = true;
      }
   }

   if(%inEnemyZone)
   {
      %client.BeepMsg("You can not manifest in an enemy zone!");
      return;
   }
   else if(%inOwnZone && !%inOwnTerritory)
   {
      %client.BeepMsg("This is not a territory zone!");
      return;
   }
   else if(!%inOwnZone)
   {
      %client.BeepMsg("You can only manifest in your team's territory zones!");
      return;
   }
   else if(%zoneBlocked)
   {
      %client.BeepMsg("This zone is currently blocked!");
      return;
   }
   
   %data = FrmStandardcat;
   switch(%client.zActiveLoadout)
   {
      case 2:
         %data = FrmTerritoryGenerator2DV;
   }

   %player = new Player() {
      dataBlock = %data;
      client = %client;
      teamId = %client.team.teamId;
      isCAT = true;
   };
   MissionCleanup.add(%player);
   copyPalette(%obj, %player);

   Game.loadOut(%player);

   %mat = %obj.getTransform();
   %dmg = %obj.getDamageLevel();
   %nrg = %obj.getEnergyLevel();
   %buf = %obj.getDamageBufferLevel();
   %vel = %obj.getVelocity();

   %player.setTransform(%mat);
   %player.setTransform(%pos);
   %player.setDamageLevel(%player.getDataBlock().maxDamage * (1-%percent));
   //%player.setShieldLevel(%buf);

	//if(%tagged || $Server::Game.tagMode == $Server::Game.alwaystag)
	//	%player.setTagged();

   %client.control(%player);

   // Remove any z-velocity.
   %vel = getWord(%vel, 0) SPC getWord(%vel, 1) SPC "0";

   %player.setEnergyLevel(%nrg);
   %player.setVelocity(VectorScale(%vel, 0.25));

   %client.player.schedule(9, "delete");
	%client.player = %player;
}

function GameCoreCTF::suicide(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::suicide");
   ETH::switchToEtherform(%client);
}

function GameCoreCTF::F(%game, %client, %nr)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCoreCTF::F");
   if(%nr >= 1 && %nr <= 2)
      ETH::joinTeam(%client, %nr);
}

  
