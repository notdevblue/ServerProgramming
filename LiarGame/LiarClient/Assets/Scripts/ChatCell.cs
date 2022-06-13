using UnityEngine.UI;
using UnityEngine;

public class ChatCell : MonoBehaviour
{
	public void Setup(Color color, string textData)
	{
		Text text = GetComponent<Text>();
		text.color = color;
		text.text = textData;
	}
}