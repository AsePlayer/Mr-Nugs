using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    [SerializeField] GameObject hitbox;
    private bool dealDamage;
    
    private Unit unit;
    public int damage;


    [SerializeField] Collider2D mostRecentCollision;

    void Awake()
    {
        // Cache unit
        unit = GetComponentInParent<Unit>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if hitbox overlaps with another object
            if (hitbox.GetComponent<Collider2D>().IsTouching(mostRecentCollision) && mostRecentCollision.tag == "Enemy" && !dealDamage)
            {
                // Deal damage to enemy
                // mostRecentCollision.GetComponent<Unit>().health -= damage;
                // dealDamage = true;
                dealDamage = true;
                mostRecentCollision.gameObject.GetComponent<Unit>().TakeDamage(unit.attack, true);
                Debug.Log("Hit");
            }
            else
            {
                dealDamage = false;
            }

            // Check for all objects near the hitbox
            // Collider2D[] colliders = Physics2D.OverlapBoxAll(hitbox.transform.position, hitbox.transform.localScale, /*rotation of some kind*/);
            // physics2doverlapboxall 

            // Maybe try spawning object and then checking if it collides! PROB WAY EASIER!
        }
    }

    // Check if hitbox overlaps with another object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        mostRecentCollision = collision;
    }

    // On click, activate hitbox
    // private void OnMouseDown()
    // {
    //     dealDamage = true;
    //     // Check if hitbox overlaps with another object when mouse clicks
    // }

    // Get list of objects currently touching hitbox with a for loop



}
