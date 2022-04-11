using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text.RegularExpressions;

public class Login : MonoBehaviour
{
   private string PASSWORD_REGEX = "(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.{8,})";



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

      if (username.Length < 3 || username.Length > 24)
      {
         Debug.Log("Invalid username");
         yield break;
      }

      if (!Regex.IsMatch(password, PASSWORD_REGEX))
      {
         Debug.Log("Invalid password");
         yield break;
      }


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
         LoginResponse response = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);

         if (response.code == 0) // login success
         {
            Debug.Log("Login Success");
            // text rendering
         }
         else
         {
            // Invalid Credentials.
            switch (response.code)
            {
               case 1:
                  Debug.Log("Invalid Credentials");
                  break;
               default:

                  break;
            }
         }
      }
      else
      {
         Debug.LogError("Failed to connect server");
         Debug.LogError(req.error);
      }

      // Debug.Log($"{username}:{password}");

   }

   private IEnumerator TryCrate()
   {
      string username = _usernameInput.text;
      string password = _passwordInput.text;
      // string url =
      //          $"{_createEndPoint}?rUsername={username}&rPassword={password}";

      if (username.Length < 3 || username.Length > 24)
      {
         Debug.Log("Invalid username");
         yield break;
      }

      if (!Regex.IsMatch(password, PASSWORD_REGEX))
      {
         Debug.Log("Invalid password");
         yield break;
      }

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
         CreateResponse response = JsonUtility.FromJson<CreateResponse>(req.downloadHandler.text);

         if (response.code == 0) // Create success
         {
            Debug.Log("Create Success");
            // text rendering
         }
         else // Create fail
         {
            // Invalid Credentials.
            switch (response.code)
            {
               case 1:
                  Debug.Log("Invalid Credentials");
                  break;
               case 2:
                  Debug.Log("Username is already taken");
                  break;
               default:
                  break;
            }
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
