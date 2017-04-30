using System;
using System.Collections.Generic;
using pstudio.GoM.Game.Models.World;
using pstudio.GoM.Game.Views;
using pstudio.GoM.Game.Views.UI;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace pstudio.GoM.Game.Controllers
{
    public class ActionController : IInitializable
    {
        private ActionSelector _selector;
        private WorldView _worldView;
        private readonly List<GodAction> _actions;
        private readonly Queue<GodAction> _queue;
        private int _actionIndex;

        public bool AcceptInput { get; set; }

        public ActionController(WorldView worldView)
        {
            _worldView = worldView;

            _queue = new Queue<GodAction>();
            _actions = new List<GodAction>();
            _actionIndex = 0;

            _actions.Add(new GodAction("No Action", -1, null));

            _actions.Add(new GodAction("Bless Land", 10, (world, x, y) =>
            {
                var neighbors = world.Land.GetMooreNeighbors(x, y, 3);
                foreach (var neighbor in neighbors)
                {
                    neighbor.Food = 1f;
                    neighbor.Animals = 1f;
                    //neighbor.Animals = 1f;
                }
            })
            {
                AniNeumann = false,
                AniRange = 3
            });

            _actions.Add(new GodAction("Small Earthquake", 10, (world, x, y) =>
            {
                var neighbors = world.Populations.GetNeumannNeighbors(x, y, 2);
                foreach (var neighbor in neighbors)
                {
                    neighbor.PopulationSize = Random.Range(0, neighbor.PopulationSize);
                }

                var mooreNeighbors = world.Land.GetMooreNeighbors(x, y, 2);
                foreach (var neighbor in mooreNeighbors)
                {
                    neighbor.Food = Random.Range(0f, neighbor.Food);
                    neighbor.Animals = Random.Range(0f, neighbor.Animals);
                }
            })
            {
                AniNeumann = true,
                AniRange = 2
            });

            _actions.Add(new GodAction("Large Earthquake", 25, (world, x, y) =>
            {
                var neighbors = world.Populations.GetNeumannNeighbors(x, y, 5);
                foreach (var neighbor in neighbors)
                {
                    neighbor.PopulationSize = Random.Range(0, neighbor.PopulationSize/2);
                }

                var mooreNeighbors = world.Land.GetMooreNeighbors(x, y, 5);
                foreach (var neighbor in mooreNeighbors)
                {
                    neighbor.Food = Random.Range(0f, neighbor.Food);
                    neighbor.Animals = Random.Range(0f, neighbor.Animals);
                }
            })
            {
                AniNeumann = true,
                AniRange = 5
            });


            _actions.Add(new GodAction("Send Angel of Death", 40, (world, x, y) =>
            {
                var neighbors = world.Populations.GetMooreNeighbors(x, y, 3);
                foreach (var neighbor in neighbors)
                {
                    neighbor.PopulationSize = 0;
                }
            })
            {
                AniNeumann = false,
                AniRange = 3
            });
        }

        public void Initialize()
        {
            _selector = GameObject.Find("ActionSelector").GetComponent<ActionSelector>();
            _selector.RegisterActions(this, _actions);
        }

        public void ActionSelected(int actionIndex)
        {
            _actionIndex = actionIndex;
            AcceptInput = actionIndex > 0;
        }

        public void Deselect()
        {
            _actionIndex = 0;
            AcceptInput = false;
            _selector.Actions.SelectedIndex = 0;
            _selector.UpdateView();
        }

        public void QueueAction(int x, int y)
        {
            if (_actionIndex == 0) return;
            
            if (_actions[_actionIndex].CanExecute)
            {
                _actions[_actionIndex].X = x;
                _actions[_actionIndex].Y = y;
                _queue.Enqueue(_actions[_actionIndex]);
                AcceptInput = false;
                
                _actions[_actionIndex].Queued();
                _selector.Actions.SelectedIndex = 0;
                _selector.UpdateView();
            }
        }

        public void ExecuteActions(World world)
        {
            while (_queue.Count > 0)
            {
                var action = _queue.Dequeue();
                action.Execute(world);
                _worldView.Animate(action.X, action.Y, action.AniRange, action.AniNeumann);
            }

            foreach (var godAction in _actions)
            {
                godAction.Tick();
            }

            _selector.UpdateView();
        }

        public class GodAction
        {
            private Action<World, int, int> _action;
            public string Name { get; private set; }
            public bool CanExecute { get { return _counter <= 0; } set { ; } }

            public int Cost { get; private set; }
            private int _counter;

            public int AniRange { get; set; }
            public bool AniNeumann { get; set; }

            public GodAction(string name, int cost, Action<World, int, int> action)
            {
                _action = action;
                Name = name;
                Cost = cost;
            }

            public int X { get; set; }
            public int Y { get; set; }

            public void Queued()
            {
                _counter = 1;
            }

            public bool Execute(World world)
            {
                //if (!CanExecute)
                //    return false;

                _action.Invoke(world, X, Y);
                Cost++;
                _counter = Cost;
                return true;
            }

            public void Tick()
            {
                _counter--;
            }

            public void Reset(int cost)
            {
                Cost = cost;
                _counter = 0;
            }
        }

        // ugly hotfix for making sure action costs are reset
        public void Reset()
        {
            _actions[1].Reset(10);
            _actions[2].Reset(10);
            _actions[3].Reset(25);
            _actions[4].Reset(40);
        }
    }
}
