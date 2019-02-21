using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTowerPanel : MonoBehaviour {

    [SerializeField] Text towerName;
    [SerializeField] Image sprite;
    [SerializeField] Text cost;
    [SerializeField] Text range;
    [SerializeField] Text rateOfFire;
    [SerializeField] Text damage;

    public static UpdateTowerPanel instance;

    private void Start()
    {
        instance = this;
    }

    public void updatePanel(Sprite _sprite, string _name, float _range, float _rof, short _damage, short _cost)
    {
        sprite.sprite = _sprite;
        towerName.text = _name;
        cost.text = "Cost: " + _cost;
        range.text = "Range: " + _range;
        rateOfFire.text = "Fire Rate: " + _rof;
        damage.text = "Damage: " + _damage;
    }
}
