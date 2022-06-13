using UnityEngine;
using UnityEngine.UI;

public class JoinRoom : MonoBehaviour
{
   public Button btnConnect;
   public InputField fieldNickname;
   public InputField fieldRoomID;


   private void Awake()
   {
      btnConnect.onClick.AddListener(Join);
   }

   public void Join()
   {
      string nickname = fieldNickname.text;
      string payload = JsonUtility.ToJson(new UserData(nickname, int.Parse(fieldRoomID.text)));
      WebSocketClient.Instance.Send("joinroom", payload);
   }
}