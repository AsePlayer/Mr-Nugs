using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    // The name of the status effect
    public string effectName;

    // The duration of the status effect in turns
    protected int duration;

    public int currentDuration;

    // The number of times this status effect has been stacked
    [SerializeField] protected int stackCount;

    // Apply the status effect to the target
    public virtual void ApplyEffect(Unit target)
    {
        // Override this method in derived classes to implement the specific behavior of the status effect
    }

    // Increase the stack count of this status effect
    public virtual void Stack()
    {
        // stackCount++;
        // currentDuration += duration;
        // print("current duration is: " + currentDuration + " and duration is: " + duration);
    }

    // Decrease the stack count of this status effect
    public void Unstack()
    {
        stackCount--;
    }

    // Get the stack count of this status effect
    public int GetStackCount()
    {
        return stackCount;
    }

    // Reset the stack count of this status effect
    public void ResetStackCount()
    {
        stackCount = 0;
        currentDuration = 0;
    }

}
