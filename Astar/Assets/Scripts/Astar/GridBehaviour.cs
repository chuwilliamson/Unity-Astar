using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIE;
public class GridBehaviour : MonoBehaviour, IGridBehaviour
{
    /// <summary>
    /// the GameObject Container
    /// </summary>
    public List<GameObject> Children;
    /// <summary>
    /// The nodes
    /// </summary>
    [HideInInspector]
    public List<ScriptableNode> Nodes;
    public GameObject prefab;
    public int Rows = 5;
    public int Cols = 5;
    public float Scale = 5;
    public float Offset = 5;


    /*
        1. Find node closest to your position and declare it start node and put it on 
            the open list. 
        2. While there are nodes in the open list:
        3. Pick the node from the open list having the smallest F score. Put it on 
            the closed list (you don't want to consider it again).
        4. For each neighbor (adjacent cell) which isn't in the closed list:
        5. Set its parent to current node.
        6. Calculate G score (distance from starting node to this neighbor) and 
            add it to the open list
        7. Calculate F score by adding heuristics to the G value. 
     */

    public ScriptableNode Goal;
    public ScriptableNode Current;
    public ScriptableNode StartNode;

    public List<ScriptableNode> Open = new List<ScriptableNode>();
    public List<ScriptableNode> Closed = new List<ScriptableNode>();
    public List<ScriptableNode> Path = new List<ScriptableNode>();
    public IEnumerator Astar(ScriptableNode start, ScriptableNode goal)
    {
        Current = start;
        StartNode = start;
        AddToOpen(Current);
        while(Current != goal || Open.Count > 0)
        {
            Open.Sort((a, b) => a.F.CompareTo(b.F));
            Current = Open[0];
            AddToClosed(Current);
            if(Closed.Contains(goal))
            {
                StopAllCoroutines();
                break;
            }

            foreach(var neighbor in Current.Neighbors)
            {
                int tentative_gScore = CostToMove(Current, neighbor) + Current.G;
                neighbor.H = Utilities.ManhattanDistance(neighbor.U, neighbor.V, goal.U, goal.V);

                if(!Closed.Contains(neighbor) && neighbor.Walkable)
                {
                    if(!Open.Contains(neighbor))
                    {
                        AddToOpen(neighbor);
                        neighbor.G = tentative_gScore;
                        neighbor.F = neighbor.G + neighbor.H; ;
                        neighbor.Parent = Current;
                    }
                    else if(Open.Contains(neighbor))
                    {
                        if(tentative_gScore < neighbor.G)
                        {
                            neighbor.G = tentative_gScore;
                            neighbor.F = neighbor.G + neighbor.H;
                            neighbor.Parent = Current;
                        }
                    }
                }
            }

            yield return null;
        }
        RetracePath(goal);
    }
    void Awake()
    {
        Children.ForEach(child => child.GetComponent<NodeBehaviour>().gridBehaviour = this);
    }
    void Start()
    {

        Current = Nodes[0];
        Goal = Nodes[Nodes.Count - 1];
    }


    public void RetracePath(ScriptableNode s)
    {
        var iterator = s;
        while(iterator != StartNode)
        {
            Path.Add(iterator);
            iterator = iterator.Parent;
        }
        Path.Add(StartNode);
        Path.ForEach(n => SetColor(GetChild(n), Color.yellow));
    }

    public int CostToMove(ScriptableNode current, ScriptableNode neighbor)
    {
        int cost = (current.U == neighbor.U || current.V == neighbor.V) ? 10 : 14;
        return cost;
    }

    public void AddToOpen(ScriptableNode s)
    {
        Open.Add(s);
        GetChild(s).GetComponent<NodeBehaviour>().Tween();
        GetChild(s).GetComponent<MeshRenderer>().material.color = Color.cyan;
    }

    public void AddToClosed(ScriptableNode s)
    {
        Open.Remove(s);
        Closed.Add(s);
        GetChild(s).GetComponent<NodeBehaviour>().Tween();
        GetChild(s).GetComponent<MeshRenderer>().material.color = Color.grey;
    }

    public void SetGoal(ScriptableNode s)
    {
        Clear();
        Open.Clear();
        Closed.Clear();
        Path.Clear();
        Goal = s;
        Goal.Walkable = true;
        SetColor(GetChild(Goal), Color.green);
        Nodes.ForEach(n => n.H = Utilities.ManhattanDistance(n.U, n.V, Goal.U, Goal.V));
        StopAllCoroutines();
        StartCoroutine(Astar(Current, Goal));
    }

    public void SetStart(ScriptableNode s)
    {
        Clear();
        Open.Clear();
        Closed.Clear();
        Path.Clear();
        s.Walkable = true;
        SetColor(GetChild(s), Color.green);
        Current = s;
    }

    public GameObject GetChild(ScriptableNode s)
    {
        if(Children[s.Id].GetComponent<NodeBehaviour>().Node != s)
        {
            Debug.LogError("node behaviour node mismatch");
            return null;
        }
        return Children[s.Id];
    }

    public void Clear()
    {
        Children.ForEach(child => SetColor(child, Color.white));
        foreach(var n in Nodes)
        {
            if(!n.Walkable)
                SetColor(GetChild(n), Color.red);
        }

        Nodes.ForEach(node => node.Parent = null);
    }

    public void SetColor(GameObject go, Color c)
    {
        go.GetComponent<MeshRenderer>().material.color = c;
    }

    public void SetColor(ScriptableNode s, Color c)
    {
        GameObject go = GetChild(s);
        go.GetComponent<MeshRenderer>().material.color = c;
    }

    [ContextMenu("Clear")]
    public void ClearNodes()
    {
        if(Nodes.Count > 0)
            Nodes.ForEach(node => DestroyImmediate(node, true));
        Nodes.Clear();

        if(Children.Count > 0)
            Children.ForEach(child => DestroyImmediate(child));
        Children.Clear();
    }

    [ContextMenu("Create Nodes")]
    public void CreateNodes()
    {
        ClearNodes();
        Children = new List<GameObject>();
        Nodes = new List<ScriptableNode>();
        var positions = Utilities.CreateGrid(Rows, Cols);

        foreach(var p in positions)
        {
            var node = ScriptableObject.CreateInstance<ScriptableNode>();
            node.Create(new AstarNode(p.U, p.V, Nodes.Count));
            node.name = string.Format("Node {0}", Nodes.Count.ToString());
            Nodes.Add(node);
        }

        Nodes.ForEach(n => n.Neighbors = Neighbors(n));     
        Current = Nodes[0];
        Goal = Nodes[Nodes.Count - 1];
        CreateGameObjects();
    }

    public void CreateGameObjects()
    {
        foreach(var n in Nodes)
        {
            GameObject go = (prefab == null) ? GameObject.CreatePrimitive(PrimitiveType.Quad) : go = Instantiate(prefab);
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(n.U * Offset, n.V * Offset);
            go.transform.localScale *= Scale;
            go.name = string.Format("Node {0}", n.Id);

            var nb = go.GetComponent<NodeBehaviour>();
            if(nb == null)
                nb = go.AddComponent<NodeBehaviour>();

            nb.Node = n;
            nb.gridBehaviour = this;
            Children.Add(go);
        }
    }

    public List<ScriptableNode> Neighbors(ScriptableNode node)
    {
        var neighbors = new List<ScriptableNode>();
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

        foreach(var dir in dirs)
        {
            var nay = Nodes.Find(n => n.U == node.U + dir.U && n.V == node.V + dir.V);
            if(nay != null)
                neighbors.Add(nay);
        }

        return neighbors;
    }
}
