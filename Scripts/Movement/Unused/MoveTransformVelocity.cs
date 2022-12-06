using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTransformVelocity : MonoBehaviour, IMoveVelocity
{
    [SerializeField] private float moveSpeed;

    private Vector3 velocityVector;    
    private void Awake() {

    }

    public void Start() {
        if (GetComponent<Unit>() != null)
        {
            moveSpeed = GetComponent<Unit>().speed;
        }
    }
    public void SetVelocity(Vector3 velocityVector) { // Interface method
        this.velocityVector = velocityVector;
    }

    private void Update() {
        transform.position += velocityVector * moveSpeed * Time.deltaTime; // Update movement in transform
    }
}
