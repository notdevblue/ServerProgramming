using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;


struct Message
{
   public int opCode; // 이름이 기계어 상에서 명령어 의미한다고 함
}

struct Player
{
   public int owner; // 소유권 정보
   public int posX;
   public int posY;
   public byte r;
   public byte g;
   public byte b;
   public int cellType;
}

public class WebSocketClient : MonoBehaviour
{
   private WebSocket ws;
   private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

   public GameObject playerObj;

   // Server op code (서버 이벤트 처리)
   const int START_COUNTDOWN_OP_CODE = 101;
   const int MOVE_PLAYER_OP_CODE = 102;

   // Client op code (클라 이벤트 처리)
   const int SCENE_READY_OP_CODE = 200;

   private void Start()
   {
      ws = new WebSocket("ws://localhost:48000");
      ws.Connect();

      ws.OnMessage += (sender, e) => {
         if (!string.IsNullOrEmpty(e.Data))
         {
            _actions.Enqueue(() => GeneratePlayer(e.Data));
            Debug.Log($"Message received from: {((WebSocket)sender).Url}, Data: {e.Data}");
         }
      };
      
   }

   private void Update()
   {
      if (ws == null)
      {
         return;
      }

      if (Input.GetKeyDown(KeyCode.Space))
      {
         Message message = new Message();
         message.opCode = SCENE_READY_OP_CODE;

         ws.Send(JsonUtility.ToJson(message));
      }

      if (Input.GetKeyDown(KeyCode.Return))
      {
         ws.Close();
      }

      while (_actions.Count > 0)
      {
         if (_actions.TryDequeue(out var action))
         {
            action?.Invoke();
         }
      }

   }

   private void GeneratePlayer(string data)
   {
      Player p = JsonUtility.FromJson<Player>(data);
      Instantiate(playerObj, new Vector3(p.posX, p.posY, 0), Quaternion.identity);
      playerObj.GetComponentInChildren<MeshRenderer>().sharedMaterial.color = new Color32(p.r, p.g, p.b, 255);
      Debug.Log("Player Generated");
   }
}
