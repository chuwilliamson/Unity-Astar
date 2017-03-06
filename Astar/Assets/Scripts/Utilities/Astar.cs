using System;
using System.Collections.Generic;


namespace AIE
{
    /// <summary>
    /// interface for everything even the attributes
    /// </summary>
    public interface INode
    {
        Point Position { get; }
        int Id { get; }
    }

    public interface IGrid
    {
        int Rows { get; set; }
        int Cols { get; set; }
        void Generate();
        void Update();
    }

    public class Astar
    {
        public Astar(AstarNode start, AstarNode goal)
        {

        }
    }
}
