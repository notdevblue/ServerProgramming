using UnityEngine;
using UnityEngine.UI;

public class VoteIcon : MonoBehaviour
{
   public Button btnVote;
   public Text nickname;
   private int id;

   private VoteSender vs;

   private void Awake()
   {
      vs = FindObjectOfType<VoteSender>();
   }

   public void Init(int id, string nickname)
   {
      this.id = id;
      btnVote.onClick.AddListener(() => {
         vs.Vote(this.id);
      });

      this.nickname.text = nickname;
   }
}