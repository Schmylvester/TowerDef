using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHealth : MonoBehaviour
{
    [SerializeField] RectTransform healthSprite;

    public void updatePos(Entity entity, Sprite sprite)
    {
        transform.position = Camera.main.WorldToScreenPoint(entity.transform.position);
        transform.position += Vector3.down * ((sprite.rect.size.y / 2) * entity.transform.lossyScale.y);
    }

    public void updateHealth(Entity entity)
    {
        healthSprite.localScale = new Vector3(entity.getHealth(), 1, 1);
    }
}
