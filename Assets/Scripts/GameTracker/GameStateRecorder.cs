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
    [SerializeField] bool clear_files;
    [SerializeField] string attackPathIn;
    [SerializeField] string attackPathOut;
    [SerializeField] string defendPathIn;
    [SerializeField] string defendPathOut;

    Vector2 normalisePos(Vector2Int inPos)
    {
        float x = ((float)inPos.x / 360) + 0.5f;
        float y = ((float)inPos.y / 210) + 0.5f;
        return new Vector2(x, y);
    }

    private void Awake()
    {
        instance = this;
        if (clear_files)
        {
            clearFiles();
        }
        if (attackPathIn != "" && defendPathIn != "")
        {
            getFileData();
        }
        else
        {
            attacks = new Attacks() { attacks = new List<IOASetup>() };
            defends = new Defends() { defends = new List<IODSetup>() };
        }
    }

    void getFileData()
    {
        StreamReader file = new StreamReader("Assets/kNNData/Attacks/" + attackPathIn + ".json");
        attacks = JsonUtility.FromJson<Attacks>(file.ReadToEnd());
        file.Close();

        file = new StreamReader("Assets/kNNData/Defends/" + defendPathIn + ".json");
        defends = JsonUtility.FromJson<Defends>(file.ReadToEnd());
        file.Close();
    }

    public void incrementOutFile()
    {
        if (attackPathOut != "")
        {
            uint fileID = uint.Parse(attackPathOut);
            fileID++;
            attackPathOut = fileID.ToString();
            defendPathOut = fileID.ToString();
        }
    }

    void clearFiles()
    {
        StreamWriter file = new StreamWriter("Assets/kNNData//Defends/" + defendPathIn + ".json", false);
        file.Write(JsonUtility.ToJson(new Defends()
        {
            defends = new List<IODSetup>()
        }));
        file.Close();
        file = new StreamWriter("Assets/kNNData/Attacks/" + attackPathIn + ".json", false);
        file.Write(JsonUtility.ToJson(new Attacks()
        {
            attacks = new List<IOASetup>()
        }));
        file.Close();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public InputRecord getGameState()
    {
        InputRecord input = new InputRecord()
        {
            walls = new List<EntityData>(),
            units = new List<EntityData>(),
            towers = new List<EntityData>(),
        };
        foreach (Structure wall in walls)
        {
            Vector2Int iPos = getGridPos(wall.transform.position);
            Vector2 pos = normalisePos(iPos);
            EntityData wallData = new EntityData()
            {
                x = pos.x,
                y = pos.y,
                type = new short[0],
                health = wall.getHealth()
            };
            input.walls.Add(wallData);
        }
        EntityTracker tracker = EntityTracker.instance;
        foreach (Unit unit in tracker.getUnits())
        {
            Vector2Int iPos = getGridPos(unit.transform.position);
            Vector2 pos = normalisePos(iPos);
            EntityData unitData = new EntityData()
            {
                x = pos.x,
                y = pos.y,
                health = unit.getHealth(),
                type = new short[(int)UnitType.Count]
            };
            unitData.type[(int)unit.getType()] = 1;
            input.units.Add(unitData);
        }
        foreach (Tower tower in tracker.getTowers())
        {
            if (tower.isReady())
            {
                Vector2Int iPos = getGridPos(tower.transform.position);
                Vector2 pos = normalisePos(iPos);
                EntityData towerData = new EntityData()
                {
                    x = pos.x,
                    y = pos.y,
                    health = tower.getHealth(),
                    type = new short[(int)TowerType.Count]
                };
                towerData.type[(int)tower.getType()] = 1;
                input.towers.Add(towerData);
            }
        }
        input.attackerResources = tracker.getResources(true).normalisedValue();
        input.defenderResources = tracker.getResources(false).normalisedValue();

        return input;
    }

    EntityData createOutput(Tower addedTower)
    {
        Vector2Int iPos = getGridPos(addedTower.transform.position);
        Vector2 pos = normalisePos(iPos);
        EntityData towerData = new EntityData()
        {
            x = pos.x,
            y = pos.y,
            health = 1,
            type = new short[(int)TowerType.Count]
        };
        towerData.type[(int)addedTower.getType()] = 1;
        return towerData;
    }

    UnitData createOutput(Unit addedUnit)
    {
        UnitData newUnit = new UnitData()
        {
            type = new short[(int)UnitType.Count],
            track = new short[tracks.Length]
        };
        newUnit.type[(int)addedUnit.getType()] = 1;
        newUnit.track[addedUnit.getTrack()] = 1;
        return newUnit;
    }

    Vector2Int getGridPos(Vector3 pos)
    {
        return new Vector2Int(grid.WorldToCell(pos).x, grid.WorldToCell(pos).y);
    }

    public void unitAdded(UnitData unit)
    {
        if (attackPathOut == "")
        {
            return;
        }
        InputRecord _in = getGameState();
        IOASetup data = new IOASetup()
        {
            frame = PlayFrames.instance.frame,
            input = _in,
            output = unit
        };
        attacks.attacks.Add(data);
    }

    public void unitAdded(Unit unit)
    {
        if (attackPathOut == "")
        {
            return;
        }
        InputRecord _in = getGameState();
        UnitData _out = createOutput(unit);
        IOASetup data = new IOASetup()
        {
            frame = PlayFrames.instance.frame,
            input = _in,
            output = _out
        };
        attacks.attacks.Add(data);
    }

    public void towerAdded(Tower tower)
    {
        if (defendPathOut == "")
        {
            return;
        }
        InputRecord _in = getGameState();
        EntityData _out = createOutput(tower);
        IODSetup data = new IODSetup()
        {
            frame = PlayFrames.instance.frame,
            input = _in,
            output = _out
        };
        defends.defends.Add(data);
    }

    public void towerAdded(EntityData tower)
    {
        if (defendPathOut == "")
        {
            return;
        }
        InputRecord _in = getGameState();
        IODSetup data = new IODSetup()
        {
            frame = PlayFrames.instance.frame,
            input = _in,
            output = tower
        };
        defends.defends.Add(data);
    }

    public void onGameOver(bool defenderWins)
    {
        PlayFrames.instance.gameOver = true;
        if (attackPathOut != "")
        {
            float aScore = GameScorer.instance.getAttScore(!defenderWins);
            for (int i = 0; i < attacks.attacks.Count; i++)
            {
                attacks.attacks[i] = setScore(attacks.attacks[i], aScore);
            }
            packIntoFile(attacks, attackPathOut);
        }
        if (defendPathOut != "")
        {
            float dScore = GameScorer.instance.getDefScore(defenderWins);
            for (int i = 0; i < defends.defends.Count; i++)
            {
                defends.defends[i] = setScore(defends.defends[i], dScore);
            }
            packIntoFile(defends, defendPathOut);
        }
    }

    public void packIntoFile(Defends def, string fileName)
    {
        StreamWriter file = new StreamWriter("Assets/kNNData/Defends/" + fileName + ".json", false);
        file.Write(JsonUtility.ToJson(def));
        file.Close();
    }

    public void packIntoFile(Attacks att, string fileName)
    {
        StreamWriter file = new StreamWriter("Assets/kNNData/Attacks/" + fileName + ".json", false);
        file.Write(JsonUtility.ToJson(att));
        file.Close();
    }

    public void getEvents(uint thisGame, ref List<IODSetup> def, ref List<IOASetup> att)
    {
        foreach (IODSetup d in defends.defends)
        {
            def.Add(d);
        }
        foreach (IOASetup a in attacks.attacks)
        {
            att.Add(a);
        }
    }

    public List<IODSetup> getDefenceData()
    {
        return defends.defends;
    }

    public List<IOASetup> getAttackData()
    {
        return attacks.attacks;
    }

    IOASetup setScore(IOASetup _in, float score)
    {
        return new IOASetup()
        {
            score = score,
            frame = _in.frame,
            input = _in.input,
            output = _in.output
        };
    }

    IODSetup setScore(IODSetup _in, float score)
    {
        return new IODSetup()
        {
            score = score,
            frame = _in.frame,
            input = _in.input,
            output = _in.output
        };
    }
}