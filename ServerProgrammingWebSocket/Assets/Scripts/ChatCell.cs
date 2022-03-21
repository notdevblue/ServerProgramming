using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatCell : MonoBehaviour
{
   public ChatType chatType { private set; get; }

   public void SetChatCell(ChatType type, Color color, string message)
   {
      TextMeshProUGUI chatMessage = GetComponent<TextMeshProUGUI>();

      chatType = type;
      chatMessage.color = color;
      chatMessage.text = message;
   }
}
