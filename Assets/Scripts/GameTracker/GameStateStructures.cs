using System.Collections.Generic;

[System.Serializable]
public struct EntityData
{
    public int x;
    public int y;
    public float health;
    public short[] type;
}

[System.Serializable]
public struct InputRecord
{
    public List<EntityData> towers;
    public List<EntityData> units;
    public List<EntityData> walls;
    public int attackerResources;
    public int defenderResources;
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
    public bool didWin;
    public uint gameID;
    public uint frame;
    public InputRecord input;
    public UnitData output;
}

[System.Serializable]
public struct IODSetup
{
    public bool didWin;
    public uint gameID;
    public uint frame;
    public InputRecord input;
    public EntityData output;
}

[System.Serializable]
public struct Attacks
{
    public List<IOASetup> attacks;
}

[System.Serializable]
public struct Defends
{
    public List<IODSetup> defends;
}