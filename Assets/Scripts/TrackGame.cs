using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGame : MonoBehaviour
{
    [SerializeField] BalanceAttacks balance;
    [SerializeField] Fighter[] fighters;
    [SerializeField] Camera cam;
    [SerializeField] Vector2Int gameConstraints;
    int turn = 0;
    Fighter activePlayer;
    int turns = 0;
    int[] wins = new int[2] { 0, 0 };
    List<bool> gameResults = new List<bool>();

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
        ++turns;
        for (int i = 0; i < fighters.Length; i++)
        {
            if (fighters[i].getHealth() <= 0)
            {
                //record the win
                ++wins[1 - i];
                gameResults.Add(i == 1);
                //get the game's score
                float score = fighters[1 - i].getScore();
                //tell the players the game is over
                fighters[i].endGame(-score);    //this player lost, so they get the negative score
                fighters[1 - i].endGame(score); //this player won, so they get the positive score

                //change the camera colour slightly to help visualise winning streaks
                if (i == 0)
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.blue, 0.01f);
                else
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, new Color(1.0f, 0.7f, 0.0f), 0.01f);

                //balance the players, if the winning player had a lot of health left, balance more
                balance.balance(score, 1 - i);
                //start again
                gameEnd();
                return;
            }
        }

        //both players still alive, next turn
        turn++;
        turn %= fighters.Length;
        activePlayer = fighters[turn];
    }

    private void gameEnd()
    {
        string w = " Wins: " + wins[0] + " : " + wins[1];
        //check out what's up every 100 games
        if (gameResults.Count % 1000 == 0)
        {
            Debug.Break();
        }
        getWinRatio(50);
        //Debug.Log("Turns: " + turns + w);

        //game was too long, make both players stronger
        if (turns > gameConstraints[1])
        {
            balance.balance(1, -1);
        }
        //game was too short, make both players weaker
        else if(turns < gameConstraints[0])
        {
            balance.balance(-1, -1);
        }

        //start again
        turns = 0;
        fighters[0].reset();
        fighters[1].reset();
        advanceTurn();
    }

    void getWinRatio(int numMatches)
    {
        //not enough matches yet
        if (numMatches > gameResults.Count)
        {
            return;
        }

        int p1Wins = 0;
        for (int i = gameResults.Count - numMatches; i < gameResults.Count; i++)
        {
            if (gameResults[i])
                p1Wins++;
        }

        Debug.Log("Win rate of the last " + numMatches + " matches: " + p1Wins + " : " + (numMatches - p1Wins));
    }
}
