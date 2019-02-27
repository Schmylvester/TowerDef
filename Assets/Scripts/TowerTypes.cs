using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTypes : MonoBehaviour
{
    public static TowerTypes instance;
    [SerializeField] Tower[] towers;

    private void Awake()
    {
        instance = this;
    }
    public Tower getTower(int i)
    {
        return towers[i];
    }
}
