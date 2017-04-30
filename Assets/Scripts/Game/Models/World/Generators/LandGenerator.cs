using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace pstudio.GoM.Game.Models.World.Generators
{
    public class LandGenerator : IGenerator<Land>
    {
        private readonly Settings _settings;

        public LandGenerator(Settings settings)
        {
            _settings = settings;
        }

        public IGrid<Land> Generate(int width, int height, object param = null)
        {
            var land = new Grid<Land>(width, height,
                (x, y) => Random.value < _settings.InitialLandPercentage ? CreateDirt(x, y) : CreateOcean(x, y));

            // Create land mass
            for (var i = 0; i < _settings.Generations; i++)
            {
                land.DoGeneration((x, y, cell) =>
                {
                    var count = land.GetMooreNeighborsExclusive(x, y).Count(n => n.Type == Land.LandType.Dirt);
                    if (count < _settings.DieNeighbors)
                        return CreateOcean(x, y);
                    if (count > _settings.GrowNeighbors)
                        return CreateDirt(x, y);
                    return land[x, y];
                });
            }

            // Create shore line
            land.DoGeneration(((x, y, cell) =>
            {
                if (land[x, y].Type != Land.LandType.Dirt) return land[x, y];

                var count = land.GetNeumannNeighborsExclusive(x, y).Count(n => n.Type == Land.LandType.Ocean);
                if (count > 0 && Random.value < _settings.ShorePercentage)
                    return CreateSand(x, y);

                return land[x, y];
            }));

            // Create shallow water
            land.DoGeneration(((x, y, cell) =>
            {
                if (land[x, y].Type != Land.LandType.Ocean) return land[x, y];

                var count = land.GetMooreNeighborsExclusive(x, y, _settings.OceanRange).Count(n => n.Type != Land.LandType.Ocean);
                if (count > 0)
                    return CreateWater(x, y);

                return land[x, y];
            }));

            // Create rocks
            land.DoGeneration(((x, y, cell) =>
            {
                if (land[x, y].Type != Land.LandType.Dirt) return land[x, y];

                if (Random.value < _settings.RockPercentage)
                    return CreateRock(x, y);

                return land[x, y];
            }));

            // Create grass
            land.DoGeneration(((x, y, cell) =>
            {
                if (land[x, y].Type != Land.LandType.Dirt) return land[x, y];

                if (Random.value < _settings.GrassPercentage)
                    return CreateGrass(x, y);

                return land[x, y];
            }));

            return land;
        }

        private Land CreateOcean(int x, int y)
        {
            return CreateLand(Land.LandType.Ocean, x, y, _settings.Ocean);
        }

        private Land CreateWater(int x, int y)
        {
            return CreateLand(Land.LandType.Water, x, y, _settings.Water);
        }

        private Land CreateDirt(int x, int y)
        {
            return CreateLand(Land.LandType.Dirt, x, y, _settings.Dirt);
        }

        private Land CreateGrass(int x, int y)
        {
            return CreateLand(Land.LandType.Grass, x, y, _settings.Grass);
        }

        private Land CreateRock(int x, int y)
        {
            return CreateLand(Land.LandType.Rock, x, y, _settings.Rock);
        }

        private Land CreateSand(int x, int y)
        {
            return CreateLand(Land.LandType.Sand, x, y, _settings.Sand);
        }

        private Land CreateLand(Land.LandType landType, int x, int y, Settings.TileSettings tileSettings)
        {
            var maxFood = Mathf.Clamp(Random.value, tileSettings.MinFood, tileSettings.MaxFood);
            var maxAnimals = Mathf.Clamp(Random.value, tileSettings.MinAnimals, tileSettings.MaxAnimals);

            return new Land(landType, maxFood, maxAnimals)
            {
                X = x,
                Y = y,
            }; // using clamp like this will give a bias towards the min/max boundary values
        }

        [Serializable]
        public class Settings
        {
            public float InitialLandPercentage = 0.5f;
            public int DieNeighbors = 3;
            public int GrowNeighbors = 5;
            public int Generations = 3;
            public float ShorePercentage = 0.8f;
            public int OceanRange = 2;
            public float RockPercentage = 0.05f;
            public float GrassPercentage = 0.9f;

            public TileSettings Ocean;
            public TileSettings Water;
            public TileSettings Dirt;
            public TileSettings Grass;
            public TileSettings Rock;
            public TileSettings Sand;


            [Serializable]
            public class TileSettings
            {
                public float MinFood = 0f;
                public float MaxFood = 1f;
                public float MinAnimals = 0f;
                public float MaxAnimals = 1f;
            }
        }
    }

}
