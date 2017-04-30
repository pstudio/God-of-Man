using System;
using System.Collections.Generic;

namespace pstudio.GoM.Game.Models.World.Generators
{
    public class WorldGenerator
    {
        private readonly Settings _settings;
        private readonly IGenerator<Land> _landGenerator;
        private readonly IGenerator<Population> _populationGenerator; 
         
        public WorldGenerator(Settings settings, IGenerator<Land> landGenerator, IGenerator<Population> populationGenerator)
        {
            _settings = settings;
            _landGenerator = landGenerator;
            _populationGenerator = populationGenerator;
        }

        public World Generate()
        {
            var world = new World();
            world.Land = _landGenerator.Generate(_settings.WorldWidth, _settings.WorldHeight, null);
            world.Populations = _populationGenerator.Generate(_settings.WorldWidth, _settings.WorldHeight, world.Land);
            world.Settlers = new List<Settler>();

            return world;
        }

        [Serializable]
        public class Settings
        {
            public int WorldWidth = 16;
            public int WorldHeight = 9;
        }
    }

}
