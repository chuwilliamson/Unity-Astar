using System.Collections.Generic;
using UnityEngine;
using AIE;
public class ScriptableGrid : ScriptableObject
{
    public void Create(AstarGrid grid)
    {
        Nodes = new List<AstarNode>();
        foreach(AstarNode n in grid.Nodes)
            Nodes.Add(n);

    
        
    }


    public List<AstarNode> Nodes;

    /// <summary>
    /// gridbehaviour uses this
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AstarNode GetNode(int id)
    {
        return Nodes[id];
    }

    public AstarNode GetNode(Point p)
    {
        AstarNode node = Nodes.Find(n => n.U == p.U && n.V == p.V);
        return node;
    }

    public List<AstarNode> GetNeighbors(int id)
    {
        var node = Nodes[id];
        var dirs = new List<Point>()
        {
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),
            new Point(-1, 1),
            new Point(-1, 0),
            new Point(-1, -1),
            new Point(0, -1),
            new Point(1, -1),

        };
        var neighbors = new List<AstarNode>();

        foreach(var dir in dirs)
        {
            var pos = new Point(node.U + dir.U, node.V + dir.V);
            var nay = GetNode(pos);
            if(nay != null)
                neighbors.Add(nay);
        }

        return neighbors;
    }
}

