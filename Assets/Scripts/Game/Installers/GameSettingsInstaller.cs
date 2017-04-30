using pstudio.GoM.Game.Models.World.Generators;
using UnityEngine;
using Zenject;

namespace pstudio.GoM.Game.Installers
{
    [CreateAssetMenu(menuName = "God of Man/Game Settings")]
    public class GameSettingsInstaller : ScriptableObjectInstaller
    {
        public WorldGenerator.Settings World;
        public LandGenerator.Settings Land;
        public PopulationGenerator.Settings Population;

        public override void InstallBindings()
        {
            Container.BindInstance(World);
            Container.BindInstance(Land);
            Container.BindInstance(Population);
        }
    }

}
