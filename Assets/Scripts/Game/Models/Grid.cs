using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace pstudio.GoM.Game.Models
{
    public class Grid<T> : IGrid<T> where T : class, new()
    {
        private T[,] _grid;
        private T[,] _buffer;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            _grid = new T[Width, Height];
            _buffer = new T[Width, Height];
        }

        public Grid(int width, int height, Func<int, int, T> initializer) : this(width, height)
        {
            for (var i = 0; i < Width; ++i)
                for (var o = 0; o < Height; ++o)
                    _grid[i, o] = initializer.Invoke(i, o);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var t in _grid)
            {
                yield return t;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public T GetCell(int x, int y)
        {
            return
                _grid[
                    x >= 0 ? x%Width : (Width - Math.Abs(x%Width))%Width,
                    y >= 0 ? y%Height : (Height - Math.Abs(y%Height))%Height];
        }

        public void SetCell(int x, int y, T cell)
        {
            _grid[
                x >= 0 ? x%Width : (Width - Math.Abs(x%Width))%Width,
                y >= 0 ? y%Height : (Height - Math.Abs(y%Height))%Height] = cell;
        }

        public T this[int x, int y]
        {
            get { return _grid[x, y]; }
            set { _grid[x, y] = value; }
        }

        public IEnumerable<T> GetNeumannNeighbors(int x, int y, int range = 1)
        {
            Assert.IsTrue(range > 0);

            var neighbors = new List<T>(1 + 2*range*(range + 1));

            for (var i = -range; i <= range; ++i)
                for (var o = -range; o <= range; ++o)
                    if (Math.Abs(i) + Math.Abs(o) <= range)
                        neighbors.Add(GetCell(i + x, o + y));

            return neighbors;
        }

        public IEnumerable<T> GetNeumannNeighborsExclusive(int x, int y, int range = 1)
        {
            return GetNeumannNeighbors(x, y, range).Except(new[] {GetCell(x, y)});
        }

        public IEnumerable<T> GetMooreNeighbors(int x, int y, int range = 1)
        {
            Assert.IsTrue(range > 0);

            var neighbors = new List<T>((2*range + 1)*(2*range + 1));

            for (var i = -range; i <= range; ++i)
                for (var o = -range; o <= range; ++o)
                    neighbors.Add(GetCell(i + x, o + y));

            return neighbors;
        }

        public IEnumerable<T> GetMooreNeighborsExclusive(int x, int y, int range = 1)
        {
            return GetMooreNeighbors(x, y, range).Except(new[] {GetCell(x, y)});
        }

        public void DoGeneration(Func<int, int, T, T> update)
        {
            for (var i = 0; i < Width; ++i)
                for (var o = 0; o < Height; ++o)
                    _buffer[i, o] = update.Invoke(i, o, _grid[i, o]);

            var temp = _grid;
            _grid = _buffer;
            _buffer = temp;
        }
    }
}
