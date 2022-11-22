using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCircle : MonoBehaviour
{
    private Rigidbody2D rb;
    Transform movementCircle; // The circle that the player can move around in
    Vector2 centerPosition; // Location of center of movementCircle
    
    Vector2 offset; // Offset between center of movementCircle and player
    public float movementRadius; // The radius of the circle the player can move around in
    public float speed = 5f; // The speed the player moves at

    public bool canMove = false; // Whether or not the player can move

    [SerializeField] private GameObject movementCirclePrefab;

    void Start()
    {

    }

    void Update()
    {
        if(!canMove) return;

        // Move the object around with the arrow keys but confine it
        // to a given radius around a center point.

        // Get player input
        Vector2 movement;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Player's new desired position
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y) + movement * speed * Time.deltaTime;

        // Calculate the distance of the new position from the center point. Keep the direction
        // the same but clamp the length to the specified radius.
        // Source: https://docs.unity3d.com/ScriptReference/Vector3.ClampMagnitude.html
        offset = newPos - centerPosition;
        
        // Move the object to the new position.
        transform.position = centerPosition + Vector2.ClampMagnitude(offset, movementRadius);

    }

    void OnDrawGizmos()
    {
        // Draw a line from the player to the center of the movement circle
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, centerPosition);
    }

    public void SetupCircle(float speed)
    {
        // Create movement circle
        movementCircle = Instantiate(movementCirclePrefab, transform.position, Quaternion.identity).transform;

        // Get center of movementCircle
        centerPosition = transform.position;

        // Set movementCircle based on how fast a Unit is
        movementRadius = (speed * 2) / 2;   // Radius = Diameter / 2
        movementCircle.localScale = new Vector3(movementRadius, movementRadius, 0) * 2;
        canMove = true;
    }

    public void UpdateCircle(float speed)
    {
        // Set movementCircle based on how fast a Unit is
        movementRadius = (speed * 2) / 2;   // Radius = Diameter / 2
        movementCircle.localScale = new Vector3(movementRadius, movementRadius, 0) * 2;
    }

    public void DestroyCircle()
    {
        canMove = false;
        Destroy(movementCircle.gameObject);
    }

}
