using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowPath : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "100Games")
        {
            line.enabled = false;
        }
        else
        {
            Vector3[] pos = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                pos[i] = transform.GetChild(i).position;
            }
            line.positionCount = transform.childCount;
            line.SetPositions(pos);
        }
    }
}
