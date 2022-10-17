using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVelocity : MonoBehaviour, IMoveVelocity
{
    [SerializeField] private float moveSpeed;

    private Vector3 velocityVector;
    private Rigidbody2D rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        if (GetComponent<Unit>() != null)
        {
            moveSpeed = GetComponent<Unit>().speed;
        }
    }

    public void SetVelocity(Vector3 velocityVector) { // Interface method
        this.velocityVector = velocityVector;
    }

    private void FixedUpdate() {
        rb.velocity = velocityVector * moveSpeed; // Set the velocity in Rigidbody2D
    }
}
