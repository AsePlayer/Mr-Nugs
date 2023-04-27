using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpriteCutter;

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
    // Animator
    public Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 scale;

    LinecastCutter linecastCutter;
    public float spriteHeight;


    // Some prefabs for display
    [SerializeField] GameObject dmgNum;

    void Start()
    {
        statusEffectDatabase = GameObject.Find("BattleManager").GetComponent<StatusEffectDatabase>();
        // Set the current speed to the unit's speed
        currentSpeed = speed;
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        scale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();

        linecastCutter = GetComponent<LinecastCutter>();
        spriteHeight = GetComponentInChildren<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        if(anim == null) return;
        // check if moving
        if (isMoving())
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    /*******************************************
     *         Status Effect Related Stuff
     * *****************************************/

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
            // TODO: Trigger mesh slicing to visualize the loss of morsel via a Interface.
            float offset = morsels.Count / 1.75f;
			Vector2 lineStart = transform.position + new Vector3( -2, (morsels.Count + offset) / spriteHeight, 0 );
			Vector2 lineEnd = transform.position + new Vector3(2, (morsels.Count + offset) / spriteHeight, 0 );
			// randomly modify the angle of the cut
			lineStart.y += Random.Range( -0.25f, 0.25f );

			// draw the line in the scene view
			// GetComponent<LineRenderer>().SetPosition( 0, lineStart );
			// GetComponent<LineRenderer>().SetPosition( 1, lineEnd );


            morsels.RemoveAt(morsels.Count - 1);
			linecastCutter.LinecastCut( lineStart, lineEnd, linecastCutter.layerMask.value );

            if (morsels.Count <= 0)
            {
                // Cleanup Unit's BattleHUD if one exists.
                if (gameObject.TryGetComponent(out BattleHUD b))
                {
                    b.Cleanup();
                }

                // Destroy the gameobject
                // Destroy(gameObject);
                // Trigger Death Animation "Nug_Death"
                anim.SetBool("isDead", true);
                dead = true;
            }
        }
    }

    // Check if moving
    public bool isMoving()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if(dead || usedTurn) return false;
        if (rb.velocity != Vector2.zero)
        {
            // if moving left, flip sprite
            if (rb.velocity.x < 0)
                spriteRenderer.flipX = true;
            // transform.localScale = new Vector3(-scale.x, scale.y, 1);
            else
                spriteRenderer.flipX = false;
                // transform.localScale = new Vector3(scale.x,scale.y, 1);

                return true;
        }
        return false;
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
        // if morselCaps is missing an entry compared to morsels, then return the entry in morsels. do this by checking if the index is out of bounds.
        if (morselCaps.Count - 1 < morsels.Count - 1)
        {
            return morsels[morsels.Count - 1];
        }
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
