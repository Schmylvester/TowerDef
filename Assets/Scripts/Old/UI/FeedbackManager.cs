using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] Text attacker = null;
    [SerializeField] Text defender = null;
    public static FeedbackManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void setFeedback(bool attack, string feedback, Color color)
    {
        if (attack)
        {
            attacker.color = color;
            attacker.text = feedback;
        }
        else
        {
            defender.color = color;
            defender.text = feedback;
        }
    }
    public void setFeedback(bool attack, string feedback)
    {
        setFeedback(attack, feedback, Color.black);
    }
}
