using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] protected Fighter user;
    [SerializeField] protected Fighter target;
    [SerializeField] protected float damage;
    [SerializeField] protected float accuracy;
    [SerializeField] protected UnityEngine.UI.Text damageUI;
    [SerializeField] protected UnityEngine.UI.Text accuracyUI;

    private void Start()
    {
        balance(0);
    }

    public void balance(float by)
    {
        if (Random.Range(0, 2) == 0)
        {
            accuracy = Mathf.Clamp(accuracy + by, 0.01f, 1.0f);
        }
        else
        {
            damage = Mathf.Max(1, damage * (1 + by));
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
