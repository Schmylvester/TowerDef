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
    [SerializeField] Text uses;
    [SerializeField] Text damage;

    public void updatePanel(Sprite _sprite, string _name, TowerStats stats)
    {
        sprite.sprite = _sprite;
        towerName.text = _name;
        cost.text = "Cost: " + stats.cost;
        range.text = "Range: " + stats.range;
        rateOfFire.text = "Fire Rate: " + (short)(30 / stats.rateFire);
        damage.text = "Damage: " + stats.damage;
        uses.text = "Uses: " + stats.uses;
    }
}
