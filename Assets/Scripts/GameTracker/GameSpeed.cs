using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeed : MonoBehaviour
{
    [SerializeField] int framesPerFrame;
    [SerializeField] float frameSpeed;

    public int getFrameCount()
    {
        return framesPerFrame;
    }

    public float getFrameSpeed()
    {
        return frameSpeed;
    }

    public float getSpeed()
    {
        return framesPerFrame * frameSpeed;
    }
}
