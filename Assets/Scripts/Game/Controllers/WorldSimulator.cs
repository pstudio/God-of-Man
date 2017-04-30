using System;
using System.Collections.Generic;
using System.Linq;
using pstudio.GoM.Game.Models.World;
using pstudio.GoM.Game.Models.World.Generators;
using pstudio.GoM.Game.Views;
using pstudio.GoM.Game.Views.Heatmap;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace pstudio.GoM.Game.Controllers
{
    public class WorldSimulator : IInitializable
    {
        private readonly WorldGenerator _worldGenerator;
        private readonly WorldView _worldView;
        private readonly HeatmapView _heatmapView;
        private readonly ActionController _actionController;
        private World _world;

        public ReactiveProperty<int> Generations { get; private set; }
        public ReactiveProperty<bool> GameOver { get; private set; } 

        public WorldSimulator(WorldGenerator worldGenerator, WorldView worldView, HeatmapView heatmapView, ActionController actionController)
        {
            _worldGenerator = worldGenerator;
            _worldView = worldView;
            _heatmapView = heatmapView;
            _actionController = actionController;

            Generations = new ReactiveProperty<int>();
            GameOver = new ReactiveProperty<bool>();
        }

        public void Initialize()
        {
            GenerateNewWorld();
        }

        public void GenerateNewWorld()
        {
            _world = _worldGenerator.Generate();
            _worldView.World = _world;
            _heatmapView.World = _world;
            Generations.Value = 0;
            GameOver.Value = false;
            _actionController.Reset();
        }

        private bool IsWater(Land land)
        {
            return land.Type <= Land.LandType.Water;
        }

        public void DoGeneration()
        {
            // Sigh - God help me :/
            // This code is NSFL

            _actionController.ExecuteActions(_world);

            // Reproduce food
            _world.Land.DoGeneration((x, y, land) =>
            {
                var multiplier = 1.02f;
                var neighborsExclusive = _world.Land.GetNeumannNeighborsExclusive(x, y).ToList();

                if (land.Type == Land.LandType.Grass)
                    multiplier += 0.1f;

                land.Food *= multiplier;
                if (land.Food <= 0f) // food can spread slowly
                    land.Food += neighborsExclusive.Sum(l => l.Food)/16f;


                // animals reproduce and migrate from neighboring tiles
                land.Animals += neighborsExclusive.Where(n => IsWater(n) == IsWater(land)).Sum(l => l.Animals) / 32f;

                land.Food = Mathf.Clamp01(land.Food);
                land.Animals = Mathf.Clamp01(land.Animals);
                
                return land;
            });

            // Handle population
            var newSettlements = new Queue<Population>();
            var farms = new HashSet<Land>();
            int settlementsCount = 0;
            _world.Populations.DoGeneration((x, y, pop) =>
            {
                if (pop.PopulationSize <= 0)
                    return pop;

                settlementsCount++;

                var range = (int) Math.Ceiling(pop.PopulationSize/5f);
                var foodTiles = _world.Land.GetMooreNeighbors(x, y, range).ToList();
                var food = foodTiles.Sum(l => l.Food + l.Animals)*2;
                //Debug.Log(food);
                //Debug.Log(pop.PopulationSize);

                if (food >= pop.PopulationSize)
                    pop.PopulationSize++; // Grow population if plenty of food
                else if (food < pop.PopulationSize/2f)
                {
                    if (pop.PopulationSize > 9) // if population is a city try and kill a small village outside instead
                    {
                        var neigbors =
                            _world.Populations.GetMooreNeighborsExclusive(x, y)
                                .Where(p => p.PopulationSize > 0 && p.PopulationSize < 5).ToList();

                        if (neigbors.Count > 0)
                            neigbors[Random.Range(0, neigbors.Count)].PopulationSize /= 2;
                        else
                            pop.PopulationSize--; // cities have food reserves, so the population doesn't die as fast as in a village

                    }
                    else
                    {
                        pop.PopulationSize /= 2; // Food shortage - kill population
                        if (pop.PopulationSize == 1)
                            pop.PopulationSize = 0; // Kill the remains

                        if (pop.PopulationSize == 0) settlementsCount--;
                    }
                    
                }
                    

                // consume food
                var foodLeft = false;
                foreach (var foodTile in foodTiles)
                {
                    var consumed = (float)Math.Pow(pop.PopulationSize/20f, 2)/range;
                    //Debug.Log(consumed);
                    if (foodTile.Food >= consumed)
                    {
                        foodTile.Food -= consumed;
                        foodLeft = true;
                    }
                    else
                    {
                        consumed -= foodTile.Food;
                        foodTile.Food = 0;
                        foodTile.Animals = Math.Max(0f, foodTile.Animals - consumed);
                        if (foodTile.Animals > 0f)
                            foodLeft = true;
                    }
                }

                // Cities will farm the land and grow new food - multiple cities can farm the same tile but each tile will only receive the food bonus once per generation
                if (pop.PopulationSize > 9)
                {
                    foreach (var foodTile in foodTiles)
                    {
                        farms.Add(foodTile);
                    }
                }

                if (!foodLeft) return pop;

                // With 25% chance there's a baby boom
                if (Random.value < 0.25f + Generations.Value / 500f)
                    pop.PopulationSize++;

                // Large cities may spawn a settler
                if (pop.PopulationSize > 14 && Random.value < 0.1f + Generations.Value / 500f)
                {
                    _world.Settlers.Add(new Settler() {X = x, Y = y, Life = 20});
                    settlementsCount++;
                }

                // Theres a chance to create a new settlement if population is larger than 5
                if (pop.PopulationSize > 5 && Random.value > 1f/pop.PopulationSize - Generations.Value / 500f)
                    newSettlements.Enqueue(pop);

                return pop;
            });

            // grow farms
            foreach (var farm in farms)
            {
                farm.Food += Random.Range(0.0f, 0.05f);
            }

            // Create new populations
            while (newSettlements.Count > 0)
            {
                var pop = newSettlements.Dequeue();
                var grassTiles = _world.Land.GetMooreNeighborsExclusive(pop.X, pop.Y).Where(l => l.Type == Land.LandType.Grass).ToList();
                if (grassTiles.Count == 0) continue;
                var grass = grassTiles[Random.Range(0, grassTiles.Count)];
                if (_world.Populations[grass.X, grass.Y].PopulationSize <= 0)
                {
                    _world.Populations[grass.X, grass.Y].PopulationSize = 1;
                    settlementsCount++;
                }
            }

            // Update settlers
            foreach (var settler in _world.Settlers)
            {
                if (settler.Life <= 0)
                {
                    settler.ShouldDie = true;
                    continue;
                }

                var neighborPops =
                    _world.Populations.GetMooreNeighbors(settler.X, settler.Y).Sum(pop => pop.PopulationSize);
                if (neighborPops > 0)
                {
                    MoveSettler(settler);
                    continue;
                }

                var neighborFood =
                    _world.Land.GetMooreNeighbors(settler.X, settler.Y).Sum(land => land.Food + land.Animals);

                if (neighborFood < 4.5f)
                {
                    MoveSettler(settler);
                    continue;
                }

                // settle
                _world.Populations[settler.X, settler.Y].PopulationSize = 1;
                settler.ShouldDie = true;
            }

            _world.Settlers.RemoveAll(s => s.ShouldDie);

            _worldView.UpdateDecorators();
            _heatmapView.UpdateHeatmap();

            ++Generations.Value;

            if (settlementsCount <= 0 && _world.Settlers.Count == 0)
                GameOver.Value = true;
        }

        private void MoveSettler(Settler settler)
        {
            settler.Life--;

            if (settler.LastDirectionX != 0 || settler.LastDirectionY != 0)
            {
                var land = _world.Land.GetCell(settler.X + settler.LastDirectionX, settler.Y + settler.LastDirectionY);
                if (!IsWater(land))
                {
                    settler.X = land.X;
                    settler.Y = land.Y;
                    return;
                }
            }

            var potentialTilesSorted = _world.Land.GetNeumannNeighborsExclusive(settler.X, settler.Y)
                .Where(l => !IsWater(l))
                .OrderBy(l => l.Food + l.Animals + l.MaxFood + l.MaxAnimals).ToArray();

            var goal = Random.value < 0.5f ? potentialTilesSorted[0] : potentialTilesSorted[Random.Range(0, potentialTilesSorted.Length)];
            settler.LastDirectionX = goal.X - settler.X;
            settler.LastDirectionY = goal.Y - settler.Y;
            settler.X = goal.X;
            settler.Y = goal.Y;
        }
    }

}
