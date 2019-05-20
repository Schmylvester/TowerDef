using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeed : MonoBehaviour
{
    public static GameSpeed instance;
    [SerializeField] int framesPerFrame = 1;
    [SerializeField] float frameSpeed = 1;

    private void Awake()
    {
        instance = this;
    }

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
