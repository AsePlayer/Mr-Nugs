using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slam : Move
{
    [SerializeField] GameObject hitboxPrefab;   // The prefab of the hitbox of the move (Oval)
    [SerializeField] float range;         // The distance of the hitbox from the player
    [SerializeField] float size;          // Size multiplier of the hitbox
    [SerializeField] int damage;          // The damage of the move
    [SerializeField] float knockback;     // The knockback of the move

    private bool active = false;          // True if move is currently being used by the player
    private Camera cam;                   // The main game camera
    private Transform player;             // Position of the move user
    private GameObject hitbox;            // The currently instantiated hitbox prefab
    private SlamHitbox slamhb;            // Script for the hitbox to add targets
    private Rigidbody2D hitboxrb;         // Rigidbody component of Gameobject 'hitbox'
    private Vector2 hitboxPos;            // Keeps track of where the hitbox should be
    private BoxCollider2D playerHurtBox;  // The user's hurtbox (The move's hitbox rotates around this)

    private Vector3 scale = new Vector3(1, 0.5f, 0);  //Used for positioning of hitbox relative to mouse

    public override void execute(GameObject user)
    {
        // Slam object should be a child of GameManager
        if (gameObject.transform.parent.TryGetComponent(out NewBattleSystem n))
        {
            battleSystem = n;
        }
        else Debug.Log("Could not find BattleSystem for move Slam.");

        player = user.transform;
        playerHurtBox = user.GetComponent<BoxCollider2D>();

        // Instantiates hitbox at correct postition in terms of mouse placement
        cam = Camera.main;
        Vector2 middle = player.position + new Vector3(playerHurtBox.offset.x * player.localScale.x, playerHurtBox.offset.y * player.localScale.y);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        hitboxPos = middle + range * Vector2.Scale((new Vector2(ray.origin.x, ray.origin.y) - middle).normalized, scale);

        hitbox = Instantiate(hitboxPrefab, hitboxPos, Quaternion.identity);
        hitbox.transform.localScale = new Vector3(hitbox.transform.localScale.x*size, hitbox.transform.localScale.y*size, hitbox.transform.localScale.z*size);

        // Set some variables
        slamhb = hitbox.GetComponent<SlamHitbox>();
        slamhb.setSlam(this);
        hitboxrb = hitbox.GetComponent<Rigidbody2D>();

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

        yield return StartCoroutine(
            battleSystem.friendlyUsedTurn(
                player.gameObject,
                player.gameObject.name + " used Slam."
                ));

        cancel();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // Sets hitbox position relative to mouse
            Vector2 middle = player.position + new Vector3(playerHurtBox.offset.x * player.localScale.x, playerHurtBox.offset.y * player.localScale.y);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            hitboxPos = middle + range * Vector2.Scale((new Vector2(ray.origin.x, ray.origin.y) - middle).normalized, scale);
            hitboxrb.position = hitboxPos;

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

    // SlamHitbox.cs script attached to GameObject 'hitbox' will call these functions when entering or exiting range of a unit
    public void addTarget(GameObject t)
    {
        if (active)
        {
            addToTargets(t);
        }
        else
        {
           // Debug.Log("Removing " + t.gameObject.name + " denied. (Move is not active)");
        }
    }

    public void removeTarget(GameObject t)
    {
        if (active)
        {
            removeFromTargets(t);
        }
        else
        {
            // Debug.Log("Removing " + t.gameObject.name + " denied. (List already cleared)");
        }
    }
}
