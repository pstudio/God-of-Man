namespace pstudio.GoM.Game.Models.World
{
    public class Settler : Cell
    {
        public bool ShouldDie { get; set; }
        public int LastDirectionX { get; set; }
        public int LastDirectionY { get; set; }
        public int Life { get; set; }
    }

}
