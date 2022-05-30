using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
   // public GameObject _camera;
   public float moveSpeed;
   public int ownerId;
   public Text nickname;

   Vector3 target;

   // Start is called before the first frame update
   void Start()
   {
      GetComponentInChildren<Canvas>().worldCamera = Camera.main;
      if (WebsocketClient.GetInstance().clientId == ownerId)
         Camera.main.GetComponent<CameraScripts>().player = this.transform;
      InvokeRepeating(nameof(SendTargetToServer), 0.0f, 0.5f);
      InvokeRepeating(nameof(CheckTarget), 0.0f, 1.0f);
   }

   // Update is called once per frame
   void Update()
   {
      // if (WebsocketClient.GetInstance().clientId != ownerId) return;
      //Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      // Vector3 target = CheckTarget();
      target.z = transform.position.z;

      transform.position = Vector3.Lerp(transform.position, target, moveSpeed * Time.deltaTime);
   }

   void SendTargetToServer()
   {
      if (WebsocketClient.GetInstance().clientId != ownerId) return;
      WebsocketClient.GetInstance().SendUpdate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
      Debug.Log("SendTargetToServer");
   }

   void CheckTarget()
   {
      float x = WebsocketClient.GetInstance().playerPool[ownerId].posX;
      float y = WebsocketClient.GetInstance().playerPool[ownerId].posY;
      
      target = new Vector3(x, y, 0);
      // return new Vector3(x, y, 0);
   }
}
