using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject unitPrefab = null;
    [SerializeField] GameObject healthBarPrefab = null;
    [SerializeField] Transform canvas = null;
    [SerializeField] LineRenderer[] paths = null;
    [SerializeField] Resources resources = null;
    UnitType spawn;
    short targettedNode = 0;
    [SerializeField] GameManager m = null;

    #region UnitUI
    [SerializeField] Image sprite = null;
    [SerializeField] Text unitName = null;
    [SerializeField] Text health = null;
    [SerializeField] Text moveSpeed = null;
    [SerializeField] Text damage = null;
    [SerializeField] Text attackSpeed = null;
    [SerializeField] Text cost = null;
    #endregion

    private void Start()
    {
        switchTargettedNode(0);
        updatePanel();
    }

    private void Update()
    {
        if (!m.autoplay.replayRunning())
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                switchUnitType(1);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                switchUnitType(-1);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                switchTargettedNode(-1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                switchTargettedNode(1);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

                short cost = UnitTypes.instance.getStats(spawn).cost;
                if (resources.canAfford(cost))
                {
                    Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
                    unit.initEntity(m);
                    unit.initUnit();
                    unit.setStartNode(paths[targettedNode].transform.GetChild(0).GetComponent<Node>(), targettedNode);
                    unit.setClass(spawn);
                    ShowHealth healthBar = Instantiate(healthBarPrefab, canvas).GetComponent<ShowHealth>();
                    unit.setHealthBar(healthBar);
                    unit.GetComponent<SpriteRenderer>().sprite = UnitTypes.instance.getSprite(spawn);
                    m.gsr.unitAdded(unit);
                    resources.updateGold((short)-cost);
                }
                else
                {
                    FeedbackManager.instance.setFeedback(true, "You can't afford that.", Color.red);
                }
            }
        }
    }

    void switchUnitType(short dir)
    {
        short sp = (short)spawn;
        sp += dir;
        if (sp < 0)
        {
            sp = (short)(UnitType.Count - 1);
        }
        if (sp == (short)(UnitType.Count))
        {
            sp = 0;
        }
        spawn = (UnitType)sp;
        updatePanel();
    }

    void updatePanel()
    {
        UnitTypes units = UnitTypes.instance;
        sprite.sprite = units.getSprite(spawn);
        unitName.text = spawn.ToString();
        UnitStats stats = units.getStats(spawn);
        health.text = "Health: " + stats.health;
        moveSpeed.text = "Move Speed: " + stats.moveSpeed;
        damage.text = "Damage: " + stats.damage;
        attackSpeed.text = "Attack Speed: " + (1.0f / stats.attackRate).ToString("F1");
        cost.text = "Cost: " + stats.cost;
    }

    void switchTargettedNode(short dir)
    {
        paths[targettedNode].startColor = Color.white;
        targettedNode += dir;
        targettedNode = (short)Mathf.Max(0, targettedNode);
        targettedNode = (short)Mathf.Min(targettedNode, paths.Length - 1);
        paths[targettedNode].startColor = Color.blue;
    }
}