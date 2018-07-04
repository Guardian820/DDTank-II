using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(15)]
	public class AddBallSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			if (player.IsSpecialSkill)
			{
				return;
			}
			if ((player.CurrentBall.ID == 3 || player.CurrentBall.ID == 5 || player.CurrentBall.ID == 1) && item.TemplateID == 10003)
			{
				player.BallCount = 1;
				return;
			}
			player.CurrentDamagePlus *= 0.5f;
			player.BallCount = item.Property2;
		}
	}
}
