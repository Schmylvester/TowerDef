using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Entity
{
    public bool destroyed = false;
    protected override void beDestroyed()
    {
        sprite.enabled = false;
        destroyed = true;
    }
}
