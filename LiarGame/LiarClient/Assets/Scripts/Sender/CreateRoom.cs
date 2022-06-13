using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
   public Button btnCreateRoom;
   public InputField fieldNickname;

   private void Awake()
   {
      btnCreateRoom.onClick.AddListener(() => {
         WebSocketClient.Instance.Send("createroom", fieldNickname.text);
      });
   }
}