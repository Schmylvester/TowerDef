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

public class Tower : MonoBehaviour
{
    [SerializeField] TowerType type;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] short cost;
    [SerializeField] float range;
    [SerializeField] float rateOfFire;
    [SerializeField] short damage;
    [SerializeField] DrawCircle circle;
    float timer = 0;
    List<Unit> units = new List<Unit>();
    Resources resources;
    bool ready = false;
    bool attachedToMouse = false;
    Vector2 startPos;

    private void Start()
    {
        timer = rateOfFire;
        startPos = new Vector2(transform.position.x, transform.position.y);
        PlayFrames.instance.addItem(this);
        EntityTracker.instance.addTower(this);
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
    private void OnMouseUp()
    {
        if (attachedToMouse)
        {
            float dist = EntityTracker.instance.getClosestTower(this);
            if (dist > 1.5f)
            {
                ready = true;
                attachedToMouse = false;
                GSRecorder.GameStateRecorder.instance.towerAdded(this);
            }
            else
            {
                FeedbackManager.instance.setFeedback(false, "This tower is too close to another tower.", Color.red);
                resources.updateGold(cost);
                EntityTracker.instance.removeTower(this);
                PlayFrames.instance.removeItem(this);
                Destroy(gameObject);
            }
        }
    }

    private void OnMouseEnter()
    {
        UpdateTowerPanel.instance.updatePanel(sprite.sprite, type.ToString(), range, rateOfFire, damage, cost);
    }

    private void Update()
    {
        if (attachedToMouse)
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(p.x, p.y);
            circle.drawCircle(range, transform.position, 18);
        }
    }

    public void update(float rate)
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

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, range);
    }
#endif

}
