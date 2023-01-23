using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Move
{
    [SerializeField] GameObject hitboxVisualPrefab;   // The prefab of the visual for the hitbox (Red Line)
    [SerializeField] GameObject hitboxPrefab;         // The prefab of the hitbox of the move (Collider Line)

    private bool active = false;          // True if move is currently being used by the player
    private Camera cam;                   // The main game camera
    private List<GameObject> targets = new List<GameObject>();     // Every game object with a health component in the hitbox
    private Transform player;             // Position of the move user
    private GameObject hitboxVisual;      // The currently instantiated hitbox visual prefab
    private GameObject hitbox;            // The currently instantiated hitbox prefab
    private SniperHitbox sniperhb;        // Script for the hitbox to add targets
    private float hitboxRotation;         // Keeps track of how the hitbox should be rotated
    private Quaternion currentRotation;   // Keeps track of the hitbox's current rotation
    private BoxCollider2D playerHurtBox;  // The user's hurtbox (The move's hitbox rotates around this)
    private Vector3 axis = new Vector3(0, 0, 1); // The rotation axis for the hitbox

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

        // Instantiates hitbox at correct rotation in terms of mouse placement
        cam = Camera.main;
        Vector3 middle = player.position;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        hitboxRotation = Vector3.Angle(middle, new Vector3(ray.origin.x, ray.origin.y));

        hitboxVisual = Instantiate(hitboxVisualPrefab, middle, Quaternion.identity);
        hitbox = Instantiate(hitboxPrefab, middle + new Vector3(playerHurtBox.offset.x * player.localScale.x, playerHurtBox.offset.y * player.localScale.y), Quaternion.identity);
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
        targets.Clear();
        Destroy(hitbox);
        Destroy(hitboxVisual);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // Sets the hitbox rotation relative to the mouse
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            hitboxRotation = Vector3.SignedAngle(new Vector3(1, 0), new Vector3(ray.origin.x, ray.origin.y) - player.position, axis);

            hitboxVisual.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);
            hitbox.transform.eulerAngles = new Vector3(0, 0, hitboxRotation);

            // Damages units in 'targets' on click
            if (Input.GetMouseButtonDown(0))
            {
                foreach (GameObject t in targets)
                {
                    t.GetComponent<Health>().TakeDamage(30, true);

                    // Call Knockback function
                    Knockback(t, 25f);
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

    // SniperHitbox.cs script attached to GameObject 'hitbox' will call these functions when entering or exiting range of a unit
    public void addTarget(GameObject t)
    {
        // Do not add the player to targets
        if (t != player.gameObject)
        {
            targets.Add(t);
        }
        else
        {
            Debug.Log("Adding " + t.gameObject.name + " denied.");
        }
    }

    public void removeTarget(GameObject t)
    {
        // Do not add the player to targets
        if (t != player.gameObject)
        {
            targets.Remove(t);
        }
        else
        {
            Debug.Log("Removing " + t.gameObject.name + " denied. (Should not be in target list)");
        }
    }
}
