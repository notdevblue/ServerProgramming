using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAccount
{
   public string _id; // db 통해 자동 부여되는 아이디.
   public string username;
   public int adminFlag; // 어드민 계정인지 판단
}
