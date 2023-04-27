using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Move
{
    [SerializeField] GameObject hitboxVisualPrefab;   // The prefab of the visual for the hitbox (Red Line)
    [SerializeField] GameObject hitboxPrefab;         // The prefab of the hitbox of the move (Collider Line)

    [SerializeField] int damage;          // The damage of the move
    [SerializeField] float knockback;     // The knockback of the move

    private bool active = false;          // True if move is currently being used by the player
    private bool playerControl = false;   // True if the player is using the move. Control of the move is removed when false
    private Camera cam;                   // The main game camera
    private Transform player;             // Position of the move user
    private Pathfinder pathfinder;        // Pathfinder of the move user
    private GameObject hitboxVisual;      // The currently instantiated hitbox visual prefab
    private GameObject hitbox;            // The currently instantiated hitbox prefab
    private SniperHitbox sniperhb;        // Script for the hitbox to add targets
    private float hitboxRotation;         // Keeps track of how the hitbox should be rotated
    private BoxCollider2D playerHurtBox;  // The user's hurtbox (The move's hitbox rotates around this)
    private Vector3 axis = new Vector3(0, 0, 1); // The rotation axis for the hitbox
    private Vector3 halfHeight;           // Half the user's height. Used to find middle of sprite
    private ContactFilter2D detectCircle; // Contact Filter that will detect triggers (for the movement circle)

    private float optimalScore;           // Best score for possible moves based on enemy AI calculation
    private GameObject optimalTarget;     // Best person to aim at based on enemy AI calculation
    private List<Vector2> optimalPath;    // Best spot to stand based on enemy AI calculation
    // private float optimalRotation;        // Best angle to aim based on enemy AI calcuation

    public override void execute(GameObject user)
    {
        playerControl = true;
        // Sniper object should be a child of BattleManager
        if (gameObject.transform.parent.TryGetComponent(out NewBattleSystem n))
        {
            battleSystem = n;
        }
        else Debug.Log("Could not find BattleSystem for move Sniper.");

        player = user.transform;
        playerHurtBox = user.GetComponent<BoxCollider2D>();
        halfHeight = new Vector3(0, player.GetComponentInChildren<SpriteRenderer>().bounds.size.y/2, 0);

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
        NewBattleSystem nbs = gameObject.GetComponentInParent<NewBattleSystem>();
        nbs.unfreezeAll();

        foreach (GameObject t in targets)
        {
            if (t.GetComponent<Unit>().GetMorsels().Count > 0)
            {
                t.GetComponent<Unit>().TakeDamage(damage, true);
            }
            

            // Call Knockback function
            Knockback(t, knockback);
        }

        clearTargets();
        Destroy(hitbox);
        Destroy(hitboxVisual);

        if (playerControl)
        {
            yield return StartCoroutine(
                battleSystem.friendlyUsedTurn(
                    player.gameObject,
                    player.gameObject.name + " used Sniper."
                    ));
        }
        else
        {
            yield return StartCoroutine(
                battleSystem.enemyUsedTurn(
                    player.gameObject,
                    player.gameObject.name + " used Sniper."
                    ));
        }

        cancel();
    }

    // Temporary function until I put in the advanced AI
    // Finds the first living target as the best spot to aim.
    public override float enemyFindOptimal(List<GameObject> targets, GameObject user)
    {
        pathfinder = user.GetComponent<Pathfinder>();
        detectCircle.useTriggers = true;
        optimalScore = Mathf.NegativeInfinity;
        foreach (GameObject target in targets)
        {
            if (!target.GetComponent<Unit>().dead)
            {
                RaycastHit2D[] results = new RaycastHit2D[20];                        // Linecast collisions are stored here
                Physics2D.Linecast(target.transform.position, user.transform.position, detectCircle, results);  // Draws a line from where corner is and will be
                List<Vector2> path = new();
                bool obstacle = false;
                float score = 0;
                foreach (RaycastHit2D hit in results)
                {
                    if (hit.collider.gameObject == user.gameObject)
                    {
                        break;
                    }
                    else if (hit.collider.gameObject.TryGetComponent<Unit>(out Unit u))
                    {
                        score += damage;
                    }
                    else if (hit.collider.gameObject.name == "MovementCircle(Clone)")
                    {
                        path = pathfinder.findPathTo(hit.point);
                    }
                    else obstacle = true;
                }

                if (score > optimalScore) //&& !obstacle)
                {
                    optimalScore = score;
                    optimalPath = path;
                    optimalTarget = target;
                }
                return 100;
            }
        }
        return 100;
    }

    public override IEnumerator enemyAnim(GameObject user)
    {
        NewBattleSystem nbs = gameObject.GetComponentInParent<NewBattleSystem>();
        nbs.unfreezeAll();

        pathfinder = user.GetComponent<Pathfinder>();
        yield return StartCoroutine(pathfinder.moveAlongPath(optimalPath));

        // Create all move assets
        execute(user);
        playerControl = false;

        // Position assets correctly
        if (optimalTarget == null)
        {
            Debug.Log("vruh");
        }
        hitboxRotation = Vector3.SignedAngle(new Vector3(1, 0), optimalTarget.transform.position - user.transform.position, axis);

        hitboxVisual.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);
        hitbox.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);

        yield return new WaitForSeconds(1);

        // Use attack
        yield return StartCoroutine(anim());
    }

    // Update is called once per frame
    void Update()
    {
        if (active && playerControl)
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
        }/*
        else
        {
            if (active)
            {
                Debug.Log("Adding " + t.gameObject.name + " denied. (Not self-damaging move)");
            }
            else
            {
                Debug.Log("Adding " + t.gameObject.name + " denied. (Move is not active)");
            }
        }*/
    }

    public void removeTarget(GameObject t)
    {
        // Do not add the player to targets
        if (t != player.gameObject && active)
        {
            removeFromTargets(t);
        }/*
        else
        {
            if (active)
            {
                Debug.Log("Removing " + t.gameObject.name + " denied. (Should not be in target list)");
            }
            else
            {
                Debug.Log("Removing " + t.gameObject.name + " denied. (List already cleared)");
            }
        }*/
    }
}
