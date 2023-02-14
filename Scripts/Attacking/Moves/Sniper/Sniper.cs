using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Move
{
    [SerializeField] GameObject hitboxVisualPrefab;   // The prefab of the visual for the hitbox (Red Line)
    [SerializeField] GameObject hitboxPrefab;         // The prefab of the hitbox of the move (Collider Line)

    private bool active = false;          // True if move is currently being used by the player
    private Camera cam;                   // The main game camera
    private Transform player;             // Position of the move user
    private GameObject hitboxVisual;      // The currently instantiated hitbox visual prefab
    private GameObject hitbox;            // The currently instantiated hitbox prefab
    private SniperHitbox sniperhb;        // Script for the hitbox to add targets
    private float hitboxRotation;         // Keeps track of how the hitbox should be rotated
    private BoxCollider2D playerHurtBox;  // The user's hurtbox (The move's hitbox rotates around this)
    private Vector3 axis = new Vector3(0, 0, 1); // The rotation axis for the hitbox
    private Vector3 halfHeight;           // Half the user's height. Used to find middle of sprite

    public override void execute(GameObject user)
    {
        // Sniper object should be a child of BattleManager
        if (gameObject.transform.parent.TryGetComponent(out NewBattleSystem n))
        {
            battleSystem = n;
        }
        else Debug.Log("Could not find BattleSystem for move Sniper.");

        player = user.transform;
        playerHurtBox = user.GetComponent<BoxCollider2D>();
        halfHeight = new Vector3(0, player.GetComponent<SpriteRenderer>().bounds.size.y/2, 0);

        // Instantiates hitbox at correct rotation in terms of mouse placement
        cam = Camera.main;
        Vector3 middle = player.position + halfHeight;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        hitboxRotation = Vector3.Angle(middle, new Vector3(ray.origin.x, ray.origin.y));

        hitboxVisual = Instantiate(hitboxVisualPrefab, middle, Quaternion.identity);
        hitbox = Instantiate(hitboxPrefab, player.position + new Vector3(playerHurtBox.offset.x * player.localScale.x, playerHurtBox.offset.y * player.localScale.y), Quaternion.identity);
        hitboxVisual.transform.SetParent(player);
        hitbox.transform.SetParent(player);

        hitboxRotation = Vector3.SignedAngle(new Vector3(1, 0), new Vector3(ray.origin.x, ray.origin.y) - player.position, axis);
        hitboxVisual.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);
        hitbox.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);

        // Set some variables
        sniperhb = hitbox.GetComponentInChildren<SniperHitbox>();
        sniperhb.setSniper(this);

        // Start update function
        active = true;
    }

    public override void cancel()
    {
        // Stop update function
        active = false;

        // Remove instantiated gameobjects
        clearTargets();
        Destroy(hitbox);
        Destroy(hitboxVisual);
        Destroy(gameObject);
    }

    protected override IEnumerator anim()
    {
        active = false;

        foreach (GameObject t in targets)
        {
            if (t.GetComponent<Unit>().GetMorsels().Count > 0)
            {
                t.GetComponent<Unit>().TakeDamage(100, true);
            }
            

            // Call Knockback function
            Knockback(t, 25f);
        }

        clearTargets();
        Destroy(hitbox);
        Destroy(hitboxVisual);

        yield return StartCoroutine(
            battleSystem.friendlyUsedTurn(
                player.gameObject,
                player.gameObject.name + " used Sniper."
                ));

        cancel();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // Sets the hitbox rotation relative to the mouse
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            hitboxRotation = Vector3.SignedAngle(new Vector3(1, 0), new Vector3(ray.origin.x, ray.origin.y) - (player.position + halfHeight), axis);

            hitboxVisual.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);
            hitbox.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);

            // Damages units in 'targets' on click
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(anim());
            }
        }
    }

    void Knockback(GameObject target, float force)
    {
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        // Apply force to rb
        rb.AddForce((target.transform.position - player.position).normalized * force, ForceMode2D.Impulse);
    }

    // SniperHitbox.cs script attached to GameObject 'hitbox' will call these functions when entering or exiting range of a unit
    public void addTarget(GameObject t)
    {
        // Do not add the player to targets
        if (t != player.gameObject && active)
        {
            addToTargets(t);
        }
        else
        {
            if (active)
            {
                // Debug.Log("Adding " + t.gameObject.name + " denied. (Not self-damaging move)");
            }
            else
            {
                // Debug.Log("Adding " + t.gameObject.name + " denied. (Move is not active)");
            }
        }
    }

    public void removeTarget(GameObject t)
    {
        // Do not add the player to targets
        if (t != player.gameObject && active)
        {
            removeFromTargets(t);
        }
        else
        {
            if (active)
            {
                //Debug.Log("Removing " + t.gameObject.name + " denied. (Should not be in target list)");
            }
            else
            {
                //Debug.Log("Removing " + t.gameObject.name + " denied. (List already cleared)");
            }
        }
    }
}
