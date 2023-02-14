using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : StatusEffect
{
    // Start is called before the first frame update
    void Awake()
    {
        // Set the name of the status effect
        effectName = "Stun";
        // Set the duration of the status effect
        duration = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Stack()
    {
        stackCount++;
        currentDuration += duration;
        print("current duration is: " + currentDuration + " and duration is: " + duration);
    }
    public override void ApplyEffect(Unit target)
    {
        // Increase the stack count of the status effect
        Stack();
        
        if(currentDuration == 0)
            return;

        if(stackCount == 0)
            stackCount = 1;

        // Stun the target
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
}
