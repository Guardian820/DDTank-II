using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Logic.Spells.NormalSpell
{
	[SpellAttibute(1)]
	public class AddLifeSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			switch (item.Property2)
			{
			case 0:
				if (player.IsLiving)
				{
					player.AddBlood(item.Property3);
					return;
				}
				break;

			case 1:
				{
					List<Player> allFightPlayers = player.Game.GetAllFightPlayers();
					foreach (Player current in allFightPlayers)
					{
						if (current.IsLiving && current.Team == player.Team)
						{
							current.AddBlood(item.Property3);
						}
					}
					break;
				}

			default:
				return;
			}
		}
	}
}
