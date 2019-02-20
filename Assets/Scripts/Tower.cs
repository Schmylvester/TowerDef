using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    [SerializeField] float range;
    [SerializeField] float rateOfFire;
    [SerializeField] short damage;
    float timer = 0;
    List<Unit> units = new List<Unit>();

    private void Start()
    {
        EntityTrack.instance.addTower(this);
    }

    public void addUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void removeUnit(Unit unit)
    {
        units.Remove(unit);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > rateOfFire)
        {
            foreach (Unit unit in units)
            {
                float dist = (transform.position - unit.transform.position).magnitude;
                if (dist < range)
                {
                    unit.takeDamage(damage, gameObject, Color.green);
                    timer = 0;
                    break;
                }
            }
        }
    }

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, range);
    }
#endif

}
