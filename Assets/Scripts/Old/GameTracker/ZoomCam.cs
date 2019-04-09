using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCam : MonoBehaviour
{
    Camera activeCam = null;
    Camera[] allCams;
    Rect camDefRect;
    int camIdx = 0;

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

        if (Input.GetMouseButtonDown(0) && activeCam == null)
        {
            foreach (Camera cam in allCams)
            {
                Vector2 pos = (cam.ScreenToViewportPoint(Input.mousePosition));
                if(pos.x >= 0 && pos.x < 1 && pos.y >= 0 && pos.y < 1)
                {
                    setSingleCam(cam);
                }
            }
        }
    }

    void switchCamMode()
    {
        if (activeCam)
        {
            setManyCam();
        }
        else
        {
            setSingleCam();
        }
    }

    void setSingleCam(Camera targetCam = null)
    {
        if (activeCam)
            setManyCam();
        if (targetCam == null)
        {
            int newCam = camIdx++;
            camIdx %= allCams.Length;
            activeCam = allCams[newCam];
        }
        else
        {
            activeCam = targetCam;
        }
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
