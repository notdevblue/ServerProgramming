using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class WebsocketClient : MonoBehaviour
{
   private static WebsocketClient instance;
   public static WebsocketClient GetInstance()
   {
      if (instance == null)
         instance = new WebsocketClient();
      return instance;
   }

   WebSocket ws;
   private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
   public int clientId = -1;
   public Dictionary<int, Player> playerPool = new Dictionary<int, Player>();

   //server op code
   const int START_COUNTDOWN_OP_CODE = 101;
   const int MOVE_PLAYER_OP_CODE = 102;
   const int JOIN_PLAYER_OP_CODE = 103;
   const int ADD_FOOD_OP_CODE = 105;
   const int ADD_VIRUS_OP_CODE = 106;
   const int GAMEOVER_OP_CODE = 110;

   //client op code
   const int SCENE_READY_OP_CODE = 200;
   const int EAT_FOOD_OP_CODE = 202;
   const int EAT_VIRUS_OP_CODE = 203;
   const int PING_CHECK_OP_CODE = 205;
   const int DEATH_OP_CODE = 204;

   public Action playerUpdateAction;

   public InputField inputNickname;
   public Text scoreText;

   public RectTransform gameOverPanel;

   void Awake()
   {
      instance = this;
   }

   // Start is called before the first frame update
   void Start()
   {
      ws = new WebSocket("ws://127.0.0.1:3003");
      ws.Connect();
      ws.OnMessage += (sender, e) =>
      {
         if (!string.IsNullOrEmpty(e.Data))
         {
            Message message = JsonUtility.FromJson<Message>(e.Data);
            switch (message.opCode)
            {
               case START_COUNTDOWN_OP_CODE:
                  _actions.Enqueue(() => Join(e.Data));
                  break;
               case JOIN_PLAYER_OP_CODE:
                  _actions.Enqueue(() => GeneratePlayer(e.Data));
                  break;
               case ADD_FOOD_OP_CODE:
                  _actions.Enqueue(() => GenerateFood(e.Data));
                  break;
               case MOVE_PLAYER_OP_CODE:
                  _actions.Enqueue(() => PlayerUpdate(e.Data));
                  break;
               case GAMEOVER_OP_CODE:
                  _actions.Enqueue(() => gameOverPanel.gameObject.SetActive(!gameOverPanel.gameObject.activeInHierarchy));
                  break;
            }

            // _actions.Enqueue(() => PongCheck(e.Data));
         }
         Debug.Log("Message Received From:" + ((WebSocket)sender).Url + ",Data:" + e.Data);
      };
      InvokeRepeating("PingCheck", 0, 3f);
   }

   // Update is called once per frame
   void Update()
   {
      if (ws == null)
      {
         return;
      }

      if (Input.GetKeyDown(KeyCode.Space))
      {
         Message msg = new Message();
         msg.opCode = SCENE_READY_OP_CODE;
         msg.nickname = inputNickname.text;
         inputNickname.gameObject.SetActive(false);

         ws.Send(JsonUtility.ToJson(msg));
      }
      // if (Input.GetKeyDown(KeyCode.A))
      // {
      //    Debug.Log("close");
      //    ws.Close();
      // }

      while (_actions.Count > 0)
      {
         if (_actions.TryDequeue(out var action))
         {
            action?.Invoke();
         }
      }
   }

   public GameObject playerObj;
   public GameObject foodObj;

   void Join(string data)
   {
      if (clientId >= 0) return;
      Message message = JsonUtility.FromJson<Message>(data);

      switch (message.opCode)
      {
         case START_COUNTDOWN_OP_CODE:
            clientId = message.socketId;
            break;
      }
   }

   void GeneratePlayer(string data)
   {
      if (clientId < 0) return;
      Message msg = JsonUtility.FromJson<Message>(data);
      Player p = msg.player;
      GameObject o = Instantiate(playerObj, new Vector3(p.posX, p.posY, 0), Quaternion.identity);
      o.GetComponent<Movement>().ownerId = p.owner;
      o.GetComponent<Movement>().nickname.text = p.nickname;

      playerPool.Add(p.owner, p);
      Renderer r = o.GetComponentInChildren<MeshRenderer>();
      r.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
      r.sharedMaterial.SetColor("_Color", new Color(p.r / 255f, p.g / 255f, p.b / 255f));

      Debug.Log("Generate Player");
   }

   public List<Food> foodList = null;
   void GenerateFood(string data)
   {
      Message msg = JsonUtility.FromJson<Message>(data);
      msg.foods.ForEach(f =>
      {
         GameObject o = Instantiate(foodObj, new Vector3(f.posX, f.posY, 0), Quaternion.identity);

         Renderer r = o.GetComponentInChildren<MeshRenderer>();
         r.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
         r.sharedMaterial.SetColor("_Color", new Color(f.r / 255f, f.g / 255f, f.b / 255f));
         foodList.Add(f);
         Debug.Log("Generate Food");
      });
   }

   public GameObject virusObj;
   public List<Virus> virusList = null;
   void Generatevirus()
   {

   }

   void PlayerUpdate(string data)
   {
      Message msg = JsonUtility.FromJson<Message>(data);
      playerPool[msg.socketId] = msg.player;
      msg.visibleCells.ForEach(cell =>
      {
         if (!cell.nickname.IsNullOrEmpty()) // 무조건 닉네임이 존재하기 때문
         {
            playerPool[cell.owner].targetX = cell.targetX;
            playerPool[cell.owner].targetY = cell.targetY;
            playerPool[cell.owner] = cell;
         }
         
      });

      List<Food> removeFoodList = foodList.Where(i => !msg.foods.Any(e => i.id == e.id)).ToList(); // 사라진 푸드
      List<Food> addFoodList = msg.foods.Where(i => !foodList.Any(e => i.id == e.id)).ToList(); // 새로 날아온 푸드

      addFoodList.ForEach(f =>
      {
         GameObject o = Instantiate(foodObj, new Vector3(f.posX, f.posY, 0), Quaternion.identity);
         o.gameObject.name = f.id.ToString();

         Renderer r = o.GetComponentInChildren<MeshRenderer>();
         r.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
         r.sharedMaterial.SetColor("_Color", new Color(f.r / 255f, f.g / 255f, f.b / 255f));
         foodList.Add(f);
         Debug.Log("Generate Food");
      }); // 유니티 2018 부터 포이치 가비지 발생 안 되게 업뎃됨 for 와 foreaetch 성능상 큰 차이 없다고 커뮤에서 그럼

      List<Virus> addVirusList = msg.virus.Where(i => !virusList.Any(e => i.id == e.id)).ToList();
      addVirusList.ForEach(v => {
         GameObject g = Instantiate(virusObj, new Vector3(v.posX, v.posY, 0.0f), Quaternion.identity);
         g.name = v.id.ToString();
         virusList.Add(v);
      });

      GameObject.FindGameObjectsWithTag("Food")
         .ToList<GameObject>()
         .Where(g => removeFoodList.Any(e => g.name.Equals(e.id.ToString())))
         .ToList().ForEach(f =>
         {
            f.SetActive(false);
         });

      scoreText.text = "Score: " + msg.player.score;

      playerUpdateAction?.Invoke();
      Debug.Log("getFromServer: " + msg.player.targetX);

      playerUpdateAction();
   }

   public void SendEatFood(int eatenFoodId, float mass)
   {
      Message msg = new Message();
      msg.socketId = clientId;
      msg.opCode = EAT_FOOD_OP_CODE;
      msg.eatenFoodId = eatenFoodId;
      playerPool[clientId].mass = mass;
      ws.Send(JsonUtility.ToJson(msg));
   }

   public void SendEatVirus(int eatenVirusId, float mass)
   {
      Message msg = new Message();
      msg.socketId = clientId;
      msg.opCode = EAT_VIRUS_OP_CODE;
      msg.eatenVirusId = eatenVirusId;
      playerPool[clientId].mass = mass;

      ws.Send(JsonUtility.ToJson(msg));
   }

   public void SendCollision(int p1, int p2)
   {
      Message msg = new Message();
      msg.socketId = clientId;
      msg.opCode = DEATH_OP_CODE;
      msg.collisions = new List<Player>
      {
         playerPool[p1],
         playerPool[p2]
      };

      Debug.Log("Send Collision: " + p1 + " / " + p2);
      ws.Send(JsonUtility.ToJson(msg));
   }

   public void GameOver()
   {
      gameOverPanel.gameObject.SetActive(!gameOverPanel.gameObject.activeInHierarchy);
      SceneManager.LoadScene(gameObject.scene.name);
   }

   public void SendUpdate(Vector3 target)
   {
      Message msg = new Message();
      msg.socketId = clientId;
      msg.opCode = MOVE_PLAYER_OP_CODE;
      msg.player = playerPool[clientId];
      msg.player.targetX = target.x;
      msg.player.targetY = target.y;
      msg.player.mass = playerPool[clientId].mass;

      ws.Send(JsonUtility.ToJson(msg));
   }


   public DateTime _time;
   public void PingCheck()
   {
      Message msg = new Message();
      msg.opCode = PING_CHECK_OP_CODE;
      msg.socketId = clientId;

      _time = DateTime.Now;
      Debug.Log("time:" + _time.ToString());

      ws.Send(JsonUtility.ToJson(msg));
   }
}