using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Transform canvas;
    [SerializeField] LineRenderer[] paths;
    [SerializeField] Resources resources;
    UnitType spawn;
    short targettedNode = 0;
    float[] cooldowns;

    #region UnitUI
    [SerializeField] Image sprite;
    [SerializeField] Text unitName;
    [SerializeField] Text health;
    [SerializeField] Text moveSpeed;
    [SerializeField] Text damage;
    [SerializeField] Text attackSpeed;
    [SerializeField] Text cost;
    [SerializeField] Transform darkness;
    #endregion

    private void Start()
    {
        cooldowns = new float[(int)UnitType.Count];
        switchTargettedNode(0);
        updatePanel();
    }

    public void update(float rate)
    {
        for (int i = 0; i < cooldowns.Length; i++)
            cooldowns[i] -= rate;
        if (cooldowns[(int)spawn] > 0)
        {
            darkness.localScale = new Vector3(1, cooldowns[(int)spawn] / UnitTypes.instance.getStats(spawn).cooldown, 1);
        }
        else
        {
            darkness.localScale = new Vector3(1, 0, 1);
        }
    }

    private void Update()
    {
        if (!ReplayGame.instance.isRunning())
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
                if (cooldowns[(int)spawn] <= 0)
                {
                    short cost = UnitTypes.instance.getStats(spawn).cost;
                    if (resources.canAfford(cost))
                    {
                        Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
                        unit.setStartNode(paths[targettedNode].transform.GetChild(0).GetComponent<Node>(), targettedNode);
                        unit.setClass(spawn);
                        ShowHealth healthBar = Instantiate(healthBarPrefab, canvas).GetComponent<ShowHealth>();
                        unit.setHealthBar(healthBar);
                        unit.GetComponent<SpriteRenderer>().sprite = UnitTypes.instance.getSprite(spawn);
                        cooldowns[(int)spawn] = unit.getCooldown();
                        GameStateRecorder.instance.unitAdded(unit);
                        resources.updateGold((short)-cost);
                    }
                    else
                    {
                        FeedbackManager.instance.setFeedback(true, "You can't afford that.", Color.red);
                    }
                }
                else
                {
                    FeedbackManager.instance.setFeedback(true, spawn.ToString() + " is cooling down.", Color.red);
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
        attackSpeed.text = "Attack Speed: " + stats.attackRate;
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