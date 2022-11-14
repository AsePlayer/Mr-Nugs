using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{

     public Transform target;
     private Transform pivot;
 
     void Start() {
        target = GameObject.Find("Player").transform;
        pivot = new GameObject().transform;
        transform.parent = pivot;
     }
     
     void Update () {
         Vector3 v3Pos = Camera.main.WorldToScreenPoint(target.position);
         v3Pos = (Input.mousePosition - v3Pos);
         float angle = Mathf.Atan2(v3Pos.y, v3Pos.x) * Mathf.Rad2Deg;
         
         pivot.position = target.position;
         pivot.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
     }
}

