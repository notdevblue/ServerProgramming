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
            //scoreBoard.text = "Score : " + Score;
            Destroy(other.gameObject);
        }
    }
}
