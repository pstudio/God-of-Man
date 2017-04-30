using System;
using System.Collections.Generic;
using System.Linq;
using pstudio.GoM.Game.Models.World;
using pstudio.GoM.Game.Views.UI;
using UnityEngine;

namespace pstudio.GoM.Game.Views.Heatmap
{
    public class HeatmapView : MonoBehaviour
    {
        [SerializeField] private float _mapScale = 1.3333333333f;
        [SerializeField] private float _mapAlpha = 0.5f;
        private float[] _data;
        private World _world;
        public World World { get { return _world; } set { _world = value; UpdateHeatmap(); } }
        private Material _material;

        private List<HeatmapFunction> _heatmaps;
        private int _heatmapIndex = 0;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().material;

            _heatmaps = new List<HeatmapFunction>();
            
            #region Heatmap setup

            // No map - not pretty
            _heatmaps.Add(new HeatmapFunction()
            {
                Name = "None",
                Function = (x, y, world) => {
                    HideMap();
                    return 0f;
                }
            });

            // Show food
            _heatmaps.Add(new HeatmapFunction()
            {
                Name = "Food",
                Function = (x, y, world) => World.Land[x, y].Food
            });

            // Show animals
            _heatmaps.Add(new HeatmapFunction()
            {
                Name = "Animals",
                Function = (x, y, world) => World.Land[x, y].Animals
            });

            // Show population
            _heatmaps.Add(new HeatmapFunction()
            {
                Name = "Population",
                Function = (x, y, world) => 1f - World.Populations[x, y].PopulationSize / 10f
            });

            #endregion

            // what a hack - this sure is a jam - need to figure out a way to make Marklight work with Zenject
            var heatmapSelector = GameObject.Find("HeatmapSelector").GetComponent<HeatmapSelector>();
            heatmapSelector._heatmapView = this;
        }

        public IEnumerable<string> GetHeatmapNames()
        {
            return _heatmaps.Select(hf => hf.Name);
        } 

        public void UpdateHeatmap()
        {
            if (_data == null || _data.Length == 0)
                _data = new float[World.Width * World.Height];

            transform.localScale = new Vector3(World.Width, World.Height, 1f);
            //transform.localScale = new Vector3(World.Width/_mapScale, World.Height/_mapScale, 1f);
            //transform.position = new Vector3(World.Width/2f, World.Height/2f);

            for (var y = 0; y < World.Height; ++y)
                for (var x = 0; x < World.Width; ++x)
                    _data[y*World.Width + x] = _heatmaps[_heatmapIndex].Function.Invoke(x, y, World);

            _material.SetInt("_Data_Array_Width", World.Width);
            _material.SetInt("_Data_Array_Height", World.Height);
            _material.SetFloatArray("_Data_Array", _data);
            _material.SetFloat("_Alpha", _mapAlpha);
        }

        public void ShowMap(int mapIndex)
        {
            gameObject.SetActive(true);
            _heatmapIndex = mapIndex;
            UpdateHeatmap();
        }

        public void HideMap()
        {
            gameObject.SetActive(false);
        }

        public class HeatmapFunction
        {
            public string Name;
            public Func<int, int, World, float> Function;
        }
    }

}
