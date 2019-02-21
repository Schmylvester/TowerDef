using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTracker : MonoBehaviour
{
    public static EntityTracker instance;
    List<Tower> towers = new List<Tower>();
    List<Unit> units = new List<Unit>();
    public Material lineMat;
    [SerializeField] Resources attackerRes;
    [SerializeField] Resources defenderRes;

    private void Awake()
    {
        instance = this;
    }

    public void addUnit(Unit unit)
    {
        units.Add(unit);
        unit.setResources(attackerRes);
        foreach(Tower tower in towers)
        {
            tower.addUnit(unit);
        }
    }

    public float getClosestTower(Tower to)
    {
        float closest = float.MaxValue;
        foreach(Tower tower in towers)
        {
            if (tower != to)
            {
                float dist = Vector2.Distance(to.transform.position, tower.transform.position);
                if (dist < closest)
                {
                    closest = dist;
                }
            }
        }
        return closest;
    }

    public void removeUnit(Unit unit)
    {
        units.Remove(unit);
        foreach(Tower tower in towers)
        {
            tower.removeUnit(unit);
        }
        if(units.Count == 0 && !attackerRes.canAfford(UnitTypes.instance.cheapestUnit()))
        {
            Debug.Log("I think this means the towers win.");
        }
    }

    public void addTower(Tower tower)
    {
        towers.Add(tower);
        foreach(Unit unit in units)
        {
            tower.addUnit(unit);
        }
        tower.setResources(defenderRes);
    }

    public void removeTower(Tower tower)
    {
        towers.Remove(tower);
    }
}
