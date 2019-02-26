using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPath : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    void Start()
    {
        Vector3[] pos = new Vector3[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            pos[i] = transform.GetChild(i).position;
        }
        line.positionCount = transform.childCount;
        line.SetPositions(pos);
    }
}
