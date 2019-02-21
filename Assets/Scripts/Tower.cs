using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] string towerID;
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
            if (dist > range * 0.7f)
            {
                ready = true;
                attachedToMouse = false;
            }
            else
            {
                FeedbackManager.instance.setFeedback(false, "This tower is too close to another tower.", Color.red);
                resources.updateGold(cost);
                EntityTracker.instance.removeTower(this);
                Destroy(gameObject);
            }
        }
    }

    private void OnMouseEnter()
    {
        UpdateTowerPanel.instance.updatePanel(sprite.sprite, towerID, range, rateOfFire, damage, cost);
    }

    private void Update()
    {
        if (ready)
        {
            timer += Time.deltaTime;
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
        else if (attachedToMouse)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y);
            circle.drawCircle(range, transform.position, 18);
        }
    }

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, range);
    }
#endif

}
