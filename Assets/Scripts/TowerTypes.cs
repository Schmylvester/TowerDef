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

[System.Serializable]
public struct TowerStats
{
    public short cost;
    public short uses;
    public float range;
    public float rateFire;
    public short damage;
}

public class TowerTypes : MonoBehaviour
{
    [SerializeField] Tower[] towers;

    TowerStats[] stats;

    private void Awake()
    {
        stats = new TowerStats[(int)TowerType.Count];

        stats[(int)TowerType.Archer].cost = 15;
        stats[(int)TowerType.Archer].damage = 2;
        stats[(int)TowerType.Archer].range = 1.4f;
        stats[(int)TowerType.Archer].rateFire = 0.6f;
        stats[(int)TowerType.Archer].uses = 20;

        stats[(int)TowerType.Bard].cost = 20;
        stats[(int)TowerType.Bard].damage = 1;
        stats[(int)TowerType.Bard].range = 1.5f;
        stats[(int)TowerType.Bard].rateFire = 0.2f;
        stats[(int)TowerType.Bard].uses = 30;

        stats[(int)TowerType.Necromancer].cost = 30;
        stats[(int)TowerType.Necromancer].damage = 40;
        stats[(int)TowerType.Necromancer].range = 3.0f;
        stats[(int)TowerType.Necromancer].rateFire = 20.0f;
        stats[(int)TowerType.Necromancer].uses = 5;
    }

    public TowerStats getTowerStats(TowerType type)
    {
        return stats[(int)type];
    }

    public Tower getTower(int i)
    {
        return towers[i];
    }
}
