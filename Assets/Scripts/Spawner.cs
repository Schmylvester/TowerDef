using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Transform canvas;
    [SerializeField] Node startNode;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
            unit.setStartNode(startNode);
            ShowHealth healthBar = Instantiate(healthBarPrefab, canvas).GetComponent<ShowHealth>();
            unit.setHealthBar(healthBar);
        }
    }
}