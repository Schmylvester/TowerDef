using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceAttacks : MonoBehaviour
{
    //the rate of change between each fight for the attacks
    [SerializeField] float balance_rate = 0.03f;
    //when this is higher than 1 the game favours player0, lower than 1 favours player1
    [SerializeField] float skew = 1.0f;
    [SerializeField] Fighter[] fighters;


    public void balance(float score, int winner)
    {
        switch (winner)
        {
            //not balancing because a fight was won, don't balance health
            case -1:
                fighters[0].balance(score * balance_rate, false);
                fighters[1].balance(score * balance_rate, false);
                break;
            //player0 won the fight
            //balance less if skew is high, more if skew is low
            case 0:
                fighters[0].balance(-score * (1 / skew) * balance_rate);
                fighters[1].balance(score * (1 / skew) * balance_rate);
                break;
            //player1 won the fight
            //balance more if skew is low, less if skew is high
            case 1:
                fighters[0].balance(score * skew * balance_rate);
                fighters[1].balance(-score * skew * balance_rate);
                break;
        }
    }
}
