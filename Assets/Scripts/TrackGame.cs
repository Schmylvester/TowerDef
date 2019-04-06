using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGame : MonoBehaviour
{
    [SerializeField] BalanceAttacks balance;
    [SerializeField] Fighter[] fighters;
    [SerializeField] Camera cam;

    //min and max length of a game in turns
    [SerializeField] Vector2Int gameConstraints;
    //the player whose turn it is
    int activePlayer = 0;
    //turns passed in this instance of the game
    int turns = 0;
    //results of all games so far
    List<bool> gameResults = new List<bool>();


    public bool isActivePlayer(Fighter fighter)
    {
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

                cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.white, 0.01f);
                //change the camera colour slightly to help visualise winning streaks
                if (i == 0)
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.red, 0.01f);
                else
                    cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.blue, 0.01f);

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
        //check out what's up every 100 games
        if (gameResults.Count % 1000 == 0)
        {
            Debug.Break();
        }
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

    void getWinRatio(int numMatches = -1)
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
            return;
        }

        //count p1 wins
        int p1Wins = 0;
        for (int i = gameResults.Count - numMatches; i < gameResults.Count; i++)
        {
            if (gameResults[i])
                p1Wins++;
        }
        
        Debug.Log("Win rate of the last " + numMatches + " matches: " + p1Wins + " : " + (numMatches - p1Wins));

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
    }
}
