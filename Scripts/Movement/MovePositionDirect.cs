using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePositionDirect : MonoBehaviour
{
    private Vector3 movePosition;
    public void SetMovePosition(Vector3 movePosition) {
        this.movePosition = movePosition;
    }

    private void Update() {
        Vector3 moveDir = (movePosition - transform.position).normalized;
        GetComponent<IMoveVelocity>().SetVelocity(moveDir); // Set the velocity in Interface IMoveVelocity
    }
}
