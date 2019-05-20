using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Entity
{
    [SerializeField] short reward = 0;
    bool destroyed = false;
    protected override void beDestroyed()
    {
        sprite.color = Color.Lerp(Color.clear, sprite.color, 0.5f);
        destroyed = true;
    }

    public short getReward()
    {
        return reward;
    }

    public bool getDestroyed()
    {
        return destroyed;
    }

    public override void update(float rate)
    {

    }

    public void resetGame()
    {
        currentHealth = health;
        destroyed = false;
        healthBar.updateHealth(this);
        sprite.color = Color.white;
    }
}
