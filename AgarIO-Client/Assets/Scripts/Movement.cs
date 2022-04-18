using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
   public float moveSpeed = 3.0f;


   private void Update()
   {
      Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      target.z = transform.position.z;

      transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
   }
}
