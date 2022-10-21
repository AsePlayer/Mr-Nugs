using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private Unit unit;
    private int actualLives; // Whent his hits 0, Unit dies!
    public List<int> morsels = new List<int>();
    [SerializeField] GameObject dmgNum;


    // Start is called before the first frame update
    void Start()
    {
        // Initial health setup
        if(GetComponent<Unit>() != null)
        {
            unit = GetComponent<Unit>();
        
            for (int i = 0; i < unit.lives; i++)
            {
                morsels.Add(unit.health);
            }
            actualLives = unit.lives;
        }

    }

    // if clicked take damage (test)
    private void OnMouseDown()
    {
        TakeDamage(33, true);
    }

    public void TakeDamage(int damage, bool showDamage)
    {
        // Take damage
        morsels[morsels.Count - 1] -= damage;

        // Display damage numbers
        if (showDamage)
        {
            // TODO: Maybe the bigger the damage, the bigger the scale?
            GameObject dmg = Instantiate(dmgNum);
            dmg.transform.SetParent(GameObject.Find("Canvas").transform);
            dmg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            dmg.GetComponent<RectTransform>().transform.position = gameObject.transform.position + new Vector3(0.5f, 0.75f, 0);
            dmg.GetComponent<DmgNum>().displaydmg(-damage);
        }

        CheckMorselForDeath();
    }

    private void CheckMorselForDeath()
    {
        // Remove morsel if HP too low!
        if(morsels[morsels.Count - 1] <= 0) {
            morsels.RemoveAt(morsels.Count - 1);
            actualLives--;
            // TODO: Trigger mesh slicing to visualize the loss of morsel via a Interface.
            
            if(actualLives <= 0)
            {
                // Cleanup Unit's BattleHUD if one exists.
                if(gameObject.GetComponent<BattleHUD>() != null)
                    gameObject.GetComponent<BattleHUD>().Cleanup();

                // Destroy the gameobject
                Destroy(gameObject);
            }
        }
    }

    public List<int> GetMorsels() {
        return morsels;
    }
}
