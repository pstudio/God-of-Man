using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace pstudio.GoM.Game.Models.World.Generators
{
    public class PopulationGenerator : IGenerator<Population>
    {
        private readonly Settings _settings;

        public PopulationGenerator(Settings settings)
        {
            _settings = settings;
        }

        public IGrid<Population> Generate(int width, int height, object param)
        {
            var land = param as IGrid<Land>;
            var population = new Grid<Population>(width, height, (x, y) => new Population() {X = x, Y = y});

            var populationCount = Random.Range(_settings.MinInitialPopulations,
                _settings.MaxInitialPopulations + 1);
            var grass = land.Where(l => l.Type == Land.LandType.Grass).ToList();
            for (var i = 0; i < populationCount; i++)
            {
                var tile = grass[Random.Range(0, grass.Count)];
                population[tile.X, tile.Y].PopulationSize = Random.Range(_settings.MinPopulationSize, _settings.MaxPopulationSize + 1);
            }

            return population;
        }

        [Serializable]
        public class Settings
        {
            public int MinInitialPopulations = 2;
            public int MaxInitialPopulations = 8;
            public int MinPopulationSize = 1;
            public int MaxPopulationSize = 4;
        }
    }
}
