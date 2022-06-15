using UnityEngine;
using System.Collections;

public class DropHandler : MonoBehaviour
{
   public GameObject isLiarPanel;
   public GameObject notLiarPanel;

   DropVO vo;
   Flag _flag = new Flag();

   private void Awake()
   {
      BufferHandler.Instance.AddHandler("drop", data => {
         vo = JsonUtility.FromJson<DropVO>(data);

         _flag.Set();
      });

      StartCoroutine(Drop());
   }


   IEnumerator Drop()
   {
      while (true)
      {
         yield return new WaitUntil(_flag.Get);

         if (vo.isLiar) {
            isLiarPanel.SetActive(true);
         } else {
            Room.Instance.DeleteUser(vo.id);
            if (vo.id == WebSocketClient.Instance.id) {
               WebSocketClient.Instance.canSend = false;
            }

            notLiarPanel.SetActive(true);
         }

      }
   }
}