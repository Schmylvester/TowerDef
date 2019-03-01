using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Knight,
    Rogue,
    Pirate,
    Paladin,

    Count
}
public struct UnitStats
{
    public short health;
    public float moveSpeed;
    public short damage;
    public float attackRate;
    public short reward;
    public short cost;
}

public class UnitTypes : MonoBehaviour
{
    public static UnitTypes instance;

    [SerializeField] Sprite[] sprites;
    UnitStats[] stats;

    void Awake()
    {
        instance = this;
        stats = new UnitStats[(int)UnitType.Count];
        setStats(UnitType.Knight,  45, 0.8f, 15, 1.5f, 10, 15);
        setStats(UnitType.Rogue,   5,  1.8f, 1,  0.7f, 3,  5);
        setStats(UnitType.Pirate,  15, 1.2f, 5,  0.8f, 6,  8);
        setStats(UnitType.Paladin, 60, 0.8f, 5,  1.0f, 15, 15);
    }

    public UnitStats getStats(UnitType type)
    {
        return stats[(int)type];
    }

    public Sprite getSprite(UnitType type)
    {
        return sprites[(int)type];
    }

    void setStats(UnitType type, short health,
        float moveSpeed, short damage, float attackRate,
        short reward, short cost)
    {
        int i = (int)type;
        stats[i].health = health;
        stats[i].moveSpeed = moveSpeed;
        stats[i].damage = damage;
        stats[i].attackRate = attackRate;
        stats[i].reward = reward;
        stats[i].cost = cost;
    }
}
