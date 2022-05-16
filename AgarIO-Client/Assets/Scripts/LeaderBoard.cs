using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
   public ScrollRect leaderBorad;
   public GameObject textObject;

   private void Start()
   {
      WebsocketClient.GetInstance().playerUpdateAction += UpdateLeaderBoard; // 원레는 동사가 앞으로 가고 뒤에 가고 통일 해야 함
   }

   void UpdateLeaderBoard()
   {
      if (WebsocketClient.GetInstance().playerPool.Count > leaderBorad.content.childCount)
      {
         Instantiate(textObject, leaderBorad.content);
      }
   }
}
