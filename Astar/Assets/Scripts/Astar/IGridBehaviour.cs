using System.Collections.Generic;
using UnityEngine;

public interface IGridBehaviour
{
    void Clear();
    void ClearNodes();
    void CreateGameObjects();
    void CreateNodes();
    GameObject GetChild(ScriptableNode s);
    List<ScriptableNode> Neighbors(ScriptableNode node);
    void SetColor(ScriptableNode s, Color c);
    void SetColor(GameObject go, Color c);
    void SetGoal(ScriptableNode s);
    void SetStart(ScriptableNode s);
}