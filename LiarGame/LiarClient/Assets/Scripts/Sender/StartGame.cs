using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
   public Button btnStart;

   private void Awake()
   {
      btnStart.onClick.AddListener(() => {
         WebSocketClient.Instance.Send("startgame", "");
         btnStart.gameObject.SetActive(false);
      });
   }
}