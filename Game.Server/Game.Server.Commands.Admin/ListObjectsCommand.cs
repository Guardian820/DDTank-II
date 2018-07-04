using Game.Base;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;
namespace Game.Server.Commands.Admin
{
	[Cmd("&list", ePrivLevel.Player, "List the objects info in game", new string[]
	{
		"   /list [Option1][Option2] ...",
		"eg:    /list -g :list all game objects",
		"       /list -c :list all client objects",
		"       /list -p :list all gameplaye objects",
		"       /list -r :list all room objects",
		"       /list -b :list all battle servers"
	})]
	public class ListObjectsCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string a;
				if ((a = args[1]) != null)
				{
					if (a == "-c")
					{
						Console.WriteLine("client list:");
						Console.WriteLine("-------------------------------");
						GameClient[] allClients = GameServer.Instance.GetAllClients();
						GameClient[] array = allClients;
						for (int i = 0; i < array.Length; i++)
						{
							GameClient gameClient = array[i];
							Console.WriteLine(gameClient.ToString());
						}
						Console.WriteLine("-------------------------------");
						Console.WriteLine(string.Format("total:{0}", allClients.Length));
						return true;
					}
					if (a == "-p")
					{
						Console.WriteLine("player list:");
						Console.WriteLine("-------------------------------");
						GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
						GamePlayer[] array2 = allPlayers;
						for (int j = 0; j < array2.Length; j++)
						{
							GamePlayer gamePlayer = array2[j];
							Console.WriteLine(gamePlayer.ToString());
						}
						Console.WriteLine("-------------------------------");
						Console.WriteLine(string.Format("total:{0}", allPlayers.Length));
						return true;
					}
					if (a == "-r")
					{
						Console.WriteLine("room list:");
						Console.WriteLine("-------------------------------");
						List<BaseRoom> allUsingRoom = RoomMgr.GetAllUsingRoom();
						foreach (BaseRoom current in allUsingRoom)
						{
							Console.WriteLine(current.ToString());
						}
						Console.WriteLine("-------------------------------");
						Console.WriteLine(string.Format("total:{0}", allUsingRoom.Count));
						return true;
					}
					if (a == "-g")
					{
						Console.WriteLine("game list:");
						Console.WriteLine("-------------------------------");
						List<BaseGame> allGame = GameMgr.GetAllGame();
						foreach (BaseGame current2 in allGame)
						{
							Console.WriteLine(current2.ToString());
						}
						Console.WriteLine("-------------------------------");
						Console.WriteLine(string.Format("total:{0}", allGame.Count));
						return true;
					}
					if (a == "-b")
					{
						Console.WriteLine("battle list:");
						Console.WriteLine("-------------------------------");
						List<BattleServer> allBattles = BattleMgr.GetAllBattles();
						foreach (BattleServer current3 in allBattles)
						{
							Console.WriteLine(current3.ToString());
						}
						Console.WriteLine("-------------------------------");
						Console.WriteLine(string.Format("total:{0}", allBattles.Count));
						return true;
					}
				}
				this.DisplaySyntax(client);
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
