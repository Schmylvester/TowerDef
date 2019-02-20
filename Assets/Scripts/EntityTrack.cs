using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTrack : MonoBehaviour
{
    public static EntityTrack instance;
    List<Tower> towers = new List<Tower>();
    List<Unit> units = new List<Unit>();
    public Material lineMat;

    private void Awake()
    {
        instance = this;
    }

    public void addUnit(Unit unit)
    {
        units.Add(unit);
        foreach(Tower tower in towers)
        {
            tower.addUnit(unit);
        }
    }

    public void removeUnit(Unit unit)
    {
        units.Remove(unit);
        foreach(Tower tower in towers)
        {
            tower.removeUnit(unit);
        }
    }

    public void addTower(Tower tower)
    {
        towers.Add(tower);
        foreach(Unit unit in units)
        {
            tower.addUnit(unit);
        }
    }
}
