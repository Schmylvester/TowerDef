using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum UnitState
{
    Moving,
    Attacking,
}

public class Unit : Entity
{
    UnitType type;
    UnitState state = UnitState.Moving;
    [SerializeField] Node target;
    UnitStats stats;
    float timer = 0;
    Resources resources;
    float timeToTarget;
    short track;

    protected override void Start()
    {
        base.Start();
        if (target)
            setNewTarget(target);
        EntityTracker.instance.addUnit(this);
        PlayFrames.instance.addItem(this);
    }

    public void setStartNode(Node set, short trackID)
    {
        track = trackID;
        target = set;
        setNewTarget(set);
    }

    public void setHealthBar(ShowHealth hBar)
    {
        healthBar = hBar;
        healthBar.updatePos(this, sprite.sprite);
    }

    public void setClass(UnitType _type)
    {
        type = _type;
        stats = UnitTypes.instance.getStats(type);
        health = stats.health;
        currentHealth = stats.health;
    }

    public void setResources(Resources _resources)
    {
        resources = _resources;
    }

    public void update(float rate)
    {
        if (target)
        {
            switch (state)
            {
                case UnitState.Moving:
                    move(rate);
                    break;
                case UnitState.Attacking:
                    attack(rate);
                    break;
            }
        }
    }

    void move(float rate)
    {
        timer += rate;
        Vector3 dir = (target.transform.position - transform.position).normalized;
        if (timer < timeToTarget)
        {
            transform.position += dir * rate * stats.moveSpeed;
        }
        else
        {
            nextTargetInSequence();
        }
        healthBar.updatePos(this, sprite.sprite);
    }

    void nextTargetInSequence()
    {
        if (target.next_node == null)
        {
            FeedbackManager.instance.setFeedback(true, "You win.");
            FeedbackManager.instance.setFeedback(false, "You lose.");
            GameStateRecorder.instance.onGameOver(false);
            target = null;
            beDestroyed();
            return;
        }
        else
        {
            setNewTarget(target.next_node);
        }
    }

    private void attack(float rate)
    {
        if (target.structure.getDestroyed())
            nextTargetInSequence();

        timer += rate;
        if (timer > stats.attackRate)
        {
            timer -= stats.attackRate;
            if (target)
            {
                if (target.structure.takeDamage(stats.damage, gameObject, Color.red))
                {
                    resources.updateGold(target.structure.getReward());
                    nextTargetInSequence();
                }
            }
        }
    }

    void setNewTarget(Node node)
    {
        timer = 0;
        if (state == UnitState.Moving)
            transform.position = target.transform.position;
        if (node.structure)
            if (!node.structure.getDestroyed())
                state = UnitState.Attacking;
            else
                state = UnitState.Moving;
        else
            state = UnitState.Moving;
        target = node;
        if (state == UnitState.Moving)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            timeToTarget = distance / stats.moveSpeed;
        }
    }

    public short getReward()
    {
        return stats.reward;
    }

    public UnitType getType()
    {
        return type;
    }

    protected override void beDestroyed()
    {
        Destroy(healthBar.gameObject);
        EntityTracker.instance.removeUnit(this);
        PlayFrames.instance.removeItem(this);
        Destroy(gameObject);
    }

    public float getCooldown()
    {
        return stats.cooldown;
    }

    public short getTrack()
    {
        return track;
    }
}
