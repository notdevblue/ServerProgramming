using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Login : MonoBehaviour
{
   [SerializeField] private string _loginEndPoint = "http://127.0.0.1:3003/login";
   [SerializeField] private string _createEndPoint = "http://127.0.0.1:3003/create";

   [SerializeField] private TMP_InputField _usernameInput;
   [SerializeField] private TMP_InputField _passwordInput;
   
   public void OnLoginClick()
   {
      StartCoroutine(TryLogin());
   }

   public void OnCreateClick()
   {
      StartCoroutine(TryCrate());
   }

   private IEnumerator TryLogin()
   {
      string username = _usernameInput.text;
      string password = _passwordInput.text;
      WWWForm form = new WWWForm();

      form.AddField("rUsername", username);
      form.AddField("rPassword", password);

      UnityWebRequest req = UnityWebRequest.Post(_loginEndPoint, form);

      
      Debug.Log($"Sending data to {_loginEndPoint}");

      var handler = req.SendWebRequest();

      float startTime = 0.0f;

      while (!handler.isDone)
      {
         startTime += Time.deltaTime;
         yield return null;
      }

      if (req.result == UnityWebRequest.Result.Success)
      {
         string result = req.downloadHandler.text;
         Debug.Log(result);

         if (result != "Invalid credentials") // login success
         {
            GameAccount reAccount = JsonUtility.FromJson<GameAccount>(result);
            // text rendering
         }
         else
         {
            // Invalid Credentials.
         }
      }
      else
      {
         Debug.LogError("Failed to connect server");
         Debug.LogError(req.error);
      }

      Debug.Log($"{username}:{password}");

   }

   private IEnumerator TryCrate()
   {
      string username = _usernameInput.text;
      string password = _passwordInput.text;
      // string url =
      //          $"{_createEndPoint}?rUsername={username}&rPassword={password}";

      WWWForm form = new WWWForm();
      form.AddField("rUsername", username);
      form.AddField("rPassword", password);

      // UnityWebRequest req = UnityWebRequest.Get(url);
      UnityWebRequest req = UnityWebRequest.Post(_createEndPoint, form);


      Debug.Log($"Sending data to {_createEndPoint}");

      var handler = req.SendWebRequest();

      float startTime = 0.0f;

      while (!handler.isDone)
      {
         startTime += Time.deltaTime;
         yield return null;
      }

      if (req.result == UnityWebRequest.Result.Success)
      {
         string result = req.downloadHandler.text;
         Debug.Log(result);

         if (result != "Invalid credentials") // login success
         {
            GameAccount reAccount = JsonUtility.FromJson<GameAccount>(result);
            // text rendering
         }
         else
         {
            // Invalid Credentials.
         }
      }
      else
      {
         Debug.LogError("Failed to connect server");
         Debug.LogError(req.error);
      }

      Debug.Log($"{username}:{password}");

   }

}
