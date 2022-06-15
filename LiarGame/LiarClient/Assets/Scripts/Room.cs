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

   public int userCount => usersOnRoom.Count;
   public List<UserData> GetAllUser => usersOnRoom;

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

   public void ReDraw()
   {
      DeleteAllImage();
      usersOnRoom.ForEach(e => {
         Instantiate(player, imageRoot);
      });
   }

   public void DeleteUser(int id)
   {
      var target = usersOnRoom.Find(e => e.id == id);
      if (target != null)
      {
         usersOnRoom.Remove(target);
      }

      ReDraw();
   }

   public void DeleteAllImage()
   {
      for (int i = imageRoot.childCount - 1; i >= 0; --i)
      {
         Destroy(imageRoot.GetChild(i).gameObject);
      }
   }
}