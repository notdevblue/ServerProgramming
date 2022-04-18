using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodEat : MonoBehaviour
{
   [SerializeField] private float _sizeIncrease = 0.1f;
   [SerializeField] private Text _scoreBoard;
   private int _score = 0;

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Food"))
      {
         transform.localScale += new Vector3(_sizeIncrease, _sizeIncrease, _sizeIncrease);
         _score += 10;
         _scoreBoard.text = $"Score: {_score}";
         Destroy(other.gameObject);
      }
   }
}
