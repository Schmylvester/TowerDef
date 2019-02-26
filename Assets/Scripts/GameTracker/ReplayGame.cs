using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReplayGame : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text replayText;
    [SerializeField] GameObject[] towers;
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Transform canvas;
    [SerializeField] LineRenderer[] paths;
    [SerializeField] Grid grid;
    [SerializeField] Resources[] resources;
    public static ReplayGame instance;
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

    public void replayFrame(uint _frame)
    {
        foreach (IODSetup d in defends)
        {
            if (d.frame == _frame)
            {
                short towerIdx = -1;
                for (short i = 0; i < d.output.type.Length; i++)
                {
                    if (d.output.type[i] == 1)
                    {
                        towerIdx = i;
                        break;
                    }
                }

                Vector3 pos = grid.CellToWorld(new Vector3Int((int)d.output.x, (int)d.output.y, 0));
                GameObject tower = Instantiate(towers[towerIdx], pos, new Quaternion());
                Tower t = tower.GetComponent<Tower>();t.setReady();
                resources[1].updateGold((short)-t.getCost());
            }
        }
        foreach (IOASetup a in attacks)
        {
            if (a.frame == _frame)
            {
                UnitType spawn = UnitType.Count;
                for(short i = 0; i < (short)UnitType.Count; i++)
                {
                    if(a.output.type[i] == 1)
                    {
                        spawn = (UnitType)i;
                    }
                }
                short path = -1;
                for (short i = 0; i < a.output.track.Length; i++)
                {
                    if (a.output.track[i] == 1)
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
            }
        }
    }

    public bool isRunning()
    {
        return replay > 0;
    }
}
