using System;

namespace pstudio.GoM.Game.Models.World
{
    public class Land : Cell
    {
        private float _food;
        private float _animals;

        public enum LandType
        {
            Ocean,
            Water,
            Dirt,
            Grass,
            Rock,
            Sand
        }

        public Land() : this(LandType.Ocean, 1f, 1f)
        {
            
        }

        public Land(LandType landType, float maxFood, float maxAnimals)
        {
            Type = landType;
            MaxFood = maxFood;
            MaxAnimals = maxAnimals;
            Food = MaxFood;
            Animals = MaxAnimals;
        }

        public LandType Type { get; set; }

        public float Food
        {
            get { return _food; }
            set { _food = Math.Min(value, MaxFood); }
        }

        public float MaxFood { get; private set; }

        public float Animals
        {
            get { return _animals; }
            set { _animals = Math.Min(value, MaxAnimals); }
        }

        public float MaxAnimals { get; private set; }
    }

}
