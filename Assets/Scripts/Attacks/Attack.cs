using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    protected Fighter user;
    protected Fighter target;
    [SerializeField] protected float damage;
    [SerializeField] protected float accuracy;
    [SerializeField] protected Text damageUI;
    [SerializeField] protected Text accuracyUI;

    private void Start()
    {
        user = transform.parent.GetComponent<Fighter>();
        target = user.getEnemy();

        balance(0, int.MaxValue);
    }

    public virtual void balance(float by, int maxDam)
    {
        //either mod the accuracy or the damage
        if (Random.Range(0, 2) == 0)
        {
            //accuracy between 10% and 100%
            accuracy = Mathf.Clamp(accuracy + by, 0.1f, 1.0f);
        }
        else
        {
            if (by > 0)
                by = 1;
            else
                by = -1;
            //damage goes between 1 and maxDam
            damage = Mathf.Clamp(damage + by, 1, maxDam);
        }
        updateUI();
    }

    protected virtual void updateUI()
    {
        damageUI.text = "Damage - " + damage.ToString("0");
        accuracyUI.text = "Accuracy - " + (accuracy * 100).ToString("0");
    }

    public virtual void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)damage);
        }
    }
}
