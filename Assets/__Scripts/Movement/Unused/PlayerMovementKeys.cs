using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeys : MonoBehaviour
{
    float horizontal, vertical;
    private void Update() {

        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

        // To feel "right" when moving diagonally, we need to normalize
        Vector3 moveVector = new Vector3(horizontal, vertical).normalized;

        GetComponent<IMoveVelocity>().SetVelocity(moveVector); // Set the velocity in Interface IMoveVelocity
    }

}
