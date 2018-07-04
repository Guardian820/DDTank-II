using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Buffer
{
	public class OfferMultipleBuffer : AbstractBuffer
	{
		public OfferMultipleBuffer(BufferInfo info) : base(info)
		{
		}
		public override void Start(GamePlayer player)
		{
			OfferMultipleBuffer offerMultipleBuffer = player.BufferList.GetOfType(typeof(OfferMultipleBuffer)) as OfferMultipleBuffer;
			if (offerMultipleBuffer != null)
			{
				offerMultipleBuffer.Info.ValidDate += base.Info.ValidDate;
				player.BufferList.UpdateBuffer(offerMultipleBuffer);
				return;
			}
			base.Start(player);
			player.OfferAddPlus *= (double)base.Info.Value;
		}
		public override void Stop()
		{
			this.m_player.OfferAddPlus /= (double)this.m_info.Value;
			base.Stop();
		}
	}
}
