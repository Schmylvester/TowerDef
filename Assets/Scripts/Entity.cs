using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : ManualUpdate
{
    [SerializeField] protected short health;
    [SerializeField] protected ShowHealth healthBar;
    [SerializeField] protected SpriteRenderer sprite;
    protected short currentHealth;

    protected abstract void beDestroyed();

    protected virtual void Start()
    {
        currentHealth = health;
        if (healthBar)
        {
            healthBar.updatePos(this, sprite.sprite);
            healthBar.updateHealth(this);
        }
    }

    public float getHealth()
    {
        return (float)currentHealth / health;
    }

    public string getHealthText()
    {
        return currentHealth + "/" + health;
    }

    void makeLine(GameObject attacker, Color color)
    {
        LineRenderer lRend = new GameObject().AddComponent<LineRenderer>();
        lRend.gameObject.AddComponent<LineSet>();
        lRend.SetPositions(new Vector3[] { attacker.transform.position, transform.position });
        lRend.startColor = new Color(color.r / 2, color.g / 2, color.b / 2);
        lRend.endColor = color;
        lRend.material = EntityTracker.instance.lineMat;
        lRend.startWidth = 0.1f;
        lRend.endWidth = 0.1f;
    }

    public bool takeDamage(short dam, GameObject attacker, Color color)
    {
        makeLine(attacker, color);
        currentHealth -= dam;
        currentHealth = (short)Mathf.Max(currentHealth, 0);

        healthBar.updateHealth(this);
        if (currentHealth <= 0)
        {
            beDestroyed();
            return true;
        }
        return false;
    }
}
