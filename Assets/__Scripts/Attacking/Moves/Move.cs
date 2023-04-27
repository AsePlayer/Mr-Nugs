using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    protected List<GameObject> targets = new List<GameObject>(); // Every game object with a health component in the hitbox
    protected Dictionary<GameObject, GameObject> targetRings = new Dictionary<GameObject, GameObject>(); // Key = Targeted unit, Val = Target Ring
    [SerializeField] protected GameObject targetRingPrefab;

    protected NewBattleSystem battleSystem;

    // Called when a move is selected
    public virtual void execute(GameObject user)
    {

    }

    // Called when a move is deselected
    public virtual void cancel()
    {

    }

    // What happens when you use the move.
    // Idk how animations actually work yet.
    // Probably not Coroutine function, but Coroutine function works for now.
    protected virtual IEnumerator anim()
    {
        return null;
    }

    // Each move will have its own way of calculating score.
    // Many scores will be calculated for many possibilities of ways to use the move.
    // The best score is returned, and compared with the best score of any other moves.
    public virtual float enemyFindOptimal(List<GameObject> targets, GameObject user)
    {
        return 0;
    }

    // This function is called when an enemy uses the move.
    public virtual IEnumerator enemyAnim(GameObject user)
    {
        return null;
    }

    protected void addToTargets(GameObject targetUnit)
    {
        targets.Add(targetUnit);
        GameObject ring = Instantiate(targetRingPrefab, targetUnit.transform.position + new Vector3(0, 0.25f), Quaternion.identity);
        ring.transform.SetParent(targetUnit.transform);
        float scale = targetUnit.GetComponentInChildren<SpriteRenderer>().bounds.size.x * 0.25f / targetUnit.transform.localScale.x;
        ring.GetComponent<TargetMarker>().defaultScale = new Vector2(scale, scale * 0.5f);
        targetRings.Add(targetUnit, ring);
        // printDictionary(targetRings);
    }

    protected void removeFromTargets(GameObject targetUnit)
    {
        targets.Remove(targetUnit);
        GameObject destroyThis = targetRings[targetUnit];
        targetRings.Remove(targetUnit);
        Destroy(destroyThis);
        // printDictionary(targetRings);
    }

    protected void clearTargets()
    {
        targets.Clear();
        foreach (KeyValuePair<GameObject, GameObject> i in targetRings)
        {
            Destroy(i.Value);
        }
        targetRings.Clear();
        // printDictionary(targetRings);
    }

    // For debugging
    private void printDictionary(Dictionary<GameObject, GameObject> d)
    {
        string log = "";
        foreach (KeyValuePair<GameObject, GameObject> i in targetRings)
        {
            log += i.Key.name + ", " + i.Value.name + "\n";
        }
        Debug.Log(log);
    }

    // Update is called once per frame
    // Has most of the functionality of the move
    //public virtual void Update()
    //{
        
    //}
}
