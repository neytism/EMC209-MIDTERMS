using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpRequests : MonoBehaviour
{
    private const string MAIN = "http://localhost:3000";
    private const string USERS = "/users";
    private const string LEADERBOARD = "/leaderboard";
    private const string ME = "/users/me";
    private const string LOGIN = "/auth/login";
    private const string DEATHS = "/combat/deaths";
    private const string KILLS = "/combat/kills";

    private List<HttpUserData> users;

    public IEnumerator TryTokenLogin(string token, Action<HttpUserData> successAction)
    {
        var request = UnityWebRequest.Get(MAIN + ME);
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return request.SendWebRequest();
                
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
        else
        {
            HttpUserData user = JsonConvert.DeserializeObject<HttpUserData>(request.downloadHandler.text);
            successAction?.Invoke(user);
        }
    }

    public IEnumerator TryLogin(string username, string password, Action<string> failedAction, Action<HttpUserData> successAction, Action<string> newTokenAction)
    {
        //string usernamePassword = $"{{\"username\": \"{username}\", \"password\": \"{password}\"}}";
        var authentication = new
        {
            username = username,
            password = password
        };
        
        string usernamePassword = JsonConvert.SerializeObject(authentication);
        
        Debug.Log(usernamePassword);
        
        var request = UnityWebRequest.Post(MAIN + LOGIN, usernamePassword, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
        else
        {

            if (request.downloadHandler.text.Equals("no_users"))
            {
                failedAction?.Invoke("Invalid username or password!");
            }
            else
            {
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(request.downloadHandler.text);

                string token = tokenResponse.token;
                
                //Debug.Log(token);
                newTokenAction?.Invoke(token);
                
                request = UnityWebRequest.Get(MAIN + ME);
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                yield return request.SendWebRequest();
                
                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
                }
                else
                {
                    HttpUserData user = JsonConvert.DeserializeObject<HttpUserData>(request.downloadHandler.text);
                    successAction?.Invoke(user);
                }
            }

        }
        
    }
    
    public IEnumerator TryRegister(string username, string email, string password, Action<string> failedAction, Action<HttpUserData> successAction, Action<string> newTokenAction)
    {
        var newUser = new
        {
            username = username,
            email = email,
            password = password
        };
        
        string newUserSerialized = JsonConvert.SerializeObject(newUser);
        
        var request = UnityWebRequest.Post(MAIN + USERS, newUserSerialized, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            if (request.responseCode == 405) failedAction?.Invoke(request.downloadHandler.text);
            if (request.responseCode == 406) failedAction?.Invoke(request.downloadHandler.text);
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
        else
        {
            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(request.downloadHandler.text);

            string token = tokenResponse.token;
                
            //Debug.Log(token);
            newTokenAction?.Invoke(token);
                
            request = UnityWebRequest.Get(MAIN + ME);
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();
                
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
            }
            else
            {
                HttpUserData user = JsonConvert.DeserializeObject<HttpUserData>(request.downloadHandler.text);
                successAction?.Invoke(user);
            }
        
        }
        
    }

    public IEnumerator TryGetAllUsers(Action<List<HttpUserData>> successAction)
    {
        var request = UnityWebRequest.Get(MAIN + USERS);
        string token = "";
        if (PlayerPrefs.HasKey("token"))
        {
            token = PlayerPrefs.GetString("token");
        }
        else
        {
            yield break;
        }
        
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return request.SendWebRequest();
                
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
        else
        {
            List<HttpUserData> users = JsonConvert.DeserializeObject<List<HttpUserData>>(request.downloadHandler.text);
            successAction?.Invoke(users);
        }
    }
    
    public IEnumerator TryGetSpecificUser(string username ,Action<HttpUserData> successAction, Action<string> failedAction)
    {
        var request = UnityWebRequest.Get(MAIN + USERS + "/" + username);
        string token = "";
        if (PlayerPrefs.HasKey("token"))
        {
            token = PlayerPrefs.GetString("token");
        }
        else
        {
            yield break;
        }
        
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return request.SendWebRequest();
                
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
        else
        {
            if (request.downloadHandler.text.Equals("no_user"))
            {
                failedAction?.Invoke("No Users Found");
                yield break;
            }
            
            HttpUserData user = JsonConvert.DeserializeObject<HttpUserData>(request.downloadHandler.text);
            successAction?.Invoke(user);
        }
    }
    
    public IEnumerator TryGetTopTen(Action<List<HttpUserData>> successAction)
    {
        var request = UnityWebRequest.Get(MAIN + LEADERBOARD);
        string token = "";
        if (PlayerPrefs.HasKey("token"))
        {
            token = PlayerPrefs.GetString("token");
        }
        else
        {
            yield break;
        }
        
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return request.SendWebRequest();
                
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
        else
        {
            List<HttpUserData> users = JsonConvert.DeserializeObject<List<HttpUserData>>(request.downloadHandler.text);
            successAction?.Invoke(users);
        }
    }

    public IEnumerator TryIncreaseDeaths(int idDead)
    {
        var increaseResponse = new
        {
            id = idDead,
            increase = 1
        };

        string body = JsonConvert.SerializeObject(increaseResponse);
        
        var request = UnityWebRequest.Put(MAIN + DEATHS, body);
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
    }
    
    public IEnumerator TryIncreaseKills(int idKiller)
    {
        var increaseResponse = new
        {
            id = idKiller,
            increase = 1
        };
        
        string body = JsonConvert.SerializeObject(increaseResponse);

        var request = UnityWebRequest.Put(MAIN + KILLS, body);
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
    }
    public IEnumerator TryDeleteAccount(Action successAction)
    {
        var request = UnityWebRequest.Delete(MAIN + USERS);
        string token = "";
        
        if (PlayerPrefs.HasKey("token"))
        {
            token = PlayerPrefs.GetString("token");
        }
        else
        {
            yield break;
        }
        
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return request.SendWebRequest();
                
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
        }
        else
        {
            successAction?.Invoke();
        }
    }
}

[Serializable]
public struct HttpUserData
{
    public int id;
    public string username; 
    public string email;
    public string password;
    public int kills;
    public int deaths;
    public string ratio;
}

[Serializable]
public class TokenResponse
{
    public string token;
}
