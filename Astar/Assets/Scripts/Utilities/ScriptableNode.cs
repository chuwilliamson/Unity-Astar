using UnityEngine;
using System.Collections.Generic;
using AIE;
public class ScriptableNode : ScriptableObject
{
    public int Id;

    public int U;
    public int V;

    public int G;
    public int H;
    public int _f;
    public int F
    {
        get
        {
            _f = G + H;
            return G + H;
        }
    }

    public ScriptableNode Parent;
    public List<ScriptableNode> Neighbors;

    public bool Walkable { get; set; }
    public void Create(AstarNode n)
    {
        Parent = null;
        Neighbors = new List<ScriptableNode>();
        G = n.G;
        H = n.H;
        U = n.U;
        V = n.V;
        Id = n.Id;
    }


}
