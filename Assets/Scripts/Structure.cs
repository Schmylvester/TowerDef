using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Entity
{
    [SerializeField] short reward;
    public bool destroyed = false;
    protected override void beDestroyed()
    {
        sprite.color = Color.Lerp(Color.clear, sprite.color, 0.5f);
        destroyed = true;
    }

    public short getReward()
    {
        return reward;
    }
}
