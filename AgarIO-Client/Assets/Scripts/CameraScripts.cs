using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScripts : MonoBehaviour
{
   public Transform player;
   
   void Update()
   {
      if (player != null)
         transform.position = new Vector3(player.position.x, player.position.y, -10.0f);
   }
}
