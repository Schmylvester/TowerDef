using System.Collections.Generic;

[System.Serializable]
public struct EntityData
{
    public float x;
    public float y;
    public float health;
    public short[] type;
}

[System.Serializable]
public struct InputRecord
{
    public List<EntityData> towers;
    public List<EntityData> units;
    public List<EntityData> walls;
    public float attackerResources;
    public float defenderResources;
}

[System.Serializable]
public struct UnitData
{
    public short[] type;
    public short[] track;
}

[System.Serializable]
public struct IOASetup
{
    public uint frame;
    public InputRecord input;
    public UnitData output;
}

[System.Serializable]
public struct IODSetup
{
    public uint frame;
    public InputRecord input;
    public EntityData output;
}

[System.Serializable]
public struct Attacks
{
    public float score;
    public List<IOASetup> attacks;
}

[System.Serializable]
public struct Defends
{
    public float score;
    public List<IODSetup> defends;
}