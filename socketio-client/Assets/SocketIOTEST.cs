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
      // Transport 옵션으로 WebSocket 넣었기 때문에
      // 프로토콜 업그레이드 자동 진행

      // Transport 옵션 없으면
      // 내부적으로 HTTP 폴링 방식으로 접속
      // 웹소켓 지원한다는걸 알게 되면 업그레이드 한 뒤 통신 유지
      Uri uri = new Uri("http://127.0.0.1:3000/my-namespace"); // http 로 접근해도 socketio 내부에서 변환이 됨
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

      socket.OnUnityThread("spin", data => { // 특정 이벤트 대해서
         cubeObject.transform.Rotate(0, 45, 0);
         // rotateAngle = 0;

         // 클라이언트에서 자연스러운 처리가 필요한게 아니면
         // 여기서 바로바로 가능
      });

      socket.OnAnyInUnityThread((name, response) => { // 모든 이벤트 대해서
         receivedText.text += "Received on " + name + ": " + response.GetValue().GetRawText() + "\r\n";
      });
   }

   public void EmitTest()
   {
      string eventName = "hello";
      string txt = "Install Gentoo";

      socket.Emit(eventName, txt);
   }

   public void EmitSpin()
   {
      socket.Emit("spin");
   }

   public void EmitClass()
   {
      TestStringClass testClass = new TestStringClass("GameServer프로그ram잉");
      socket.Emit("class", testClass);
   }

   float rotateAngle = 45;
   float MaxRotateAngle = 45;
   // private void Update()
   // {
   //    if (rotateAngle < MaxRotateAngle)
   //    {
   //       ++rotateAngle;
   //       cubeObject.transform.Rotate(0, 1, 0);
   //    }
   // }

   [Serializable]
   class TestStringClass
   {
      public string txt;
      public TestStringClass(string txt)
      {
         this.txt = txt;
      }
   }

   private void OnApplicationQuit()
   {
      socket.Disconnect();
   }
}
