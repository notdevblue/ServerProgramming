using UnityEngine;

public class VoteSender : MonoBehaviour
{
   public void Vote(int id)
   {
      WebSocketClient.Instance.Send("vote", JsonUtility.ToJson(new VoteVO(id)));
   }
}