using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    [SerializeField] LineRenderer line;

    public void drawCircle(float radius, Vector3 pos, short points)
    {
        line.positionCount = points + 1;
        Vector3[] positions = new Vector3[points + 1];
        float angle = (Mathf.PI * 2) / points;
        for(int i = 0; i < points; i++)
        {
            float arc = angle * i;
            positions[i] = new Vector3(radius * Mathf.Sin(arc), radius * Mathf.Cos(arc));
        }
        positions[points] = positions[0];
        line.SetPositions(positions);
    }
}
