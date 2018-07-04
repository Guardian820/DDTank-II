using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(114, "付费翻牌")]
	public class PaymentTakeCardCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (!player.HasPaymentTakeCard)
			{
				if (player.PlayerDetail.RemoveMoney(500) > 0)
				{
					int num = (int)packet.ReadByte();
					player.CanTakeOut++;
					player.FinishTakeCard = false;
					player.HasPaymentTakeCard = true;
					player.PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_PaymentTakeCard, player.PlayerDetail.PlayerCharacter.ID, 100, player.PlayerDetail.PlayerCharacter.Money);
					if (num < 0 || num > game.Cards.Length)
					{
						game.TakeCard(player);
						return;
					}
					game.TakeCard(player, num);
					return;
				}
				else
				{
					player.PlayerDetail.SendInsufficientMoney(1);
				}
			}
		}
	}
}
