using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Archer,
    Bard,
    Necromancer,

    Count
}

public class Tower : ManualUpdate
{
    [SerializeField] TowerType type;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] short cost;
    [SerializeField] short uses;
    short usesLeft;
    [SerializeField] float range;
    [SerializeField] float rateOfFire;
    [SerializeField] short damage;
    [SerializeField] DrawCircle circle;
    float timer = 0;
    List<Unit> units = new List<Unit>();
    Resources resources;
    bool ready;
    bool attachedToMouse = false;
    Vector2 startPos;
    [SerializeField] GameManager m;
    [SerializeField] UpdateTowerPanel tower_panel;

    private void Start()
    {
        timer = rateOfFire;
        startPos = new Vector2(transform.position.x, transform.position.y);
        m.frames.addItem(this);
        m.tracker.addTower(this);
        usesLeft = uses;
    }

    public void addUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void removeUnit(Unit unit)
    {
        units.Remove(unit);
    }

    public void setResources(Resources _resources)
    {
        resources = _resources;
    }

    private void OnMouseDown()
    {
        if (!m.autoplay.replayRunning())
        {
            if (!ready && !attachedToMouse)
            {
                if (resources.canAfford(cost))
                {
                    attachedToMouse = true;
                    Instantiate(this, startPos, new Quaternion());
                    resources.updateGold((short)-cost);
                }
                else
                {
                    FeedbackManager.instance.setFeedback(false, "You can't afford that.", Color.red);
                }
            }
        }
    }
    private void OnMouseUp()
    {
        if (!m.autoplay.replayRunning())
        {
            if (attachedToMouse)
            {
                float dist = m.tracker.getClosestTower(this);
                if (dist > 1.5f)
                {
                    ready = true;
                    attachedToMouse = false;
                    resources.updateGold(cost);
                    m.gsr.towerAdded(this);
                    resources.updateGold((short)(cost * -1));
                }
                else
                {
                    FeedbackManager.instance.setFeedback(false, "This tower is too close to another tower.", Color.red);
                    resources.updateGold(cost);
                    m.tracker.removeTower(this);
                    m.frames.removeItem(this);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!m.autoplay.replayRunning())
        {
            tower_panel.updatePanel(sprite.sprite, type.ToString(), range, rateOfFire, damage, uses, cost);
        }
    }

    private void Update()
    {
        if (!m.autoplay.replayRunning())
        {
            if (attachedToMouse)
            {
                Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(p.x, p.y);
                circle.drawCircle(range, transform.position, 18);
            }
        }
    }

    public override void update(float rate)
    {
        if (ready)
        {
            timer += rate;
            if (timer > rateOfFire)
            {
                foreach (Unit unit in units)
                {
                    float dist = (transform.position - unit.transform.position).magnitude;
                    if (dist < range)
                    {
                        if (unit.takeDamage(damage, gameObject, Color.green))
                        {
                            resources.updateGold(unit.getReward());
                        }
                        usesLeft--;
                        if (usesLeft <= 0)
                        {
                            m.tracker.removeTower(this);
                            Destroy(gameObject);
                        }
                        else
                        {
                            sprite.color = new Color(1, 1, 1, (float)usesLeft / uses + 0.3f);
                        }
                        timer = 0;
                        break;
                    }
                }
            }
        }
    }

    public bool isReady()
    {
        return ready;
    }

    public TowerType getType()
    {
        return type;
    }

    public void setReady()
    {
        ready = true;
    }

    public short getCost()
    {
        return cost;
    }

    public float getHealth()
    {
        return (float)usesLeft / uses;
    }

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, range);
    }
#endif

}
