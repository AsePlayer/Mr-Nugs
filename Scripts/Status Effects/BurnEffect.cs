using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : StatusEffect
{
    public override void Stack()
    {
        stackCount++;
        currentDuration += duration;
        print("current duration is: " + currentDuration + " and duration is: " + duration);
    }
    public override void ApplyEffectDuringTurn(Unit target)
    {
        // Increase the stack count of the status effect
        Stack();

        if(stackCount == 0) stackCount = 1;

        // Slow the target
        // If stack exceeds is 3 or more, set target.currentSpeed to 0. Stunned!
        if(stackCount >= 3)
        {
            target.TakeDamage(100, true);
            print(target.nameOfUnit + " blew up!!");
        }
        else 
        {
            // if they are not moving, they are burning
            // damage unit for 1 damage every 1 second

            print(target.nameOfUnit + " is burning!");
            // loop burning
            StartCoroutine(Burn(target));
        }
        
    }

    IEnumerator Burn(Unit target)
    {
        yield return new WaitForSeconds(1);
        
        // if the player has not moved, keep burning, use rb
        if(target.GetComponent<Rigidbody2D>().velocity == Vector2.zero && target.usedTurn == false)
            target.TakeDamage(stackCount, true);
        StartCoroutine(Burn(target));
    }
    public override void ApplyEffectAfterTurn(Unit target)
    {
        // Decrease the duration of the status effect
        currentDuration--;
        print("Effect applied. " + target.nameOfUnit + "'s " + effectName +  " has: " + currentDuration + " turns remaining.");
        // If the duration of the status effect is 0, remove the status effect from the target
        if (currentDuration == 0)
        {
            // Destroy the status effect
            Destroy(this);
        }
    }
}
