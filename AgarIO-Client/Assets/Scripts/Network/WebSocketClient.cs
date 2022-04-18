using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
   private WebSocket ws;
   private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

   private void Start()
   {
      ws = new WebSocket("ws://localhost:38000");
      ws.Connect();

      ws.OnMessage += (sender, e) => {
         if (!string.IsNullOrEmpty(e.Data))
         {
            _actions.Enqueue(GeneratePlayer);
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
   }

   private void GeneratePlayer()
   {

   }
}
