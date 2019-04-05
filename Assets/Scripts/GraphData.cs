//total win ratio
//win ratio 100 games
//each players attack use ratio

using System.IO;
using UnityEngine;

public class GraphData : MonoBehaviour
{
    public static GraphData instance;
    StreamWriter totalGames;
    StreamWriter xGames;
    StreamWriter gameLength;
    StreamWriter[] p1attackRatio;
    StreamWriter[] p2attackRatio;
    int gamePassed = 0;
    int addRate = 1;

    private void Start()
    {
        instance = this;
        totalGames = new StreamWriter("Assets/NewData/total.txt", false);
        xGames = new StreamWriter("Assets/NewData/100.txt", false);
        gameLength = new StreamWriter("Assets/NewData/GameLength", false);
        p1attackRatio = new StreamWriter[2]
        {
            new StreamWriter("Assets/NewData/p1attack1.txt", false),
            new StreamWriter("Assets/NewData/p1attack2.txt", false)
        };
        p2attackRatio = new StreamWriter[2]
        {
            new StreamWriter("Assets/NewData/p2attacks1.txt", false),
            new StreamWriter("Assets/NewData/p2attacks2.txt", false)
        };
    }

    private void OnDestroy()
    {
        totalGames.Close();
        xGames.Close();
        gameLength.Close();
        foreach(StreamWriter w in p1attackRatio)
        {
            w.Close();
        }
        foreach (StreamWriter w in p2attackRatio)
        {
            w.Close();
        }
    }

    public void addTotalWinRatio(int p1Wins, int games)
    {
        if (gamePassed++ % addRate == 0)
        {
            float winRatio = (float)(p1Wins) / games;
            totalGames.WriteLine(winRatio);
        }
    }

    public void addXGamesWinRatio(int p1Wins, int games)
    {
        if (gamePassed % addRate == 0)
        {
            float winRatio = (float)(p1Wins) / games;
            xGames.WriteLine(winRatio);
        }
    }

    public void addGameLength(int length)
    {
        gameLength.WriteLine(length);
    }

    public void addPlayerAttackRatio(int uses, int totalAttacks, int attackID, int playerID)
    {
        if (gamePassed % addRate == 0)
        {
            if (playerID == 0)
            {
                p1attackRatio[attackID].WriteLine((float)uses / totalAttacks);
            }
            else
            {
                p2attackRatio[attackID].WriteLine((float)uses / totalAttacks);
            }
        }
    }
}