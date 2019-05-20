using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGame : MonoBehaviour
{
    [SerializeField] BalanceAttacks balance = null;
    [SerializeField] Fighter[] fighters = null;
    [SerializeField] Camera cam = null;

    //min and max length of a game in turns
    [SerializeField] Vector2Int gameConstraints;
    //the player whose turn it is
    int activePlayer = -1;
    //turns passed in this instance of the game
    int turns = 0;
    //results of all games so far
    List<bool> gameResults = new List<bool>();

    private void Update()
    {
        if (activePlayer == -1)
        {
            if (Input.GetKeyDown(KeyCode.B))
                activePlayer = 0;
        }
    }

    public bool isActivePlayer(Fighter fighter)
    {
        if (activePlayer < 0)
            return false;
        return fighters[activePlayer] == fighter;
    }

    public void advanceTurn()
    {
        ++turns;
        for (int i = 0; i < fighters.Length; i++)
        {
            if (fighters[i].getHealth() <= 0)
            {
                gameResults.Add(i == 1);
                //get the game's score
                float score = fighters[1 - i].getScore();
                //tell the players the game is over
                fighters[i].endGame(-score);    //this player lost, so they get the negative score
                fighters[1 - i].endGame(score); //this player won, so they get the positive score


                float winRatio = getWinRatio(150);
                //change the camera colour slightly to help visualise winning streaks
                if (winRatio > 0.55f)
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.blue, 0.01f);
                else if (winRatio < 0.45f)
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.red, 0.01f);
                else if (winRatio > 0.48f && winRatio < 0.52f)
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.green, 0.01f);

                //balance the players, if the winning player had a lot of health left, balance more
                balance.balance(score, 1 - i);
                //start again
                gameEnd();
                return;
            }
        }

        //both players still alive, next turn
        activePlayer++;
        activePlayer %= fighters.Length;
    }

    private void gameEnd()
    {
        //getWinRatio();
        getWinRatio(100);

        //game was too long, make both players stronger
        if (turns > gameConstraints[1])
        {
            balance.balance(1, -1);
        }
        //game was too short, make both players weaker
        else if (turns < gameConstraints[0])
        {
            balance.balance(-1, -1);
        }

        if (GraphData.instance != null)
        {
            //add the length of the game to that graph
            GraphData.instance.addGameLength(turns);
        }

        //start again
        turns = 0;
        fighters[0].reset();
        fighters[1].reset();
        advanceTurn();
    }

    float getWinRatio(int numMatches = -1)
    {
        bool allGames = false;
        if (numMatches == -1)
        {
            allGames = true;
            numMatches = gameResults.Count;
        }
        //not enough matches yet
        if (numMatches > gameResults.Count)
        {
            numMatches = gameResults.Count;
        }

        //count p1 wins
        int p1Wins = 0;
        for (int i = gameResults.Count - numMatches; i < gameResults.Count; i++)
        {
            if (gameResults[i])
                p1Wins++;
        }

        //Debug.Log("Win rate of the last " + numMatches + " matches: " + p1Wins + " : " + (numMatches - p1Wins));
        if (GraphData.instance != null)
        {
            if (allGames)
            {
                GraphData.instance.addTotalWinRatio(p1Wins, numMatches);
            }
            else
            {
                GraphData.instance.addXGamesWinRatio(p1Wins, numMatches);
            }
        }
        return (float)p1Wins / numMatches;
    }
}
