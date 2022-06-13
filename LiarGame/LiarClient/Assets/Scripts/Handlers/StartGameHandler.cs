using System.Collections;
using UnityEngine;

public class StartGameHandler : MonoBehaviour
{
   public GameObject chatPannel = null;
   public GameObject lobbyPannel = null;

   public ChatHandler chatHandler = null;

   Flag _flag = new Flag();

   private void Awake()
   {

      BufferHandler.Instance.AddHandler("startgame", data => {
         StartGameVO vo = JsonUtility.FromJson<StartGameVO>(data);
         Game.Instance.isLiar = vo.isLiar;
         Game.Instance.topic  = vo.topic;
         Game.Instance.answer = vo.answer;

         _flag.Set();
      });

      StartCoroutine(Started());
   }


   IEnumerator Started()
   {
      while(true)
      {
         yield return new WaitUntil(_flag.Get);
         chatPannel.SetActive(true);
         lobbyPannel.SetActive(false);

         string text = Game.Instance.isLiar ? "당신은 라이어입니다." : "당신은 시민입니다.";
         chatHandler.AddChat(Color.black, text);

         text = Game.Instance.isLiar ? ("단어 힌트는 " + Game.Instance.topic + " 입니다.")
                                     : ("비밀단어는 " + Game.Instance.answer + " 입니다.");
         chatHandler.AddChat(Color.black, text);
      }
   }
}