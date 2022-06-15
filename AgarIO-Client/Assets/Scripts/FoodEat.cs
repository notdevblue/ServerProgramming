using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodEat : MonoBehaviour
{
   public float SizeIncrease;
   public Text scoreBoard;
   int Score = 0;

   void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.tag == "Food")
      {
         transform.localScale += new Vector3(SizeIncrease, SizeIncrease, SizeIncrease);
         Score += 10;
         WebsocketClient.GetInstance().SendEatFood(Int32.Parse(other.gameObject.name), transform.localScale.x);
      }

      if (other.gameObject.CompareTag("Virus"))
      {
         Vector3 mass = transform.localScale;
         if (mass.x < 2.0f) return;

         transform.localScale = new Vector3(mass.x / 2, mass.y / 2, mass.z / 2);
         GameObject g= Instantiate(gameObject, transform.parent);
         g.transform.position = new Vector3(transform.position.x + mass.x / 2, transform.position.y + mass.x / 2, 0f);
         WebsocketClient.GetInstance().SendEatVirus(int.Parse(other.gameObject.name), transform.localScale.x);
         other.gameObject.SetActive(false);
      }
   }

   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.CompareTag("Player"))
      {
         if (transform.localScale.x < other.transform.localScale.x)
            transform.parent.gameObject.SetActive(false);
         else if (transform.localScale.x > other.transform.localScale.x)
            other.transform.parent.gameObject.SetActive(false);
         else
            return;

         transform.localScale += new Vector3(SizeIncrease, SizeIncrease, SizeIncrease);

         WebsocketClient.GetInstance().SendCollision(
            other.transform.parent.GetComponent<Movement>().ownerId,
            transform.parent.GetComponent<Movement>().ownerId
         );
      }
   }
}
