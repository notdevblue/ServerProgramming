using UnityEngine;
using System.Collections;

public class LiarGameOverHandler : MonoBehaviour
{
   public GameObject liarWin;
   public GameObject liarLost;

   private LiarGameOverVO vo;

   Flag _flag = new Flag();

   private void Awake()
   {
      BufferHandler.Instance.AddHandler("liargameover", data => {
         vo = JsonUtility.FromJson<LiarGameOverVO>(data);
      });

      StartCoroutine(FinalCheck());
   }

   IEnumerator FinalCheck()
   {
      while(true)
      {
         yield return new WaitUntil(_flag.Get);

         switch (vo.result)
         {
            case true:
               liarWin.SetActive(true);
               break;
            case false:
               liarLost.SetActive(true);
               break;
         }
      }
   }
}