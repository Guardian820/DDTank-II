using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(32, "使用道具")]
	public class PropUseCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState != eGameState.Playing || player.GetSealState())
			{
				return;
			}
			int bag = (int)packet.ReadByte();
			int place = packet.ReadInt();
			int num = packet.ReadInt();
			ItemTemplateInfo item = ItemMgr.FindItemTemplate(num);
			if (player.CanUseItem(item))
			{
				if (player.PlayerDetail.UsePropItem(game, bag, place, num, player.IsLiving))
				{
					if (!player.UseItem(item))
					{
						BaseGame.log.Error("Using prop error");
						return;
					}
				}
				else
				{
					player.UseItem(item);
					int num2 = num;
					if (num2 != 10001)
					{
						if (num2 != 10004)
						{
							return;
						}
						if (player.Prop < num * 2)
						{
							player.Prop += num;
						}
					}
					else
					{
						if (player.Prop < num * 2)
						{
							player.Prop += num;
							return;
						}
					}
				}
			}
		}
	}
}
