using Game.Logic.Actions;
using System;
namespace Game.Logic.Phy.Object
{
	public class PhysicalObj : Physics
	{
		private string m_model;
		private string m_currentAction;
		private int m_scale;
		private int m_rotation;
		private BaseGame m_game;
		private bool m_canPenetrate;
		private string m_name;
		public virtual int Type
		{
			get
			{
				return 0;
			}
		}
		public string Model
		{
			get
			{
				return this.m_model;
			}
		}
		public string CurrentAction
		{
			get
			{
				return this.m_currentAction;
			}
			set
			{
				this.m_currentAction = value;
			}
		}
		public int Scale
		{
			get
			{
				return this.m_scale;
			}
		}
		public int Rotation
		{
			get
			{
				return this.m_rotation;
			}
		}
		public bool CanPenetrate
		{
			get
			{
				return this.m_canPenetrate;
			}
			set
			{
				this.m_canPenetrate = value;
			}
		}
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}
		public PhysicalObj(int id, string name, string model, string defaultAction, int scale, int rotation) : base(id)
		{
			this.m_name = name;
			this.m_model = model;
			this.m_currentAction = defaultAction;
			this.m_scale = scale;
			this.m_rotation = rotation;
			this.m_canPenetrate = false;
		}
		public void SetGame(BaseGame game)
		{
			this.m_game = game;
		}
		public void PlayMovie(string action, int delay, int movieTime)
		{
			if (this.m_game != null)
			{
				this.m_game.AddAction(new PhysicalObjDoAction(this, action, delay, movieTime));
			}
		}
		public override void CollidedByObject(Physics phy)
		{
			if (!this.m_canPenetrate && phy is SimpleBomb)
			{
				((SimpleBomb)phy).Bomb();
			}
		}
	}
}
