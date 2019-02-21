using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Transform canvas;
    [SerializeField] LineRenderer[] paths;
    [SerializeField] Resources resources;
    int targettedNode = 0;
    float[] cooldowns;

    private void Start()
    {
        cooldowns = new float[(int)UnitType.Count];
        switchTargettedNode(0);
    }

    private void Update()
    {
        for (int i = 0; i < cooldowns.Length; i++)
            cooldowns[i] -= Time.deltaTime;

        UnitType spawn = UnitType.Count;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            spawn = UnitType.Knight;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            spawn = UnitType.Rogue;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            spawn = UnitType.Pirate;
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            switchTargettedNode(-1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            switchTargettedNode(1);
        }

        if (spawn != UnitType.Count && cooldowns[(int)spawn] <= 0)
        {
            short cost = UnitTypes.instance.getStats(spawn).cost;
            if (resources.canAfford(cost))
            {
                resources.updateGold((short)-cost);
                Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
                unit.setStartNode(paths[targettedNode].transform.GetChild(0).GetComponent<Node>());
                unit.setClass(spawn);
                ShowHealth healthBar = Instantiate(healthBarPrefab, canvas).GetComponent<ShowHealth>();
                unit.setHealthBar(healthBar);
                unit.GetComponent<SpriteRenderer>().sprite = UnitTypes.instance.getSprite(spawn);
                cooldowns[(int)spawn] = unit.getCooldown();
            }
        }
    }

    void switchTargettedNode(short dir)
    {
        paths[targettedNode].startColor = Color.white;
        targettedNode += dir;
        targettedNode = Mathf.Max(0, targettedNode);
        targettedNode = Mathf.Min(targettedNode, paths.Length - 1);
        paths[targettedNode].startColor = Color.blue;
    }
}