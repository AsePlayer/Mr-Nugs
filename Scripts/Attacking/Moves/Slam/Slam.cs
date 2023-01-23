using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slam : Move
{
    [SerializeField] GameObject hitboxPrefab;   // The prefab of the hitbox of the move (Oval)
    [SerializeField] float range = 3;     // The distance of the hitbox from the player

    private bool active = false;          // True if move is currently being used by the player
    private Camera cam;                   // The main game camera
    private List<GameObject> targets = new List<GameObject>();     // Every game object with a health component in the hitbox
    private Transform player;             // Position of the move user
    private GameObject hitbox;            // The currently instantiated hitbox prefab
    private SlamHitbox slamhb;            // Script for the hitbox to add targets
    private Rigidbody2D hitboxrb;         // Rigidbody component of Gameobject 'hitbox'
    private Vector3 hitboxPos;            // Keeps track of where the hitbox should be
    private BoxCollider2D playerHurtBox;  // The user's hurtbox (The move's hitbox rotates around this)

    private Vector3 scale = new Vector3(1, 0.5f, 0);  //Used for positioning of hitbox relative to mouse

    public float knockbackTime = 1f;

    private BattleSystem battleSystem;

    void Start()
    {
        // Find BattleSystem Component
        battleSystem = GameObject.Find("GameManager").GetComponent<BattleSystem>();
    }

    public override void execute(GameObject user)
    {
        player = user.transform;
        playerHurtBox = user.GetComponent<BoxCollider2D>();

        // Instantiates hitbox at correct postition in terms of mouse placement
        cam = Camera.main;
        Vector3 middle = player.position + new Vector3(playerHurtBox.offset.x * player.localScale.x, playerHurtBox.offset.y * player.localScale.y);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        hitboxPos = middle + range * Vector3.Scale((new Vector3(ray.origin.x, ray.origin.y) - middle).normalized, scale);

        hitbox = Instantiate(hitboxPrefab, hitboxPos, Quaternion.identity);

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
        targets.Clear();
        Destroy(hitbox);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // Sets hitbox position relative to mouse
            Vector3 middle = player.position + new Vector3(playerHurtBox.offset.x * player.localScale.x, playerHurtBox.offset.y * player.localScale.y);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            hitboxPos = middle + range * Vector3.Scale((new Vector3(ray.origin.x, ray.origin.y) - middle).normalized, scale);
            hitboxrb.position = hitboxPos;

            // Damages units in 'targets' on click
            if (Input.GetMouseButtonDown(0))
            {
                foreach (GameObject t in targets)
                {
                    t.GetComponent<Health>().TakeDamage(30, true);
                                        
                    // Call Knockback function
                    Knockback(t, 10f);
                }
                battleSystem.NewMovementCircle();

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
        targets.Add(t);
    }

    public void removeTarget(GameObject t)
    {
        targets.Remove(t);
    }
}
