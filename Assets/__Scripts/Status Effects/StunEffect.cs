using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : StatusEffect
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
            target.currentSpeed = 0;  
            print(target.nameOfUnit + " is stunned!");
        }
        else 
        {
            target.currentSpeed = target.currentSpeed / (stackCount);
            print(target.nameOfUnit + " is slowed!");
        }
        
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
