using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignAttacks : MonoBehaviour
{
    [SerializeField] Fighter fighter;
    [SerializeField] GameObject specialAttackPrefab;

    private void Start()
    {
        GameObject o = Instantiate(specialAttackPrefab, transform);
        switch (Random.Range(0, 4))
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
        }
        fighter.start();
    }
}
