using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamHitbox : MonoBehaviour
{
    private Slam slam;

    public void setSlam(Slam s)
    {
        slam = s;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // print(collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out Unit u))
        {
            slam.addTarget(collision.gameObject);
            // print(collision.gameObject.name + " added");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //print(collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out Unit u))
        {
            slam.removeTarget(collision.gameObject);
            // print(collision.gameObject.name + " removed");
        }
    }
}
