using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGame : MonoBehaviour
{
    [SerializeField] BalanceAttacks balance;
    [SerializeField] Fighter[] fighters;
    [SerializeField] Camera cam;
    int turn = 0;
    Fighter activePlayer;

    private void Start()
    {
        activePlayer = fighters[turn];
    }

    public bool isActivePlayer(Fighter fighter)
    {
        return activePlayer == fighter;
    }

    public void advanceTurn()
    {
        for (int i = 0; i < fighters.Length; i++)
        {
            if (fighters[i].getHealth() <= 0)
            {
                float score = fighters[1 - i].getScore();
                fighters[1 - i].win();
                fighters[i].endGame(-score);
                fighters[1 - i].endGame(score);
                if (i == 0)
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.red, 0.01f);
                else
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.green, 0.01f);

                //balance.balance(score, 1 - i);
                reset();
                return;
            }
        }
        turn++;
        turn %= fighters.Length;
        activePlayer = fighters[turn];
    }

    private void reset()
    {
        fighters[0].reset();
        fighters[1].reset();
        advanceTurn();
    }
}
