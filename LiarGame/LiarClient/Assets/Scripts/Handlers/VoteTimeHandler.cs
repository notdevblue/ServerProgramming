using UnityEngine;
using System.Collections;

public class VoteTimeHandler : MonoBehaviour
{
   public GameObject votePannel;
   public GameObject chatPannel;

   public Transform voteIconRoot;
   public GameObject voteIconPrefab;

   Flag _flag = new Flag();

   private void Awake()
   {
      BufferHandler.Instance.AddHandler("votetime", data => {
         _flag.Set();
      });

      StartCoroutine(GoVote());
   }

   IEnumerator GoVote()
   {
      while(true)
      {
         yield return new WaitUntil(_flag.Get);

         votePannel.SetActive(true);
         chatPannel.SetActive(false);

         for (int i = voteIconRoot.childCount - 1; i >= 0; --i)
         {
            Destroy(voteIconRoot.GetChild(i).gameObject);
         }

         Room.Instance.GetAllUser.ForEach(e => {
            VoteIcon icon = Instantiate(voteIconPrefab, voteIconRoot).GetComponent<VoteIcon>();
            icon.Init(e.id, e.nickname);
         });
      }
   }
}