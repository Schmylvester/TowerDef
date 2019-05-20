using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Autoplay : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text replayText = null;
    [SerializeField] GameObject unitPrefab = null;
    [SerializeField] LineRenderer[] paths = null;
    [SerializeField] Grid grid = null;
    [SerializeField] Resources[] resources = null;
    List<IOASetup> attacks;
    List<IODSetup> defends;
    [SerializeField] bool replay = false;
    [SerializeField] GameManager m = null;
    [SerializeField] TowerTypes towerTypes = null;

    private void Awake()
    {
        m.autoplay = this;
    }

    private void Start()
    {
        if (replay)
        {
            defends = new List<IODSetup>();
            attacks = new List<IOASetup>();
            m.gsr.getEvents(ref defends, ref attacks);
            replayText.text = "Replaying Game: " + replay;
        }
    }

    Vector3Int unnormalisePos(Vector3 inPos)
    {
        int x = (int)((inPos.x - 0.5f) * 360);
        int y = (int)((inPos.y - 0.5f) * 210);
        return new Vector3Int(x, y, (int)inPos.z);
    }

    public void replayFrame(uint _frame)
    {
        foreach (IODSetup d in defends)
        {
            if (d.frame == _frame)
            {
                createTower(d.output);
            }
        }
        foreach (IOASetup a in attacks)
        {
            if (a.frame == _frame)
            {
                createUnit(a.output);
            }
        }
    }

    public bool replayRunning()
    {
        return replay;
    }

    UnitData randomUnit()
    {
        UnitData newUnit = new UnitData()
        {
            track = new short[3],
            type = new short[4],
        };
        
        //newUnit.type[Random.Range(0, newUnit.type.Length)] = 1;
        //newUnit.track[Random.Range(0, newUnit.track.Length)] = 1;

        newUnit.type[1] = 1;
        newUnit.track[1] = 1;

        return newUnit;
    }

    EntityData randomTower()
    {
        EntityData newTower = new EntityData()
        {
            type = new short[3],
            health = 1,
            x = Random.Range(0.0f, 1.0f),
            y = Random.Range(0.0f, 1.0f)
        };

        newTower.type[Random.Range(0, newTower.type.Length)]= 1;

        return newTower;
    }

    public void createTower(EntityData from, bool rand = false)
    {
        if (rand)
        {
            from = randomTower();
            Vector3Int intPos = unnormalisePos(new Vector3(from.x, from.y, 0));
            Vector3 fltPos = grid.CellToWorld(intPos);
            if (m.tracker.isValidInput(from.type, fltPos))
            {
                m.gsr.towerAdded(from);
            }
        }
        Vector3Int iPos = unnormalisePos(new Vector3(from.x, from.y, 0));
        Vector3 pos = grid.CellToWorld(iPos);
        if (m.tracker.isValidInput(from.type, pos))
        {
            short towerIdx = -1;
            for (short i = 0; i < from.type.Length; i++)
            {
                if (from.type[i] == 1)
                {
                    towerIdx = i;
                    break;
                }
            }

            Tower tower = Instantiate(
                towerTypes.getTower(towerIdx).gameObject, pos
                , new Quaternion(), transform).GetComponent<Tower>();
            tower.setReady();
            resources[1].updateGold((short)-tower.getCost());
        }
    }

    public void createUnit(UnitData from, bool rand = false)
    {
        if (rand)
        {
            from = randomUnit();
            m.gsr.unitAdded(from);
        }
        if (m.tracker.isValidInput(from))
        {
            UnitType spawn = UnitType.Count;
            for (short i = 0; i < (short)UnitType.Count; i++)
            {
                if (from.type[i] == 1)
                {
                    spawn = (UnitType)i;
                }
            }

            short path = -1;
            for (short i = 0; i < from.track.Length; i++)
            {
                if (from.track[i] == 1)
                {
                    path = i;
                    break;
                }
            }
            Unit unit = Instantiate(unitPrefab, transform).GetComponent<Unit>();
            unit.initEntity(m);
            unit.initUnit();

            unit.setStartNode(paths[path].transform.GetChild(0).GetComponent<Node>(), path);
            unit.setClass(spawn);
//            ShowHealth healthBar = Instantiate(healthBarPrefab, canvas).GetComponent<ShowHealth>();
//            unit.setHealthBar(healthBar);
            unit.GetComponent<SpriteRenderer>().sprite = UnitTypes.instance.getSprite(spawn);
            resources[0].updateGold((short)-UnitTypes.instance.getStats(spawn).cost);
        }
    }
}
