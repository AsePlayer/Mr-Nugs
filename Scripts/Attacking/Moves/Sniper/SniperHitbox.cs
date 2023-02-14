using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperHitbox : MonoBehaviour
{
    private Sniper sniper;

    public void setSniper(Sniper s)
    {
        sniper = s;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print(collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out Unit h))
        {
            sniper.addTarget(collision.gameObject);
            // print(collision.gameObject.name + " added");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //print(collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out Unit u))
        {
            sniper.removeTarget(collision.gameObject);
            // print(collision.gameObject.name + " removed");
        }
    }
}
