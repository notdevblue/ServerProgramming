using UnityEngine;
using UnityEngine.UI;

public class AnswerSender : MonoBehaviour
{
   public InputField fieldAnswer = null;
   public Button btnSend;


   private void Awake()
   {
      btnSend.onClick.AddListener(() => {
         if (Game.Instance.isLiar)
            WebSocketClient.Instance.Send("answer", fieldAnswer.text);
      });
   }
}