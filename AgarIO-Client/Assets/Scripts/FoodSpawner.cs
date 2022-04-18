using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
   public GameObject Food;
   public float Speed = 1.0f;

   private void Start()
   {
      InvokeRepeating(nameof(FoodGenerate), 0.0f, Speed);
   }

   private void FoodGenerate()
   {
      int x = Random.Range(0, Camera.main.pixelWidth);
      int y = Random.Range(0, Camera.main.pixelHeight);

      Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0.0f));
      target.z = 0.0f;
      Instantiate(Food, target, Quaternion.identity);
   }
}
