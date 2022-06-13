using System;
using System.Collections.Generic;

[Serializable]
public class JoinRoomVO
{
   public List<UserData> userData;
}

[Serializable]
public class UserData
{
   public string nickname;
   public int id;

   public UserData(string nickname, int id)
   {
      this.nickname = nickname;
      this.id = id;
   }
}
