using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

      IEnumerator e = leaderBorad.content.transform.GetEnumerator();
      // transform 이 순회가능한 타입으로 되어 있음. GetEnumerator(); 가 가능

      WebsocketClient.GetInstance().playerPool
         .OrderByDescending(i => i.Value.score)
         .ToList().ForEach(f => {
            e.MoveNext();
            Transform t = (Transform)e.Current;
            t.gameObject.GetComponent<Text>().text = f + ". " + f.Value.nickname + " : " + f.Value.score;
         });
   }
}
