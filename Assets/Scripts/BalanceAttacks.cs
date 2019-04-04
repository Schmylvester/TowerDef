using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceAttacks : MonoBehaviour
{
    [SerializeField] float balance_rate;
    [SerializeField] Fighter[] fighters;
    

    public void balance(float score, int winner)
    {
        fighters[winner].balanceAttacks(-score * balance_rate);
        fighters[1 - winner].balanceAttacks(score * balance_rate);
    }
}
