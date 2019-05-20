using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignAttacks : MonoBehaviour
{
    [SerializeField] Fighter fighter = null;
    [SerializeField] GameObject specialAttackPrefab = null;

    private void Start()
    {
        GameObject o = Instantiate(specialAttackPrefab, transform);
        int r = Random.Range(0, 5);
        switch (r)
        {
            case 0:
                o.AddComponent<Drain>();
                break;
            case 1:
                o.AddComponent<Poison>();
                break;
            case 2:
                o.AddComponent<HalfAttack>();
                break;
            case 3:
                o.AddComponent<Recoil>();
                break;
            case 4:
                o.AddComponent<Risk>();
                break;
        }
        fighter.start();
    }
}
