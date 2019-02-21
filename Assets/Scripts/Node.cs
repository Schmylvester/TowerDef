using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Structure structure = null;
    public Node next_node;


#if (UNITY_EDITOR)
    private void OnDrawGizmos()
    {
        if(next_node)
        UnityEditor.Handles.DrawLine(transform.position, next_node.transform.position);
    }
#endif
}
