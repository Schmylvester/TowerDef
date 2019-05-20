using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : ManualUpdate
{
    [SerializeField] TowerType type = TowerType.Count;
    [SerializeField] SpriteRenderer sprite = null;
    short usesLeft;
    [SerializeField] DrawCircle circle = null;
    float timer = 0;
    List<Unit> units = new List<Unit>();
    Resources resources;
    bool ready;
    bool attachedToMouse = false;
    Vector2 startPos;
    [SerializeField] GameManager m = null;
    [SerializeField] UpdateTowerPanel tower_panel = null;
    TowerStats stats;

    private void Start()
    {
        stats = GetComponentInParent<TowerTypes>().getTowerStats(type);
        timer = stats.rateFire;
        startPos = new Vector2(transform.position.x, transform.position.y);
        m.frames.addItem(this);
        m.tracker.addTower(this);
        usesLeft = stats.uses;
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
                if (resources.canAfford(stats.cost))
                {
                    attachedToMouse = true;
                    Instantiate(this, startPos, new Quaternion(), transform.parent);
                    resources.updateGold((short)-stats.cost);
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
                    resources.updateGold(stats.cost);
                    m.gsr.towerAdded(this);
                    resources.updateGold((short)(stats.cost * -1));
                }
                else
                {
                    FeedbackManager.instance.setFeedback(false, "This tower is too close to another tower.", Color.red);
                    resources.updateGold(stats.cost);
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
            tower_panel.updatePanel(sprite.sprite, type.ToString(), stats);
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
                circle.drawCircle(stats.range, transform.position, 18);
            }
        }
    }

    public override void update(float rate)
    {
        if (ready)
        {
            timer += rate;
            if (timer > stats.rateFire)
            {
                foreach (Unit unit in units)
                {
                    float dist = (transform.position - unit.transform.position).magnitude;
                    if (dist < stats.range)
                    {
                        if (unit.takeDamage(stats.damage, gameObject, Color.green))
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
                            sprite.color = new Color(1, 1, 1, (float)usesLeft / stats.uses + 0.3f);
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
        if(stats.cost == 0)
        {
            stats = GetComponentInParent<TowerTypes>().getTowerStats(type);
        }
        return stats.cost;
    }

    public float getHealth()
    {
        return (float)usesLeft / stats.uses;
    }

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, stats.range);
    }
#endif

}
