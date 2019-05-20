using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameStateRecorder : MonoBehaviour
{
    [SerializeField] Grid grid = null;
    [SerializeField] Structure[] walls = null;
    [SerializeField] Transform[] tracks = null;
    Attacks attacks;
    Defends defends;
    [SerializeField] bool clear_files = false;
    [SerializeField] short pathIn = 0;
    [SerializeField] short pathOut;
    [SerializeField] GameManager m = null;
    [SerializeField] ResetGame reset = null;
    [SerializeField] Camera m_camera = null;
    [SerializeField] bool manyGames = false;
    bool gameEnded = false;

    /// <summary>
    /// Returns the grid position normalised between 0 and 1
    /// </summary>
    /// <param name="inPos">The position of an entity on a grid</param>
    /// <returns>The relative position of the entity between the top and bottom of the screen</returns>
    Vector2 normalisePos(Vector2Int inPos)
    {
        float x = ((float)inPos.x / 360) + 0.5f;
        float y = ((float)inPos.y / 210) + 0.5f;
        return new Vector2(x, y);
    }

    private void Awake()
    {
        string fileOut = "";
        foreach (char c in gameObject.name)
        {
            if (char.IsDigit(c))
                fileOut += c;
        }
        pathOut = short.Parse(fileOut);
        if (manyGames)
        {
            m_camera.rect = new Rect(0.1f * (pathOut / 10), 0.1f * (pathOut % 10), 0.1f, 0.1f);
            transform.position = new Vector3(40 * pathOut, 0, 0);
        }
        m.gsr = this;
        if (clear_files)
        {
            clearFiles();
        }
        if (pathIn != -1)
        {
            getFileData();
        }
        else
        {
            attacks = new Attacks() { attacks = new List<IOASetup>() };
            defends = new Defends() { defends = new List<IODSetup>() };
        }
    }

    /// <summary>
    /// Gets the data for the game from a json file
    /// </summary>
    void getFileData()
    {
        StreamReader file = new StreamReader("Assets/kNNData/Attacks/" + pathIn + ".json");
        attacks = JsonUtility.FromJson<Attacks>(file.ReadToEnd());
        file.Close();

        file = new StreamReader("Assets/kNNData/Defends/" + pathIn + ".json");
        defends = JsonUtility.FromJson<Defends>(file.ReadToEnd());
        file.Close();
    }

    /// <summary>
    /// Adds 1 to the name of the files to output to
    /// </summary>
    public void incrementOutFile()
    {
        if (pathOut != -1)
        {
            pathOut++;
            pathOut %= 100;
        }
    }

    public short getGameID()
    {
        return pathOut;
    }

    /// <summary>
    /// clears the in files and quits
    /// </summary>
    void clearFiles()
    {
        StreamWriter file = new StreamWriter("Assets/kNNData//Defends/" + pathIn + ".json", false);
        file.Write(JsonUtility.ToJson(new Defends()
        {
            defends = new List<IODSetup>()
        }));
        file.Close();
        file = new StreamWriter("Assets/kNNData/Attacks/" + pathIn + ".json", false);
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

    /// <summary>
    /// tracks all entities to get the current state of the game
    /// </summary>
    /// <returns>struct containing all pertinent data</returns>
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
        foreach (Unit unit in m.tracker.getUnits())
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
        foreach (Tower tower in m.tracker.getTowers())
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
        input.attackerResources = m.tracker.getResources(true).normalisedValue();
        input.defenderResources = m.tracker.getResources(false).normalisedValue();

        return input;
    }

    /// <summary>
    /// converts tower entity to entity data struct
    /// </summary>
    /// <param name="addedTower">Which tower has been added</param>
    /// <returns>struct of entity data</returns>
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

    /// <summary>
    /// converts unit entity to the unit data struct
    /// </summary>
    /// <param name="addedUnit">Which unit has been added</param>
    /// <returns>struct of unit data</returns>
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

    /// <summary>
    /// when a unit is added by their data,
    /// this is called to add the game 
    /// state and the new unit to the list
    /// of game data
    /// </summary>
    /// <param name="unit">the data of the unit added</param>
    public void unitAdded(UnitData unit)
    {
        if (pathOut == -1)
        {
            return;
        }
        InputRecord _in = getGameState();
        IOASetup data = new IOASetup()
        {
            frame = m.frames.frame,
            input = _in,
            output = unit
        };
        attacks.attacks.Add(data);
    }

    /// <summary>
    /// when a unit is added, this is called to
    /// add the game state and the new unit to
    /// the list of game data
    /// </summary>
    /// <param name="unit">the unit added</param>
    public void unitAdded(Unit unit)
    {
        if (pathOut == -1)
        {
            return;
        }
        InputRecord _in = getGameState();
        UnitData _out = createOutput(unit);
        IOASetup data = new IOASetup()
        {
            frame = m.frames.frame,
            input = _in,
            output = _out
        };
        attacks.attacks.Add(data);
    }

    /// <summary>
    /// when a tower is added, this is called to
    /// add the game state and the new tower to
    /// the list of game data
    /// </summary>
    /// <param name="tower">the tower added</param>
    public void towerAdded(Tower tower)
    {
        if (pathOut == -1)
        {
            return;
        }
        InputRecord _in = getGameState();
        EntityData _out = createOutput(tower);
        IODSetup data = new IODSetup()
        {
            frame = m.frames.frame,
            input = _in,
            output = _out
        };
        defends.defends.Add(data);
    }

    /// <summary>
    /// when a tower is added by its data,
    /// this is called to add the game state
    /// and the new tower to the list of
    /// game data
    /// </summary>
    /// <param name="tower">data of the new tower</param>
    public void towerAdded(EntityData tower)
    {
        if (pathOut == -1)
        {
            return;
        }
        InputRecord _in = getGameState();
        IODSetup data = new IODSetup()
        {
            frame = m.frames.frame,
            input = _in,
            output = tower
        };
        defends.defends.Add(data);
    }

    /// <summary>
    /// when the game ends, this is called
    /// to add the game data to the output
    /// files
    /// </summary>
    /// <param name="defenderWins">True if defender won, False if attacker won</param>
    public void onGameOver(bool defenderWins)
    {
        if (gameEnded)
            return;
        setGameEnded(true);
        if (pathOut != -1)
        {
            float aScore = m.scorer.getAttScore(!defenderWins);
            for (int i = 0; i < attacks.attacks.Count; i++)
            {
                attacks.score = aScore;
            }
            packIntoFile(attacks, pathOut);
            attacks.attacks.Clear();
        }
        float dScore = m.scorer.getDefScore(defenderWins);
        if (pathOut != -1)
        {
            for (int i = 0; i < defends.defends.Count; i++)
            {
                defends.score = dScore;
            }
            packIntoFile(defends, pathOut);
            defends.defends.Clear();
        }
        if (!defenderWins)
        {
            Color endColour = Color.Lerp(Color.red, Color.white, dScore);
            m_camera.backgroundColor = endColour;
        }
        reset.endGame(dScore, pathOut);
    }

    public void changeColours()
    {
        m_camera.backgroundColor = Color.Lerp(m_camera.backgroundColor, Color.green, 0.2f);
    }

    /// <summary>
    /// packs the defend data into the json file
    /// </summary>
    /// <param name="def">the data to save</param>
    /// <param name="fileName">the file to save to</param>
    public void packIntoFile(Defends def, short fileName)
    {
        StreamWriter file = new StreamWriter("Assets/kNNData/Defends/" + fileName + ".json", false);
        file.Write(JsonUtility.ToJson(def));
        file.Close();
    }

    /// <summary>
    /// packs the attack data into the json file
    /// </summary>
    /// <param name="att">the data to save</param>
    /// <param name="fileName">the file to save to</param>
    public void packIntoFile(Attacks att, short fileName)
    {
        StreamWriter file = new StreamWriter("Assets/kNNData/Attacks/" + fileName + ".json", false);
        file.Write(JsonUtility.ToJson(att));
        file.Close();
    }

    /// <summary>
    /// gets the attack and defend events of a game
    /// </summary>
    /// <param name="thisGame">which game</param>
    /// <param name="def">what the defenders did and when</param>
    /// <param name="att">what the attackers did and when</param>
    public void getEvents(ref List<IODSetup> def, ref List<IOASetup> att)
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

    public bool getGameEnded()
    {
        return gameEnded;
    }
    public void setGameEnded(bool to)
    {
        gameEnded = to;
    }
}