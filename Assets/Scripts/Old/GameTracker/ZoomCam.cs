using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCam : MonoBehaviour
{
    Camera activeCam = null;
    Camera[] allCams;
    Rect camDefRect;

    private void Awake()
    {
        allCams = FindObjectsOfType<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            setSingleCam();
        else if (Input.GetKeyDown(KeyCode.K))
            setManyCam();
    }

    void switchCamMode()
    {
        if(activeCam)
        {
            setManyCam();
        }
        else
        {
            setSingleCam();
        }
    }

    void setSingleCam()
    {
        if (activeCam)
            setManyCam();
        int newCam = Random.Range(0, 100);
        //int breakerCount = 10000;
        //while(allCams[newCam].GetComponentInParent<GameStateRecorder>().getGameEnded() && breakerCount-- > 0)
        //{
        //    newCam++;
        //    newCam %= 100;
        //}
        //if (breakerCount <= 0)
        //{
        //    setManyCam();
        //    return;
        //}
        activeCam = allCams[newCam];
        camDefRect = activeCam.rect;
        activeCam.rect = new Rect(Vector2.zero, Vector2.one);
        foreach (Camera cam in allCams)
        {
            if (cam != activeCam)
                cam.enabled = false;
        }
        activeCam.enabled = true;
    }

    void setManyCam()
    {
        if (!activeCam)
            return;
        activeCam.rect = camDefRect;
        activeCam = null;
        foreach (Camera cam in allCams)
        {
            cam.enabled = true;
        }
    }
}
