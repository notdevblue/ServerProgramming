using UnityEngine;

public class InitHandler : MonoBehaviour
{
   private void Awake()
   {
      BufferHandler.Instance.AddHandler("init", data => {
         WebSocketClient.Instance.id = JsonUtility.FromJson<InitVO>(data).id;
      });
   }
}