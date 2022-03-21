using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketTalker : MonoBehaviour
{
    private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>(); // Concurrent 는 Thread Safe 함
    // 동시에 접근할때 dequeue 보다 enqueue 가 먼저 일어남 (보장됨)

    const string URL = "ws://ggmhan.herokuapp.com/";
    // const string URL = "ws://127.0.0.1:48000";
    private WebSocket ws;

    public GameObject cubeObj;

    private void Start() {
        ws = new WebSocket(URL);

        ws.OnMessage += (sender, e) => {
            if(!string.IsNullOrEmpty(e.Data)) _queue.Enqueue(GenerateCube);

            Debug.Log($"Message received from: {(sender as WebSocket).Url}\r\nData: {e.Data}");
            
        };

        ws.Connect();
    }

    private void Update() {
        if(ws == null) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.A)) {
            Debug.Log("Connection closed at client");
            ws.Close();
        }

        if(Input.GetKeyDown(KeyCode.C)) {
            if(ws.IsAlive) return;
            Debug.Log("Connection established");
            ws.Connect();
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            ws.Send("Hello");
        }


        while (_queue.Count > 0) {
            if(_queue.TryDequeue(out var e)) {
                e?.Invoke();
            } else {
                break;
            }
        }
    }

    float x = 0;
    private void GenerateCube()
    {
        Instantiate(cubeObj, new Vector3(x += 0.5f, 0.0f, 0.0f), Quaternion.identity).GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 10.0f, UnityEngine.Random.Range(-1.0f, 1.0f)), ForceMode.Impulse);
        Debug.Log("Generated");
    }


}
