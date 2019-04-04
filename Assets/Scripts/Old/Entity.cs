using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Entity : ManualUpdate
{
    [SerializeField] protected short health;
    [SerializeField] protected ShowHealth healthBar;
    [SerializeField] protected SpriteRenderer sprite;
    protected short currentHealth;
    [SerializeField] protected GameManager m;

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

    public void initEntity(GameManager _m)
    {
        m = _m;
    }

    void makeLine(GameObject attacker, Color color)
    {
        LineRenderer lRend = new GameObject().AddComponent<LineRenderer>();
        LineSet line = lRend.gameObject.AddComponent<LineSet>();
        line.init(m.frames);
        lRend.SetPositions(new Vector3[] { attacker.transform.position, transform.position });
        lRend.startColor = new Color(color.r / 2, color.g / 2, color.b / 2);
        lRend.endColor = color;
        lRend.startWidth = 0.1f;
        lRend.endWidth = 0.1f;
    }

    public bool takeDamage(short dam, GameObject attacker, Color color)
    {
        if(SceneManager.GetActiveScene().name != "100Games")
            makeLine(attacker, color);
        currentHealth -= dam;
        currentHealth = (short)Mathf.Max(currentHealth, 0);
        if(healthBar)
            healthBar.updateHealth(this);
        if (currentHealth <= 0)
        {
            beDestroyed();
            return true;
        }
        return false;
    }
}
