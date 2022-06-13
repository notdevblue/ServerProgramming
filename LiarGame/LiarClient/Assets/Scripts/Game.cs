using System;

public class Game : Singleton<Game>
{
   public bool isLiar = false;
   public string topic = "";
   public string answer = "";
}