namespace pstudio.GoM.Game.Models.World.Generators
{
    public interface IGenerator<T> where T : class, new()
    {
        IGrid<T> Generate(int width, int height, object param);
    }

}
