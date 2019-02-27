using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSet : ManualUpdate
{
    float lifeTime = 0.2f;

    private void Start()
    {
        PlayFrames.instance.addItem(this);
    }

    public override void update(float rate)
    {
        lifeTime -= rate;
        if (lifeTime < 0)
        {
            PlayFrames.instance.removeItem(this);
            Destroy(gameObject);
        }
    }
}
