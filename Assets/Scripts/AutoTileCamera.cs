using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTileCamera : MonoBehaviour
{

    int rows;
    void Start()
    {
        float sqChild = Mathf.Sqrt(transform.childCount);
        if (sqChild % 1 == 0)
        {
            rows = (int)sqChild;
        }
        else
        {
            rows = (int)sqChild + 1;
        }

        float camSize = 1.0f / rows;

        int x = 0;
        int y = 0;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            canvas.transform.parent.position = new Vector3(x * 50, y * 50);
            canvas.worldCamera.rect = new Rect(new Vector2(x, y) * camSize, Vector2.one * camSize);
            if (++x == rows)
            {
                x = 0;
                y++;
            }
        }
    }
}
