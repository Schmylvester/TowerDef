using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceAttacks : MonoBehaviour
{
    [SerializeField] float balance_rate;
    [SerializeField] Fighter[] fighters;


    public void balance(float score, int winner)
    {
        if (winner == -1)
        {
            fighters[0].balanceAttacks(score * balance_rate);
            fighters[1].balanceAttacks(score * balance_rate);
        }
        else
        {
            fighters[winner].balanceAttacks(-score * balance_rate);
            fighters[1 - winner].balanceAttacks(score * balance_rate);
        }
    }
}
