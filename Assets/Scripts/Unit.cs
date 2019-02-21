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
    UnitState state = UnitState.Moving;
    [SerializeField] Node target;
    UnitStats stats;
    float timer = 0;
    Resources resources;

    protected override void Start()
    {
        base.Start();
        if (target)
            setNewTarget(target);
        EntityTracker.instance.addUnit(this);
    }

    public void setStartNode(Node set)
    {
        target = set;
        setNewTarget(set);
    }

    public void setHealthBar(ShowHealth hBar)
    {
        healthBar = hBar;
        healthBar.updatePos(this, sprite.sprite);
    }

    public void setClass(UnitType type)
    {
        stats = UnitTypes.instance.getStats(type);
        health = stats.health;
        currentHealth = stats.health;
    }

    public void setResources(Resources _resources)
    {
        resources = _resources;
    }

    private void Update()
    {
        if (target)
        {
            switch (state)
            {
                case UnitState.Moving:
                    move();
                    break;
                case UnitState.Attacking:
                    attack();
                    break;
            }
        }
    }

    void move()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        if (Vector2.Distance(transform.position, target.transform.position) > 0.1f)
        {
            transform.position += dir * Time.deltaTime * stats.moveSpeed;
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
            target = null;
            beDestroyed();
            return;
        }
        else
        {
            setNewTarget(target.next_node);
        }
    }

    private void attack()
    {
        if (target.structure.destroyed)
            nextTargetInSequence();

        timer += Time.deltaTime;
        if (timer > stats.attackRate)
        {
            timer -= stats.attackRate;
            if (target.structure.takeDamage(stats.damage, gameObject, Color.red))
            {
                resources.updateGold(target.structure.getReward());
                nextTargetInSequence();
            }
        }
    }

    void setNewTarget(Node node)
    {
        if (state == UnitState.Moving)
            transform.position = target.transform.position;
        if (node.structure)
            if (!node.structure.destroyed)
                state = UnitState.Attacking;
            else
                state = UnitState.Moving;
        else
            state = UnitState.Moving;
        target = node;
    }

    public short getReward()
    {
        return stats.reward;
    }

    protected override void beDestroyed()
    {
        Destroy(healthBar.gameObject);
        EntityTracker.instance.removeUnit(this);
        Destroy(gameObject);
    }

    public float getCooldown()
    {
        return stats.cooldown;
    }
}
