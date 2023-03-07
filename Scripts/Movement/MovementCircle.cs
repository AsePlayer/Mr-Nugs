using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCircle : MonoBehaviour
{
    private Rigidbody2D rb;
    Transform movementCircle; // The circle that the player can move around in
    Vector2 centerPosition; // Location of center of movementCircle
    
    // Vector2 offset; // Offset between center of movementCircle and player
    public float movementRadius; // The radius of the circle the player can move around in
    public float speed = 5f; // The speed the player moves at
    private bool isPlayer = false;
    [SerializeField] float moveSpeed = 3;

    public bool canMove = false; // Whether or not the player can move

    [SerializeField] private GameObject movementCirclePrefab;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (gameObject.name == "MrNugs")
        {
            isPlayer = true;
        }

        // Set moveSpeed to Unit's currentSpeed
        speed = gameObject.GetComponent<Unit>().currentSpeed;
    }

    void Update()
    {
        if(!canMove) return;

        // Get player input
        Vector2 movement;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Move player
        if (isPlayer)
        {
            rb.velocity = movement * moveSpeed;
        }
        
        // Player's new desired position
        Vector2 newPos = rb.position;

        // Check if new position is inside the oval
        float x = (newPos.x - centerPosition.x) / (movementRadius);
        float y = (newPos.y - centerPosition.y) / (movementRadius / 2);
        if (x * x + y * y > 1)
        {
            // If outside the oval, move the player back to the edge of the oval
            float angle = Mathf.Atan2(y, x);
            newPos = new Vector2(centerPosition.x + (movementRadius) * Mathf.Cos(angle), centerPosition.y + (movementRadius / 2) * Mathf.Sin(angle));
            /* Based on the equation of an oval (x/a)^2 + (y/b)^2 = 1, where a and b are the semi-major and semi-minor axes of the oval. 
            If the new position is outside the oval, it calculates the angle of the point and move the player back to the edge of the 
            oval by using the angle and the radius of the oval. */
        }

        // Update player position
        transform.position = newPos;
    }

    void OnDrawGizmos()
    {
        // Draw a line from the player to the center of the movement circle
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, centerPosition);
    }

    public void SetupCircle(Vector3 startPosition, float speed)
    {
        // Create movement circle
        movementCircle = Instantiate(movementCirclePrefab, startPosition + new Vector3(0, 0, -1.1f), Quaternion.identity).transform;

        // Get center of movementCircle
        centerPosition = transform.position;

        // Set movementCircle based on how fast a Unit is
        movementRadius = (speed * 2) / 2;   // Radius = Diameter / 2
        // movementCircle.localScale = new Vector3(movementRadius, movementRadius /*   / 2   */, 0) * 2;
        movementCircle.localScale = new Vector3(movementRadius, movementRadius / 2, 0) * 2;
        canMove = true;
    }

    public void UpdateCircle(float speed)
    {
        // Set movementCircle based on how fast a Unit is
        movementRadius = (speed * 2) / 2;   // Radius = Diameter / 2
        movementCircle.localScale = new Vector3(movementRadius, movementRadius /*   / 2   */, 0) * 2;
    }

    public void DestroyCircle()
    {
        canMove = false;
        Destroy(movementCircle.gameObject);
    }

}
