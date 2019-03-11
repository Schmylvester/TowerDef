using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSet : ManualUpdate
{
    float lifeTime = 0.2f;
    PlayFrames frames;
    
    public void init(PlayFrames _frames)
    {
        frames = _frames;
        frames.addItem(this);
    }

    public override void update(float rate)
    {
        lifeTime -= rate;
        if (lifeTime < 0)
        {
            frames.removeItem(this);
            Destroy(gameObject);
        }
    }
}
