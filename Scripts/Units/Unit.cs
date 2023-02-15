using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Unit stats
    public string nameOfUnit;
    public List<int> morsels;       // Unit's current morsel hp's
    public List<int> morselCaps;    // Unit's max hp for each morsel
    public int attack;
    public int defense;
    public float speed;

    public float currentSpeed;

    public List<GameObject> moves = new List<GameObject>();
    
    // Status effects
    public StatusEffectDatabase statusEffectDatabase;
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    // Used for battle algorithm
    public bool usedTurn = true;    // false = still has an action this turn
    public bool dead = false;       // if hp is drained and dead = false, death message is displayed
    public Vector3 startPos;        // position at start of turn

    // Some prefabs for display
    [SerializeField] GameObject dmgNum;

    void Start()
    {
        statusEffectDatabase = GameObject.Find("BattleManager").GetComponent<StatusEffectDatabase>();
        // Set the current speed to the unit's speed
        currentSpeed = speed;
    }

    public void ExecuteStatusEffects()
    {
        // Apply status effects
        foreach (StatusEffect se in statusEffects)
        {
            se.ApplyEffectAfterTurn(this);
            // se.currentDuration--;
        }

        // Remove status effects that have expired
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].currentDuration <= 0)
            {
                statusEffects.RemoveAt(i);
            }
        }
    }

    public void AddStatusEffect(StatusEffect se)
    {
        // Check if the status effect is already on the unit
        foreach (StatusEffect s in statusEffects)
        {
            if (s.effectName == se.effectName)
            {
                // If it is, then just increase the stack count
                s.ApplyEffectDuringTurn(this);
                return;
            }
        }

        // If it isn't, then add it to the list
        se = Instantiate(se);
        se.transform.SetParent(transform);
        statusEffects.Add(se);
        se.ApplyEffectDuringTurn(this);
    }

    /******************************************
     *          Health-Related Stuff
     * ***************************************/

    public void TakeDamage(int damage, bool showDamage)
    {
        // Take damage
        if (morsels.Count > 0)
        {
            morsels[morsels.Count - 1] -= damage;
        }
        else
        {
            Debug.Log("Bruh, " + nameOfUnit + " should be dead but they taking damage??!??!?!!??? \n" +
                "This is some crazy world we live in.");
        }

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

        // Update the HUD to represent health
        if (gameObject.TryGetComponent(out BattleHUD b))
        {
            b.SetHUD(this);
        }
    }

    private void CheckMorselForDeath()
    {
        // Remove morsel if HP too low!
        if (morsels[morsels.Count - 1] <= 0)
        {
            morsels.RemoveAt(morsels.Count - 1);
            // TODO: Trigger mesh slicing to visualize the loss of morsel via a Interface.

            if (morsels.Count <= 0)
            {
                // Cleanup Unit's BattleHUD if one exists.
                if (gameObject.TryGetComponent(out BattleHUD b))
                {
                    b.Cleanup();
                }

                // Destroy the gameobject
                // Destroy(gameObject);
            }
        }
    }

    public List<int> GetMorsels()
    {
        return morsels;
    }
    public List<int> GetMorselCaps()
    {
        return morselCaps;
    }
    public int getCurrentMorselHP()
    {
        return morsels[morsels.Count - 1];
    }
    public int getCurrentMorselMaxHP()
    {
        return morselCaps[morsels.Count - 1];
    }





    /*
    public int accuracy;
    public int evasion;
    public int critChance;
    public int critDamage;
    public int level;
    public int exp;
    public int expToNextLevel;
    public int expToLevelUp;
    */
}
