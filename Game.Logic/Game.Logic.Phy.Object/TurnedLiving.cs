using System;
namespace Game.Logic.Phy.Object
{
	public class TurnedLiving : Living
	{
		protected int m_delay;
		public int DefaultDelay;
		private int m_dander;
		private int m_psychic = 20;
		private int m_petMaxMP = 100;
		private int m_petMP = 10;
		public int Delay
		{
			get
			{
				return this.m_delay;
			}
			set
			{
				this.m_delay = value;
			}
		}
		public int psychic
		{
			get
			{
				return this.m_psychic;
			}
			set
			{
				this.m_psychic = value;
			}
		}
		public int PetMaxMP
		{
			get
			{
				return this.m_petMaxMP;
			}
			set
			{
				this.m_petMaxMP = value;
			}
		}
		public int PetMP
		{
			get
			{
				return this.m_petMP;
			}
			set
			{
				this.m_petMP = value;
			}
		}
		public int Dander
		{
			get
			{
				return this.m_dander;
			}
			set
			{
				this.m_dander = value;
			}
		}
		public TurnedLiving(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction) : base(id, game, team, name, modelId, maxBlood, immunity, direction)
		{
		}
		public override void Reset()
		{
			base.Reset();
            if (this is Player)
            {
                m_delay = 1600 - 1200 * ((Player)this).PlayerDetail.PlayerCharacter.Agility / (((Player)this).PlayerDetail.PlayerCharacter.Agility + 1200) + ((Player)this).PlayerDetail.PlayerCharacter.Attack / 10;//(int)(Agility);
            }
            else
            {
                m_delay = (int)(Agility);
            }
		}
		public void AddDelay(int value)
		{
			this.m_delay += value;
		}
		public override void PrepareSelfTurn()
		{
			this.DefaultDelay = this.m_delay;
            if (IsFrost)
            {
                if (this is Player)
                {
                    AddDelay(1600 - 1200 * ((Player)this).PlayerDetail.PlayerCharacter.Agility / (((Player)this).PlayerDetail.PlayerCharacter.Agility + 1200) + ((Player)this).PlayerDetail.PlayerCharacter.Attack / 10);
                }
                else
                {
                    AddDelay((this as SimpleBoss).NpcInfo.Delay);
                }
            }
            if (IsFrost && this is NormalBoss)
            {
                AddDelay((this as NormalBoss).NpcInfo.Delay);
            }
			base.PrepareSelfTurn();
		}
		public void AddPetMP()
		{
			if (base.IsLiving && this.PetMP < this.PetMaxMP)
			{
				this.m_petMP += 10;
				return;
			}
			this.m_petMP = this.PetMaxMP;
		}
		public void AddDander(int value)
		{
			if (value > 0 && base.IsLiving)
			{
				this.SetDander(this.m_dander + value);
			}
		}
		public void SetDander(int value)
		{
			this.m_dander = Math.Min(value, 200);
			if (base.SyncAtTime)
			{
				this.m_game.SendGameUpdateDander(this);
			}
		}
		public virtual void StartGame()
		{
		}
		public virtual void Skip(int spendTime)
		{
			if (base.IsAttacking)
			{
				this.StopAttacking();
				this.m_game.CheckState(0);
			}
		}
	}
}
