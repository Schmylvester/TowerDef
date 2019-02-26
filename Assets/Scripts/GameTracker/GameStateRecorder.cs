using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameStateRecorder : MonoBehaviour
{
    public static GameStateRecorder instance;
    [SerializeField] Grid grid;
    [SerializeField] Structure[] walls;
    [SerializeField] Transform[] tracks;
    Attacks attacks;
    Defends defends;
    uint gameID;
    [SerializeField] bool clear_files;

    private void Awake()
    {
        instance = this;
        if (clear_files)
        {
            clearFiles();
        }
        StreamReader file = new StreamReader("Assets/kNNData/Att.json");
        attacks = JsonUtility.FromJson<Attacks>(file.ReadToEnd());
        file.Close();

        file = new StreamReader("Assets/kNNData/Def.json");
        defends = JsonUtility.FromJson<Defends>(file.ReadToEnd());
        file.Close();
        gameID = defends.defends[defends.defends.Count - 1].gameID + 1;
    }

    void clearFiles()
    {
        StreamWriter file = new StreamWriter("Assets/kNNData/Def.json", false);
        file.Write(JsonUtility.ToJson(new Defends() { defends = new List<IODSetup>() }));
        file.Close();
        file = new StreamWriter("Assets/kNNData/Att.json", false);
        file.Write(JsonUtility.ToJson(new Attacks() { attacks = new List<IOASetup>() }));
        file.Close();
        Application.Quit();
    }

    InputRecord getGameState()
    {
        InputRecord input = new InputRecord()
        {
            walls = new List<EntityData>(),
            units = new List<EntityData>(),
            towers = new List<EntityData>(),
        };
        foreach (Structure wall in walls)
        {
            Vector2Int pos = getGridPos(wall.transform.position);
            EntityData wallData = new EntityData() { x = pos.x, y = pos.y, type = new short[0], health = wall.getHealth() };
            input.walls.Add(wallData);
        }
        EntityTracker tracker = EntityTracker.instance;
        foreach (Unit unit in tracker.getUnits())
        {
            Vector2Int pos = getGridPos(unit.transform.position);
            EntityData unitData = new EntityData() { x = pos.x, y = pos.y, health = unit.getHealth(), type = new short[(int)UnitType.Count] };
            unitData.type[(int)unit.getType()] = 1;
            input.units.Add(unitData);
        }
        foreach (Tower tower in tracker.getTowers())
        {
            if (tower.isReady())
            {
                Vector2Int pos = getGridPos(tower.transform.position);
                EntityData towerData = new EntityData() { x = pos.x, y = pos.y, health = 1, type = new short[(int)TowerType.Count] };
                towerData.type[(int)tower.getType()] = 1;
                input.towers.Add(towerData);
            }
        }
        input.attackerResources = tracker.getResources(true).getGold();
        input.defenderResources = tracker.getResources(false).getGold();

        return input;
    }

    EntityData createOutput(Tower addedTower)
    {
        Vector2Int pos = getGridPos(addedTower.transform.position);
        EntityData towerData = new EntityData() { x = pos.x, y = pos.y, health = 1, type = new short[(int)TowerType.Count] };
        towerData.type[(int)addedTower.getType()] = 1;
        return towerData;
    }

    UnitData createOutput(Unit addedUnit)
    {
        UnitData newUnit = new UnitData() { type = new short[(int)UnitType.Count], track = new short[tracks.Length] };
        newUnit.type[(int)addedUnit.getType()] = 1;
        newUnit.track[addedUnit.getTrack()] = 1;
        return newUnit;
    }

    Vector2Int getGridPos(Vector3 pos)
    {
        return new Vector2Int(grid.WorldToCell(pos).x, grid.WorldToCell(pos).y);
    }

    public void unitAdded(Unit unit)
    {
        InputRecord _in = getGameState();
        UnitData _out = createOutput(unit);
        IOASetup data = new IOASetup() { gameID = gameID, frame = PlayFrames.instance.frame, input = _in, output = _out };
        attacks.attacks.Add(data);
    }

    public void towerAdded(Tower tower)
    {
        InputRecord _in = getGameState();
        EntityData _out = createOutput(tower);
        IODSetup data = new IODSetup() { gameID = gameID, frame = PlayFrames.instance.frame, input = _in, output = _out };
        defends.defends.Add(data);
    }

    public void onGameOver(bool defenderWins)
    {
        if (defenderWins)
        {
            for (int i = 0; i < defends.defends.Count; i++)
            {
                defends.defends[i] = new IODSetup()
                {
                    didWin = true,
                    frame = defends.defends[i].frame,
                    gameID = defends.defends[i].gameID,
                    input = defends.defends[i].input,
                    output = defends.defends[i].output
                };
            }
        }
        else
        {
            for (int i = 0; i < attacks.attacks.Count; i++)
            {
                attacks.attacks[i] = new IOASetup()
                {
                    didWin = true,
                    frame = attacks.attacks[i].frame,
                    gameID = attacks.attacks[i].gameID,
                    input = attacks.attacks[i].input,
                    output = attacks.attacks[i].output
                };
            }
        }
        StreamWriter file = new StreamWriter("Assets/kNNData/Def.json", false);
        file.Write(JsonUtility.ToJson(defends));
        file.Close();
        file = new StreamWriter("Assets/kNNData/Att.json", false);
        file.Write(JsonUtility.ToJson(attacks));
        file.Close();
    }

    public void getEvents(uint thisGame, ref List<IODSetup> def, ref List<IOASetup> att)
    {
        foreach (IODSetup d in defends.defends)
        {
            if (d.gameID == thisGame)
            {
                def.Add(d);
            }
        }
        foreach (IOASetup a in attacks.attacks)
        {
            if (a.gameID == thisGame)
            {
                att.Add(a);
            }
        }
    }
}