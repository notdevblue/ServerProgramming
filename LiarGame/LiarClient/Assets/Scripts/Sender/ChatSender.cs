using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatSender : MonoBehaviour
{
   public TMP_InputField chatInput = null;

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Return)) {
         WebSocketClient.Instance.Send("chat",
            JsonUtility.ToJson(new UserData(chatInput.text, WebSocketClient.Instance.id)));

         chatInput.text = "";
      }
   }
}