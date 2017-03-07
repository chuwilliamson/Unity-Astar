using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeBehaviour : MonoBehaviour, IPointerClickHandler , IPointerEnterHandler
{
    public int f, g, h;
    public List<GameObject> Neighbors;    
    public ScriptableNode Node;
    public IGridBehaviour gridBehaviour;
    public float scaleFactor = 1.25f;
    private Vector3 scale;
 
    private void Start()
    { 
        scale = transform.localScale;
        Neighbors = new List<GameObject>();
        Node.Neighbors.ForEach(n => Neighbors.Add(gridBehaviour.GetChild(n)));
        if(Block)
            Node.Walkable = false;
        if(!Node.Walkable)
            gridBehaviour.SetColor(Node, Color.red);        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.selectedObject = gameObject;
            gridBehaviour.SetGoal(Node);
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {            
            gridBehaviour.SetStart(Node);
        }
        eventData.Use();
    }
    public bool Block;
    
    private void Update()
    {     
        if(Node.Parent)
            Debug.DrawLine(transform.position, gridBehaviour.GetChild(Node.Parent).transform.position);
        f = Node.F;
        g = Node.G;
        h = Node.H;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Input.GetMouseButton(1))
        {
            Node.Walkable = !Node.Walkable;
            if(!Node.Walkable)
                gridBehaviour.SetColor(Node, Color.red);
            else
                gridBehaviour.SetColor(Node, Color.white);
          
            StartCoroutine("TweenScale");           
        }

        eventData.Use();
    }
    public void Tween()
    {
        StopCoroutine("TweenScale");
        StartCoroutine("TweenScale");
    }
    private IEnumerator TweenScale()
    {
        float timer = 0;
        Vector3 oldScale = scale;
        
        while(timer < .5f)
        {            
            transform.localScale = Vector3.Lerp(oldScale, oldScale * scaleFactor, timer /1f);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = oldScale;
        yield return null;
    }

  
}











