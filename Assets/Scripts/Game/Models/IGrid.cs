using System;
using System.Collections.Generic;

namespace pstudio.GoM.Game.Models
{
    public interface IGrid<T> : IEnumerable<T> where T : class, new()
    {
        /// <summary>
        /// Number of cells in a row
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Number of cells in a column
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Returns the cell at coordinate [x,y]. This function automatically wraps the x and y values.
        /// E.g. if Width = 5, Height = 4 then coordinate [x=6, y=-3] returns the cell at [1, 1].
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>The cell at the specified coordinate</returns>
        T GetCell(int x, int y);

        /// <summary>
        /// Sets the cell at coordinate [x,y]. This function automatically wraps the x and y values.
        /// E.g. if Width = 5, Height = 4 then coordinate [x=6, y=-3] sets the cell at [1, 1].
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="cell">The cell</param>
        void SetCell(int x, int y, T cell);

        /// <summary>
        /// Raw access to the cells.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
        T this[int x, int y] { get; set; }

        /// <summary>
        /// Get Neumann Neighbors
        /// https://en.wikipedia.org/wiki/Von_Neumann_neighborhood
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="range">Range of neighborhood. Range must be larger than 0</param>
        /// <returns>The Neumann neighborhood</returns>
        IEnumerable<T> GetNeumannNeighbors(int x, int y, int range = 1);

        /// <summary>
        /// Get Neumann Neighbors excluding the cell at [x,y]
        /// https://en.wikipedia.org/wiki/Von_Neumann_neighborhood
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="range">Range of neighborhood. Range must be larger than 0</param>
        /// <returns>The Neumann neighborhood excluding the cell at [x,y]</returns>
        IEnumerable<T> GetNeumannNeighborsExclusive(int x, int y, int range = 1);

        /// <summary>
        /// Get Moore Neighbors
        /// https://en.wikipedia.org/wiki/Moore_neighborhood
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="range">Range of neighborhood. Range must be larger than 0</param>
        /// <returns>The Moore neighborhood</returns>
        IEnumerable<T> GetMooreNeighbors(int x, int y, int range = 1);

        /// <summary>
        /// Get Moore Neighbors excluding the cell at [x,y]
        /// https://en.wikipedia.org/wiki/Moore_neighborhood
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="range">Range of neighborhood. Range must be larger than 0</param>
        /// <returns>The Moore neighborhood excluding the cell at [x,y]</returns>
        IEnumerable<T> GetMooreNeighborsExclusive(int x, int y, int range = 1);

        /// <summary>
        /// Treat grid as a cellular automaton and get a new generation.
        /// </summary>
        /// <param name="update">A function that receives the x and y coordinate of the cell plus the cell itself. It returns a new cell object</param>
        void DoGeneration(Func<int, int, T, T> update);
    }
}
