using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GSRecorder
{
    #region SoManyStructs
    [System.Serializable]
    public struct EntityData
    {
        public float x;
        public float y;
        public float health;
        public short[] type;
    }

    [System.Serializable]
    public struct Input
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
    public struct AttackerOutput
    {
        public UnitData newUnit;
    }

    [System.Serializable]
    public struct DefenderOutput
    {
        public EntityData newTower;
    }

    [System.Serializable]
    public struct IOASetup
    {
        public uint frame;
        public Input input;
        public AttackerOutput output;
    }

    [System.Serializable]
    public struct IODSetup
    {
        public uint frame;
        public Input input;
        public DefenderOutput output;
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
    #endregion
    public class GameStateRecorder : MonoBehaviour
    {
        public static GameStateRecorder instance;
        [SerializeField] Grid grid;
        [SerializeField] Structure[] walls;
        [SerializeField] Transform[] tracks;
        Attacks attacks;
        Defends defends;

        private void Awake()
        {
            instance = this;
            StreamReader file = new StreamReader("Assets/kNNData/Att.json");
            attacks = JsonUtility.FromJson<Attacks>(file.ReadToEnd());
            file.Close();
            file = new StreamReader("Assets/kNNData/Def.json");
            defends = JsonUtility.FromJson<Defends>(file.ReadToEnd());
            file.Close();
        }

        Input getGameState()
        {
            Input input = new Input()
            {
                walls = new List<EntityData>(),
                units = new List<EntityData>(),
                towers = new List<EntityData>(),
            };
            foreach (Structure wall in walls)
            {
                Vector2Int pos = getGridPos(wall.transform.position);
                Vector2 fPos = ((Vector2)pos / 50) + (Vector2.one / 2);
                EntityData wallData = new EntityData() { x = fPos.x, y = fPos.y, type = new short[0], health = wall.getHealth() };
                input.walls.Add(wallData);
            }
            EntityTracker tracker = EntityTracker.instance;
            foreach (Unit unit in tracker.getUnits())
            {
                Vector2Int pos = getGridPos(unit.transform.position);
                Vector2 fPos = ((Vector2)pos / 50) + (Vector2.one / 2);
                EntityData unitData = new EntityData() { x = fPos.x, y = fPos.y, health = unit.getHealth(), type = new short[(int)UnitType.Count] };
                unitData.type[(int)unit.getType()] = 1;
                input.units.Add(unitData);
            }
            foreach (Tower tower in tracker.getTowers())
            {
                if (tower.isReady())
                {
                    Vector2Int pos = getGridPos(tower.transform.position);
                    Vector2 fPos = ((Vector2)pos / 50) + (Vector2.one / 2);
                    EntityData towerData = new EntityData() { x = fPos.x, y = fPos.y, health = 1, type = new short[(int)TowerType.Count] };
                    towerData.type[(int)tower.getType()] = 1;
                    input.towers.Add(towerData);
                }
            }
            input.attackerResources = (float)tracker.getResources(true).getGold() / 150;
            input.defenderResources = (float)tracker.getResources(false).getGold() / 150;

            return input;
        }

        DefenderOutput createOutput(Tower addedTower)
        {
            Vector2Int pos = getGridPos(addedTower.transform.position);
            Vector2 fPos = ((Vector2)pos / 50) + (Vector2.one / 2);
            EntityData towerData = new EntityData() { x = fPos.x, y = fPos.y, health = 1, type = new short[(int)TowerType.Count] };
            towerData.type[(int)addedTower.getType()] = 1;
            DefenderOutput output = new DefenderOutput() { newTower = towerData };
            return output;
        }

        AttackerOutput createOutput(Unit addedUnit)
        {
            UnitData newUnit = new UnitData() { type = new short[(int)UnitType.Count], track = new short[tracks.Length] };
            newUnit.type[(int)addedUnit.getType()] = 1;
            newUnit.track[addedUnit.getTrack()] = 1;
            AttackerOutput output = new AttackerOutput() { newUnit = newUnit };
            return output;
        }

        Vector2Int getGridPos(Vector3 pos)
        {
            return new Vector2Int(grid.WorldToCell(pos).x, grid.WorldToCell(pos).y);
        }

        public void unitAdded(Unit unit)
        {
            Input _in = getGameState();
            AttackerOutput _out = createOutput(unit);
            IOASetup data = new IOASetup() { frame = PlayFrames.instance.frame, input = _in, output = _out };
            attacks.attacks.Add(data);
        }

        public void towerAdded(Tower tower)
        {
            Input _in = getGameState();
            DefenderOutput _out = createOutput(tower);
            IODSetup data = new IODSetup() { frame = PlayFrames.instance.frame, input = _in, output = _out };
            defends.defends.Add(data);
        }

        private void OnApplicationQuit()
        {
            StreamWriter file = new StreamWriter("Assets/kNNData/Att.json", false);
            file.Write(JsonUtility.ToJson(attacks));
            file.Close();
            file = new StreamWriter("Assets/kNNData/Def.json", false);
            file.Write(JsonUtility.ToJson(defends));
            file.Close();
        }
    }
}