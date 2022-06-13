using System.Collections;
using UnityEngine;

public class ChatHandler : MonoBehaviour
{
   public GameObject chatCell;
   public Transform textRoot;

   private UserData chatData;

   Flag _flag = new Flag();

   private void Awake()
   {
      BufferHandler.Instance.AddHandler("chat", data => {
         chatData = JsonUtility.FromJson<UserData>(data);

         _flag.Set();
      });

      StartCoroutine(Chat());
   }

   IEnumerator Chat()
   {
      while(true)
      {
         yield return new WaitUntil(_flag.Get);

         AddChat(Color.white, $"{Room.Instance.FindUser(chatData.id).nickname}: {chatData.nickname}");
      }
   }


   public void AddChat(Color color, string text)
   {
      ChatCell obj = Instantiate(chatCell, textRoot).GetComponent<ChatCell>();
      obj.Setup(color, text);
   }
}