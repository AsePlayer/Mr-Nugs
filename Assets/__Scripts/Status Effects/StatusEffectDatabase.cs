using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDatabase : MonoBehaviour
{
    // The list of status effect scripts that will be manually dragged into this script. it is inherited from the StatusEffect class
    public List<StatusEffect> statusEffects;
    // Start is called before the first frame update
    void Start()
    {
        // Loop through the list of status effects, setting the stack count of each status effect to 0
        foreach (StatusEffect effect in statusEffects)
        {
            effect.ResetStackCount();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Get the status effect with the given name
    public StatusEffect GetStatusEffect(string effectName)
    {
        // Loop through the list of status effects
        foreach (StatusEffect effect in statusEffects)
        {
            // If the name of the status effect matches the given name, return the status effect
            if (effect.effectName == effectName)
            {
                return effect;
            }
        }
        // If no status effect with the given name was found, return null
        return null;
    }
}
