using System.Collections;
using UnityEngine;

public class JoinedHandler : MonoBehaviour
{
   private string joinedUserNickname;
   private int joinedUserId;

   Flag _flag = new Flag();

   private void Awake()
   {

      BufferHandler.Instance.AddHandler("joined", data => {
         UserData user = JsonUtility.FromJson<UserData>(data);
         joinedUserId = user.id;
         joinedUserNickname = user.nickname;

         if (user.id != WebSocketClient.Instance.id) {
            _flag.Set();
         }

      });

      StartCoroutine(Joined());
   }


   IEnumerator Joined()
   {
      while(true)
      {
         yield return new WaitUntil(_flag.Get);
         Room.Instance.AddUser(joinedUserNickname, joinedUserId);
      }
   }
}