using pstudio.GoM.Game.Controllers;
using pstudio.GoM.Game.Models.World;
using pstudio.GoM.Game.Models.World.Generators;
using pstudio.GoM.Game.Views;
using pstudio.GoM.Game.Views.Heatmap;
using pstudio.GoM.Game.Views.UI;
using UnityEngine;
using Zenject;

namespace pstudio.GoM.Game.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private WorldView _worldView;
        [SerializeField] private HeatmapView _heatmapView;
        [SerializeField] private StatsUI _statsUI;
        [SerializeField] private Camera _camera;

        public override void InstallBindings()
        {
            Container.BindInstance(_worldView);
            Container.BindInstance(_heatmapView);
            Container.BindInstance(_statsUI);
            Container.BindInstance(_camera);

            Container.BindInterfacesAndSelfTo<ActionController>().AsSingle();

            InstallWorld();

            Container.BindInterfacesAndSelfTo<TimeController>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIController>().AsSingle();

            Container.Inject(_worldView);
        }

        private void InstallWorld()
        {
            Container.Bind<IGenerator<Land>>().To<LandGenerator>().AsSingle();
            Container.Bind<IGenerator<Population>>().To<PopulationGenerator>().AsSingle();
            Container.Bind<WorldGenerator>().AsSingle();
            Container.BindFactory<TileView, TileView.Factory>()
                .FromComponentInNewPrefab(_worldView.TilePrefab)
                .WithGameObjectName("Tile")
                .UnderTransform(_worldView.transform);
            Container.BindInterfacesAndSelfTo<WorldSimulator>().AsSingle();
        }
    }

}
