﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    FindBestAction bestAction = null;
    [SerializeField] TrackGame game = null;
    [SerializeField] Fighter enemy = null;
    [SerializeField] Image hpSprite = null;
    [SerializeField] Text hpText = null;
    [Header("Useful things")]
    [SerializeField] bool aiControlled = false;
    Attack[] attacks;
    int[] attackUses;
    float timer = 0;
    int health;
    [SerializeField] int startHealth;
    [SerializeField] int maxHealthDivergence = 30;

    public void start()
    {
        bestAction = GetComponent<FindBestAction>();
        attacks = GetComponentsInChildren<Attack>();
        attackUses = new int[attacks.Length];
        health = startHealth;
        updateUI();
    }

    private void Update()
    {
        //if space bar is held, delay the update slightly
        if (slowTime())
            return;
        //get the input
        int input = getInput();
        //it's my turn and I've given an input
        if (game.isActivePlayer(this) && input >= 0)
        {
            //record this action if i can
            if (bestAction)
            {
                bestAction.recordEvent(this, enemy, input);
            }
            //use this attack
            attackUses[input]++;
            attacks[input].use();
            //my turn is over
            game.advanceTurn();
        }
    }

    int getInput()
    {
        int input = -1;
        //if it's an AI
        if (aiControlled)
        {
            if (bestAction)
            {
                input = bestAction.getBestAction(this, enemy);
            }
            else
            {
                //don't have a best action component, random action will do
                input = Random.Range(0, attacks.Length);
            }
        }
        else
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                //the numbers on the keyboard for controls
                if (Input.GetKeyDown((KeyCode)(i + 49)))
                {
                    input = i;
                }
            }
        }
        return input;
    }

    bool slowTime()
    {
        //if space is held, wait 0.2 seconds between updates
        if (Input.GetKey(KeyCode.Space))
        {
            timer += Time.deltaTime;
            if (timer < 0.2f)
            {
                return true;
            }
        }
        timer = 0;
        return false;
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

    public int getStartHealth()
    {
        return startHealth;
    }

    public float getScore()
    {
        //score is how much health i have left
        return (float)health / startHealth;
    }

    public void endGame(float score)
    {
        //tell the action prediction how well i did
        if (bestAction)
        {
            bestAction.endGame(score);
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
        //has full health and hasn't used any attacks
        health = startHealth;
        for (int i = 0; i < attackUses.Length; i++)
            attackUses[i] = 0;
        updateUI();
    }

    public void lengthBalance(float modifier)
    {
        int attackSum = 0;
        for (int i = 0; i < attackUses.Length; i++)
        {
            attackSum += attackUses[i];
        }
        for (int i = 0; i < attackUses.Length; i++)
        {
            //if they used any attack
            if (attackSum != 0)
            {
                float use = (float)attackUses[i] / attackSum;
                //if the unit didn't win, both their attacks should be modified
                if (modifier > 1)
                {
                    attacks[i].balance(modifier, startHealth);
                }
                else
                {
                    attacks[i].balance(use * modifier, startHealth);
                }
            }
            else
            {
                //they were killed before using an attack, buff them way up
                attacks[i].balance(0.8f, int.MaxValue);
            }
        }
    }

    public void balance(float modifier)
    {
        //change their health
        modifyHealth(modifier < 0);

        //count how much they used each attack
        int attackSum = 0;
        for (int i = 0; i < attackUses.Length; i++)
        {
            attackSum += attackUses[i];
        }
        for (int i = 0; i < attackUses.Length; i++)
        {
            //if they used any attack
            if (attackSum != 0)
            {
                float use = (float)attackUses[i] / attackSum;
                //if the unit didn't win, both their attacks should be modified
                if (modifier > 1)
                {
                    attacks[i].balance(modifier, startHealth);
                }
                else
                {
                    attacks[i].balance(use * modifier, startHealth);
                }
            }
            else
            {
                //they were killed before using an attack, buff them way up
                attacks[i].balance(0.8f, int.MaxValue);
            }
            if (GraphData.instance != null)
            {
                int playerID = gameObject.name == "Fighter1" ? 0 : 1;
                GraphData.instance.addPlayerAttackRatio(attackUses[i], attackSum, i, playerID);
            }
        }

    }

    void modifyHealth(bool reduce)
    {
        if (reduce)
        {
            startHealth--;
            //make sure my health stays within my enemy's health
            if (startHealth < enemy.getStartHealth() - maxHealthDivergence)
            {
                startHealth += maxHealthDivergence / 2;
            }
        }
        else
        {
            startHealth++;
            //make sure my health stays within my enemy's health
            if (startHealth < enemy.getStartHealth() - maxHealthDivergence)
            {
                startHealth -= maxHealthDivergence / 2;
            }
        }
        //not too low
        if (startHealth < 10)
            startHealth = 10;
    }

    public Fighter getEnemy()
    {
        return enemy;
    }
}
