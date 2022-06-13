using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
   public static Room Instance = null;

   public GameObject player;
   public GameObject mafia;

   public Transform imageRoot;

   [SerializeField]
   private List<UserData> usersOnRoom = new List<UserData>();

   private void Awake()
   {
      Instance = this;
   }

   public void AddUser(string nickname, int id)
   {
      usersOnRoom.Add(new UserData(nickname, id));

      Instantiate(player, imageRoot);
   }

   public UserData FindUser(int id)
   {
      return usersOnRoom.Find(x => x.id == id);
   }

   public void DeleteAll()
   {
      for (int i = imageRoot.childCount - 1; i >= 0; --i)
      {
         Destroy(imageRoot.GetChild(i).gameObject);
      }
   }
}