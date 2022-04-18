using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
   public List<Material> mats = new List<Material>();

   private void Start()
   {
      GetComponent<Renderer>().material = mats[Random.Range(0, mats.Count)];
   }
}