using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Knight,
    Rogue,
    Pirate,

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
    public float cooldown;
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
        setStats(UnitType.Knight, 50, 0.8f, 15, 1.5f, 10, 15, 7.0f);
        setStats(UnitType.Rogue, 5, 2.0f, 1, 0.5f, 3, 5, 0.8f);
        setStats(UnitType.Pirate, 15, 1.5f, 2, 0.7f, 6, 8, 1.0f);
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
        short reward, short cost, float cooldown)
    {
        int i = (int)type;
        stats[i].health = health;
        stats[i].moveSpeed = moveSpeed;
        stats[i].damage = damage;
        stats[i].attackRate = attackRate;
        stats[i].reward = reward;
        stats[i].cost = cost;
        stats[i].cooldown = cooldown;
    }

    public short cheapestUnit()
    {
        short lowest = short.MaxValue;
        foreach(UnitStats stat in stats)
        {
            if(stat.cost < lowest)
            {
                lowest = stat.cost;
            }
        }
        return lowest;
    }
}
