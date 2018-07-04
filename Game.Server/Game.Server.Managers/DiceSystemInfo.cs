using System;
using System.Collections.Generic;
namespace Game.Server.Managers
{
	public class DiceSystemInfo
	{
		private int m_UserID;
		private bool userFirstCell;
		private int m_LuckIntegral;
		private int m_LuckIntegralLevel = -1;
		private int m_currentPosition = -1;
		private List<DiceSystemItem> m_ItemDice;
		public int UserID
		{
			get
			{
				return this.m_UserID;
			}
			set
			{
				this.m_UserID = value;
			}
		}
		public bool UserFirstCell
		{
			get
			{
				return this.userFirstCell;
			}
			set
			{
				this.userFirstCell = value;
			}
		}
		public int CurrentPosition
		{
			get
			{
				return this.m_currentPosition;
			}
			set
			{
				this.m_currentPosition = value;
			}
		}
		public int LuckIntegralLevel
		{
			get
			{
				return this.m_LuckIntegralLevel;
			}
			set
			{
				this.m_LuckIntegralLevel = value;
			}
		}
		public int LuckIntegral
		{
			get
			{
				return this.m_LuckIntegral;
			}
			set
			{
				this.m_LuckIntegral = value;
			}
		}
		public List<DiceSystemItem> ItemDice
		{
			get
			{
				if (this.m_ItemDice == null)
				{
					this.m_ItemDice = new List<DiceSystemItem>();
				}
				return this.m_ItemDice;
			}
			set
			{
				this.m_ItemDice = value;
			}
		}
	}
}
