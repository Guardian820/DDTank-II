using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(14)]
	public class AddAttackSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			if ((player.CurrentBall.ID == 3 || player.CurrentBall.ID == 5 || player.CurrentBall.ID == 1) && (item.TemplateID == 10001 || item.TemplateID == 10002))
			{
				player.ShootCount = 1;
				return;
			}
			player.ShootCount += item.Property2;
			if (item.Property2 == 2)
			{
				player.CurrentShootMinus *= 0.6f;
				return;
			}
			player.CurrentShootMinus *= 0.9f;
		}
	}
}
