using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic
{
	public class MapPoint
	{
		private List<Point> posX;
		private List<Point> posX1;
		public List<Point> PosX
		{
			get
			{
				return this.posX;
			}
			set
			{
				this.posX = value;
			}
		}
		public List<Point> PosX1
		{
			get
			{
				return this.posX1;
			}
			set
			{
				this.posX1 = value;
			}
		}
		public MapPoint()
		{
			this.posX = new List<Point>();
			this.posX1 = new List<Point>();
		}
	}
}
