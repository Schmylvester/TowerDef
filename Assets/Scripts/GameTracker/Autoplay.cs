using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Autoplay : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text replayText;
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Transform canvas;
    [SerializeField] LineRenderer[] paths;
    [SerializeField] Grid grid;
    [SerializeField] Resources[] resources;
    public static Autoplay instance;
    [SerializeField] uint replay;
    List<IOASetup> attacks;
    List<IODSetup> defends;

    private void Start()
    {
        instance = this;
        if (replay > 0)
        {
            defends = new List<IODSetup>();
            attacks = new List<IOASetup>();
            GameStateRecorder.instance.getEvents(replay, ref defends, ref attacks);
            replayText.text = "Replaying Game: " + replay;
        }
    }

    private void Update()
    {
        if (Random.Range(0, 400) == 0)
        {
            createRandom();
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
        return replay > 0;
    }

    void createRandom()
    {
        UnitData newUnit = new UnitData()
        {
            track = new short[3],
            type = new short[4],
        };
        newUnit.type[Random.Range(0, newUnit.type.Length)] = 1;
        newUnit.track[Random.Range(0, newUnit.track.Length)] = 1;
        createUnit(newUnit);
    }

    public void createTower(EntityData from)
    {
        Vector3Int iPos = unnormalisePos(new Vector3(from.x, from.y, 0));
        Vector3 pos = grid.CellToWorld(iPos);
        if (EntityTracker.instance.isValidInput(from.type, pos))
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

            GameObject tower = Instantiate(TowerTypes.instance.getTower(towerIdx).gameObject, pos, new Quaternion());
            Tower t = TowerTypes.instance.getTower(towerIdx);
            t.setReady();
            resources[1].updateGold((short)-t.getCost());
        }
    }

    public void createUnit(UnitData from)
    {
        if (EntityTracker.instance.isValidInput(from))
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
            Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
            unit.setStartNode(paths[path].transform.GetChild(0).GetComponent<Node>(), path);
            unit.setClass(spawn);
            ShowHealth healthBar = Instantiate(healthBarPrefab, canvas).GetComponent<ShowHealth>();
            unit.setHealthBar(healthBar);
            unit.GetComponent<SpriteRenderer>().sprite = UnitTypes.instance.getSprite(spawn);
            resources[0].updateGold((short)-UnitTypes.instance.getStats(spawn).cost);
            Spawner.instance.setInCooldown(spawn);
        }
    }
}
