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
    [SerializeField] float moveSpeed;
    [SerializeField] short damage;
    [SerializeField] float attackRate;
    Vector3 dirLastFrame;
    float timer = 0;

    protected override void Start()
    {
        base.Start();
        if (target)
            setNewTarget(target);
        EntityTrack.instance.addUnit(this);
    }

    public void setStartNode(Node set)
    {
        target = set;
        setNewTarget(set);
    }

    public void setStats(float _moveSpeed, short _damage, float _attackRate, short _health)
    {
        moveSpeed = _moveSpeed;
        damage = _damage;
        attackRate = _attackRate;
        health = _health;
        currentHealth = _health;
    }

    public void setHealthBar(ShowHealth hBar)
    {
        healthBar = hBar;
        healthBar.updatePos(this, sprite.sprite);
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
        if ((dir.x * dirLastFrame.x >= 0 && dir.y * dirLastFrame.y >= 0) && dir != Vector3.zero)
        {
            transform.position += dir * Time.deltaTime * moveSpeed;
        }
        else
        {
            nextTargetInSequence();
        }
        dirLastFrame = dir;
        healthBar.updatePos(this, sprite.sprite);
    }

    void nextTargetInSequence()
    {
        if (target.next_node.Length == 0)
        {
            Debug.Log("Reached end of path, I imagine this means a win for the attacker");
            target = null;
            return;
        }
        else
        {
            setNewTarget(target.next_node[Random.Range(0, target.next_node.Length)]);
        }
    }

    private void attack()
    {
        if (target.health.destroyed)
            nextTargetInSequence();

        timer += Time.deltaTime;
        if (timer > attackRate)
        {
            timer -= attackRate;
            if (target.health.takeDamage(damage, gameObject, Color.red))
            {
                nextTargetInSequence();
            }
        }
    }

    void setNewTarget(Node node)
    {
        if (state == UnitState.Moving)
            transform.position = target.transform.position;
        if (node.health)
            if (!node.health.destroyed)
                state = UnitState.Attacking;
            else
                state = UnitState.Moving;
        else
            state = UnitState.Moving;
        target = node;
        dirLastFrame = Vector3.zero;
    }

    protected override void beDestroyed()
    {
        EntityTrack.instance.removeUnit(this);
        Destroy(gameObject);
    }
}
