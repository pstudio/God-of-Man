using System.Collections.Generic;

namespace pstudio.GoM.Game.Models.World
{
    public class World
    {
        public IGrid<Land> Land { get; set; }
        public IGrid<Population> Populations { get; set; } 
        public List<Settler> Settlers { get; set; } 
        public int Width { get { return Land.Width; } }
        public int Height { get { return Land.Height; } }
    }

}
