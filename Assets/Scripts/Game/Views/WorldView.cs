using System;
using System.Collections;
using System.Collections.Generic;
using pstudio.GoM.Game.Models.World;
using UnityEngine;
using Zenject;

namespace pstudio.GoM.Game.Views
{
    public class WorldView : MonoBehaviour
    {
        private World _world;
        public World World { get { return _world; } set { _world = value; UpdateMap(); } }

        public TileView TilePrefab;
        public Settings ViewSettings;
        [Inject] public TileView.Factory TileFactory { get; set; }
        private TileView[,] _landTiles;

        private void GenerateMap()
        {
            _landTiles = new TileView[World.Width, World.Height];
            for (var x = 0; x < World.Width; ++x)
                for (var y = 0; y < World.Height; ++y)
                {
                    var tile = TileFactory.Create();
                    tile.Land = World.Land[x, y];
                    _landTiles[x, y] = tile;
                }

            transform.position = new Vector3(-_world.Width / 2f, -_world.Height / 2f + 1f, 0f);

            UpdateDecorators();
        }

        //public delegate void WorldUpdated(World world);
        //public event WorldUpdated WorldUpdatedEvent;

        public void UpdateMap()
        {
            if (_landTiles == null)
            {
                GenerateMap();
                return;
            }

            for (var x = 0; x < World.Width; ++x)
                for (var y = 0; y < World.Height; ++y)
                    _landTiles[x, y].Land = World.Land[x, y];

            UpdateDecorators();
        }

        public void UpdateDecorators()
        {
            for(var x = 0; x < World.Width; ++x)
                for (var y = 0; y < World.Height; ++y)
                {
                    _landTiles[x, y].SetDecorator(null);

                    if (_world.Populations[x, y].PopulationSize > 0)
                        _landTiles[x, y].SetDecorator(
                            ViewSettings.CitySprites[CityRank(_world.Populations[x, y].PopulationSize)]);
                }

            foreach (var settler in _world.Settlers)
            {
                _landTiles[settler.X, settler.Y].SetDecorator(ViewSettings.SettlerSprite);
            }

            //if (WorldUpdatedEvent != null) WorldUpdatedEvent.Invoke(World);
        }

        private int CityRank(int population)
        {
            if (population > 14) return 3;
            if (population > 9) return 2;
            if (population > 4) return 1;
            if (population > 0) return 0;
            return -1;
        }

        public void Animate(int x, int y, int range = 0, bool neumann = false)
        {
            _landTiles[x, y].Animate();

            if (range > 0)
                StartCoroutine(AnimateRange(x, y, range, neumann));
        }

        readonly WaitForSeconds _animationWait = new WaitForSeconds(0.2f);

        private IEnumerator AnimateRange(int x, int y, int range, bool neumann)
        {
            //var rings = new List<IEnumerable<TileView>>();

            for (int i = 1; i <= range; i++)
            {
                yield return _animationWait;

                IEnumerable<Land> tiles;

                if (neumann)
                    tiles = _world.Land.GetNeumannNeighborsExclusive(x, y, i);
                else
                    tiles = _world.Land.GetMooreNeighborsExclusive(x, y, i);

                foreach (var tile in tiles)
                {
                    _landTiles[tile.X, tile.Y].Animate();
                }
            }
        }

        [Serializable]
        public class Settings
        {
            public List<Sprite> LandSprites;
            public List<Sprite> CitySprites;
            public Sprite SettlerSprite;
        }
    }

}
