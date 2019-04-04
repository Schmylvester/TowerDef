using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    [SerializeField] Fighter enemy;
    [SerializeField] TrackGame game;
    [SerializeField] int startHealth;
    int health;
    [SerializeField] Image hpSprite;
    [SerializeField] Text hpText;
    [SerializeField] Attack[] attacks;
    int wins = 0;
    [SerializeField] bool aiControlled = false;
    int[] attackUses = new int[2];
    [SerializeField] FindBestAction action = null;
    float timer = 0;

    void Start()
    {
        health = startHealth;
        updateUI();
    }

    private void Update()
    {
        //timer += Time.deltaTime;
        //if (timer < 0.02f)
            //return;
        //timer = 0;
        int input = -1;
        if (aiControlled)
        {
            if (action)
                input = action.getBestAction(this, enemy);
            else
                input = Random.Range(0, 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            input = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            input = 1;
        }
        if (game.isActivePlayer(this) && input >= 0)
        {
            if(action)
            {
                action.recordEvent(this, enemy, input);
            }
            attackUses[input]++;
            attacks[input].use();
            game.advanceTurn();
        }
    }

    public void takeDamage(int dam)
    {
        health -= dam;
        health = Mathf.Clamp(health, 0, startHealth);
        updateUI();
    }

    public int getHealth()
    {
        return health;
    }

    public float getScore()
    {
        return (float)health / startHealth;
    }

    public void win()
    {
        if(++wins > 1000)
        {
            Debug.Break();
        }
    }

    public void endGame(float score)
    {
        if (action)
        {
            action.endGame(score);
        }
    }

    void updateUI()
    {
        float h = (float)health / startHealth;
        hpSprite.transform.localScale = new Vector3(h, 1, 1);
        hpText.text = health + "/" + startHealth;
    }

    public void reset()
    {
        health = startHealth;
        for (int i = 0; i < attackUses.Length; i++)
            attackUses[i] = 0;
    }

    public void balanceAttacks(float score)
    {
        int attackSum = 0;
        for (int i = 0; i < attackUses.Length; i++)
        {
            attackSum += attackUses[i];
        }
        for (int i = 0; i < attackUses.Length; i++)
        {
            float use = (float)attackUses[i] / attackSum;
            attacks[i].balance(use * score);
        }
    }
}
