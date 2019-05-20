using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTracker : MonoBehaviour
{
    List<Tower> towers = new List<Tower>();
    List<Unit> units = new List<Unit>();
    public Material lineMat;
    [SerializeField] Resources attackerRes = null;
    [SerializeField] Resources defenderRes = null;
    [SerializeField] GameManager m = null;
    [SerializeField] TowerTypes towerTypes = null;

    private void Awake()
    {
        m.tracker = this;
    }


    public List<Unit> getUnits()
    {
        return units;
    }

    public List<Tower> getTowers()
    {
        return towers;
    }

    public Resources getResources(bool att)
    {
        if (att)
            return attackerRes;
        else
            return defenderRes;
    }

    public void addUnit(Unit unit)
    {
        units.Add(unit);
        unit.setResources(attackerRes);
        foreach (Tower tower in towers)
        {
            tower.addUnit(unit);
        }
    }

    public float getClosestTower(Tower to)
    {
        float closest = float.MaxValue;
        foreach (Tower tower in towers)
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

    public float getClosestTower(Vector2 to)
    {
        float closest = float.MaxValue;
        foreach (Tower tower in towers)
        {
            float dist = Vector2.Distance(to, tower.transform.position);
            if (dist < closest)
            {
                closest = dist;
            }
        }
        return closest;
    }

    public void removeUnit(Unit unit)
    {
        units.Remove(unit);
        foreach (Tower tower in towers)
        {
            tower.removeUnit(unit);
        }
    }

    public void addTower(Tower tower)
    {
        towers.Add(tower);
        foreach (Unit unit in units)
        {
            tower.addUnit(unit);
        }
        tower.setResources(defenderRes);
    }

    public void removeTower(Tower tower)
    {
        towers.Remove(tower);
    }

    public bool isValidInput(UnitData newUnit)
    {
        UnitType spawn = UnitType.Count;
        for (short i = 0; i < (short)UnitType.Count; i++)
        {
            if (newUnit.type[i] == 1)
            {
                spawn = (UnitType)i;
            }
        }
        short cost = UnitTypes.instance.getStats(spawn).cost;
        if (!attackerRes.canAfford(cost))
            return false;
        return true;
    }
    public bool isValidInput(short[] newTower, Vector3 pos)
    {
        short towerIdx = 0;
        for (short i = 0; i < newTower.Length; i++)
        {
            if (newTower[i] == 1)
            {
                towerIdx = i;
                break;
            }
        }
        if (!defenderRes.canAfford(towerTypes.getTowerStats((TowerType)towerIdx).cost))
            return false;
        float dist = getClosestTower(new Vector2(pos.x, pos.y));
        if (dist <= 1.5f)
            return false;
        return true;
    }

    public void resetGame()
    {
        towers.Clear();
        units.Clear();
    }
}