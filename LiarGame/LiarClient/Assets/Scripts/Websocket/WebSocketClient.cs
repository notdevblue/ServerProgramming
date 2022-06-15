using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


public class WebSocketClient : MonoBehaviour
{
   static public WebSocketClient Instance { get; set; }

   [Header("Server Address")]
   public string ipAddr;

   [Header("Server Port")]
   public string port;

   private WebSocket ws;

   public int id;

   public bool canSend = true;

   private void Awake()
   {
      Instance = this;

      ws = new WebSocket($"ws://{ipAddr}:{port}");

      ws.OnMessage += (sender, e) => {
         Debug.Log(e.Data);
         DataVO data = JsonUtility.FromJson<DataVO>(e.Data);
         BufferHandler.Instance.Handle(data.type, data.payload);
      };

      Connect();
   }

   public void Connect()
   {
      ws.Connect();
   }

   public void Disconnect()
   {
      ws.Close(CloseStatusCode.Normal, "Client disconnected");
   }

   public void Send(string type, string payload)
   {
      if (canSend)
         ws.Send(JsonUtility.ToJson(new DataVO(type, payload)));
   }


}