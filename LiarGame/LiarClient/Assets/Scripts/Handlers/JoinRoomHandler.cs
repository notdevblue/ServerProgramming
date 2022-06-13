using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomHandler : MonoBehaviour
{
   Flag _flag = new Flag();

   public GameObject connectionPannel;
   public GameObject roomPannel;

   private List<UserData> userData;

   private void Awake()
   {
      BufferHandler.Instance.AddHandler("joinroom", data => {
         JoinRoomVO vo = JsonUtility.FromJson<JoinRoomVO>(data);
         userData = vo.userData;
         _flag.Set();
      });

      StartCoroutine(Join());
   }

   private IEnumerator Join()
   {
      while(true)
      {
         yield return new WaitUntil(_flag.Get);

         roomPannel.SetActive(true);
         connectionPannel.SetActive(false);

         userData.ForEach(e => {
            Room.Instance.AddUser(e.nickname, e.id);
         });
      }
   }
}