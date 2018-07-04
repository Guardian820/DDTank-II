using System;
namespace Game.Logic.Phy.Object
{
    public class LayerLabyrinth : PhysicalObj
    {
        public override int Type
        {
            get
            {
                return 3;
            }
        }
        public LayerLabyrinth(int id, string name, string model, string defaultAction, int scale, int rotation)
            : base(id, name, model, defaultAction, scale, rotation)
        {
        }
    }
}
