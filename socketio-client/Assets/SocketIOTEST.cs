using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

public class SocketIOTEST : MonoBehaviour
{
   public SocketIOUnity socket;

   public InputField eventNameText;
   public InputField dataText;
   public Text receivedText;

   public GameObject cubeObject;

   void Start()
   {
      Uri uri = new Uri("http://127.0.0.1:3000"); // http 로 접근해도 socketio 내부에서 변환이 됨
      socket = new SocketIOUnity(uri, new SocketIOOptions { 
         Query = new Dictionary<string, string> {
            {"token", "UNITY"}
         },
         EIO = 4, // socketIO 버전
         Transport = SocketIOClient.Transport.TransportProtocol.WebSocket // 통신 규약 지정
      });

      socket.JsonSerializer = new NewtonsoftJsonSerializer();
      
      socket.OnConnected += (sender, e) => {
         Debug.Log("Connect success");
      };

      socket.OnDisconnected += (sender, e) => {
         Debug.Log("Disconnected");
      };

      socket.OnPing += (sender, e) => {
         Debug.Log("Ping");
      };

      socket.OnPong += (sender, e) => {
         Debug.Log("Pong: " + e.TotalMilliseconds);
      };



      socket.Connect();
   }

   public void EmitTest()
   {
      string eventName = "hello";
      string txt = "Install Gentoo";

      socket.Emit(eventName, txt);
   }


   private void OnApplicationQuit()
   {
      socket.Disconnect();
   }
}
