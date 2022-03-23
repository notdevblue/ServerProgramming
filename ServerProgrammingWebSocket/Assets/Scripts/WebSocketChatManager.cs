using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;
using TMPro;

public enum ChatType
{
   NORMAL = 0,
   PARTY,
   GUILD,
   WISPER,
   SYSTEM,
   END_OF_ENUM
}

struct Message
{
   public int chat;           // 채팅타입
   public string text;        // 내용
   public string id;          // 보낸 사람의 id
   public string target;      // 귓말 보낼 타겟 유저
   public string date;        // 쳇 보낸 시간

}

public class WebSocketChatManager : MonoBehaviour
{
   private const string ADDR   = "localhost";
   private const ushort PORT  = 48000;

   [SerializeField]  private TMP_InputField   _messageInput;  // 사용자가 입력한 메세지
   [SerializeField]  private GameObject       _chatLogPrefab;
   [SerializeField]  private Transform        _parentContent; // 부모의 Transform
                     private WebSocket        _connection;
                     private ChatType         _currentInputType; // 현재 선택한 대화 타입

   private readonly ConcurrentQueue<System.Action> _actions
               = new ConcurrentQueue<System.Action>();

   private List<ChatCell> _chatList;

   private string id = "test";

   private void Awake()
   {
      _currentInputType = ChatType.NORMAL;
      _chatList = new List<ChatCell>();

      _connection = new WebSocket($"ws://{ADDR}:{PORT}");
      _connection.OnMessage += (sender, e) =>
      {
         ChatType type = ChatType.NORMAL; // 서버에서 받아온 데이터로 변경 예졍

         if (!e.Data.IsNullOrEmpty())
            _actions.Enqueue(() => Show(type, ChatTypeToColor(type), e.Data));

         Debug.Log($"Message Received from {(sender as WebSocket).Url}: {e.Data}");
      };

      _connection.OnClose += (sender, e) =>
      {
         Debug.Log("Connection Closed");
      };

      _connection.Connect();
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Return))
      {
         Send(_messageInput.text);
      }

      while (_actions.Count > 0) 
      {
         if (_actions.TryDequeue(out var result))
         {
            result?.Invoke();
         }
         else
         {
            break;
         }
      }
   }

   // 메세지를 서버에게 보냄
   private void Send(string text)
   {
      if (_messageInput.text.Trim() == "")
         return;

      _messageInput.text = "";
      Debug.Log($"Sending \"{text}\" to server");

      Message message = new Message();
      if (text.Substring(0,2).Equals("/r")) // 귓속말인 경우     /r id 내용
      {
         string[] split = text.Split(' ');
         message.chat = 3;
         message.text = split[2];
         _connection.Send(JsonUtility.ToJson(message));
      }
      else // 일반체팅?
      {
         message.chat = 0;
         message.text = text;
         _connection.Send(JsonUtility.ToJson(message));
      }
      Show(_currentInputType, ChatTypeToColor(_currentInputType), text, false);
   }

   // 서버로부터 받은 메세지를 보여줌
   private void Show(ChatType type, Color color, string text, bool isJson = true)
   {
      GameObject messageObject = Instantiate(_chatLogPrefab, _parentContent);
      ChatCell cell = messageObject.GetComponent<ChatCell>();

      if(!isJson)
      {
         cell.SetChatCell(type, color, $"me: {text}");
         return;
      }

      Message jsonMsg = JsonUtility.FromJson<Message>(text);

      switch(jsonMsg.chat)
      {
         case (int)ChatType.NORMAL:
            text = jsonMsg.date = " [" + jsonMsg.id + "] " + jsonMsg.text;
            break;
         case (int)ChatType.WISPER:
            text = jsonMsg.date = " [" + jsonMsg.id + "] " + jsonMsg.text;
            color = Color.yellow;
            break;
         case (int)ChatType.SYSTEM:
            text = jsonMsg.date = "[SYSTEM] " + jsonMsg.text;
            color = Color.blue;
            break;
      }

      cell.SetChatCell(type, color, text);

      _chatList.Add(cell);
   }

   private Color ChatTypeToColor(ChatType type)
   {
      Color[] colors = new Color[5]
      {
         Color.white,
         Color.red,
         Color.green,
         Color.magenta,
         Color.yellow
      };

      return colors[(int)type];
   }
}