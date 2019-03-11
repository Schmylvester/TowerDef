using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTypes : MonoBehaviour
{
    [SerializeField] Tower[] towers;

    public Tower getTower(int i)
    {
        return towers[i];
    }
}
