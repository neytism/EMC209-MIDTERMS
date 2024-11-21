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
    [SerializeField] private GameObject _userContainerPrefab;
    [SerializeField] private Transform _userUIParent;
    private const string MAIN = "http://localhost:3000";
    private const string USERS = "/users";
    private const string LEADERBOARD = "/leaderboard";
    private const string USER_CURRENT = "/users/current";
    private const string LOGIN = "/users/login";
    private const string CHECK_USERNAME = "/users/checkUsername";
    private const string CHECK_EMAIL = "/users/checkEmail";
    private const string REGISTER = "/users/register";
    private const string UPDATE_PASSWORD = "/users/updatePassword";
    private const string DEATHS = "/users/deaths";
    private const string KILLS = "/users/kills";

    public RawImage _imageToReplace;
    public GameObject _usersContainer;

    private HttpData data;
    
    private List<HttpUserData> users;

    private IEnumerator Start()
    {
        //yield return HttpGetRequestData(MAIN + USERS);
        
        //yield return HttpDeleteRequest(MAIN + REG + "/4");
        
        //yield return HttpPutRequest(MAIN + REG + "/4", putData);
        
        //yield return HttpPostRequest(MAIN + REG, userData);
        
        //yield return HttpGetRequest(MAIN + USERS);
        
        //yield return GetTextureFomUri(imageUri);
        
        yield break;
    }

    public IEnumerator HttpGetRequestDataAfterLogin()
    {
        _usersContainer.SetActive(true);
        yield return HttpGetRequestData(MAIN + USERS);
    }
    
    private IEnumerator HttpGetRequestData(string uri)
    {
        var request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);

            //data.userData = new List<HttpUserData>();

            Debug.Log(request.downloadHandler.text);

            users = JsonConvert.DeserializeObject<List<HttpUserData>>(request.downloadHandler.text);
            
            foreach (var user in users)
            {

                //UIUserContainer userContainer = Instantiate(_userContainerPrefab, _userUIParent).GetComponent<UIUserContainer>();
                
                //userContainer.SetUpUserContainer(user.username, user.email);
                    
                
            }
        }
    }

    private IEnumerator HttpGetRequest(string uri)
    {
        var request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }
    
    
    private IEnumerator HttpPostRequest(string uri, string data)
    {
        var request = UnityWebRequest.Post(uri, data, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);
            //var auth = JsonConvert.DeserializeObject<Authentication>(request.downloadHandler.text);
            //Debug.Log(auth.token);
        }
    }
    
    private IEnumerator HttpPutRequest(string uri, string data)
    {
        var request = UnityWebRequest.Post(uri, data, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }
    
    private IEnumerator HttpDeleteRequest(string uri)
    {
        var request = UnityWebRequest.Delete(uri);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("User Deleted");
            
        }
    }
    
    private IEnumerator GetTextureFomUri(string uri, Action<Texture> callback)
    {
        Texture texture = null;
        var request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            texture = DownloadHandlerTexture.GetContent(request);
            callback?.Invoke(texture);
            
        }

    }

    public IEnumerator TryTokenLogin(string token, Action<HttpUserData> successAction)
    {
        var request = UnityWebRequest.Get(MAIN + USER_CURRENT);
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return request.SendWebRequest();
                
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
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
            Debug.LogError(request.error);
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
                
                request = UnityWebRequest.Get(MAIN + USER_CURRENT);
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                yield return request.SendWebRequest();
                
                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    HttpUserData user = JsonConvert.DeserializeObject<HttpUserData>(request.downloadHandler.text);
                    successAction?.Invoke(user);
                }
            }
            
            // var user = JsonConvert.DeserializeObject<HttpUserData>(request.downloadHandler.text);
            // successAction?.Invoke(user);
            
            
        
        }
        
    }
    
    public IEnumerator TryRegister(string username, string email, string password, Action<string> failedAction, Action<HttpUserData> successAction, Action<string> newTokenAction)
    {
        //check usernam
        var request = UnityWebRequest.Get(MAIN + CHECK_USERNAME + "/" + username);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            if (request.downloadHandler.text == "username_taken")
            {
                failedAction?.Invoke("Username Taken");
                yield break;
                //username used
            }
        }
        
        //check email
        request = UnityWebRequest.Get(MAIN + CHECK_EMAIL + "/" + email);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            if (request.downloadHandler.text == "email_taken")
            {
                failedAction?.Invoke("Email is already used by other user.");
                yield break;
                //Email used
            }
        }
        
        //string usernamePassword = $"{{\"username\": \"{username}\", \"password\": \"{password}\"}}";
        var newUser = new
        {
            username = username,
            email = email,
            password = password
        };
        
        string newUserSerialized = JsonConvert.SerializeObject(newUser);
        
        request = UnityWebRequest.Post(MAIN + REGISTER, newUserSerialized, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(request.downloadHandler.text);

            string token = tokenResponse.token;
                
            //Debug.Log(token);
            newTokenAction?.Invoke(token);
                
            request = UnityWebRequest.Get(MAIN + USER_CURRENT);
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();
                
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
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
            Debug.LogError(request.error);
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
            Debug.LogError(request.error);
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
            Debug.LogError(request.error);
        }
        else
        {
            List<HttpUserData> users = JsonConvert.DeserializeObject<List<HttpUserData>>(request.downloadHandler.text);
            successAction?.Invoke(users);
        }
    }

    public IEnumerator TryIncreaseDeaths(int idDead)
    {
        var request = UnityWebRequest.Get(MAIN + DEATHS + "/" + idDead);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
    }
    
    public IEnumerator TryIncreaseKills(int idKiller)
    {
        var request = UnityWebRequest.Get(MAIN + KILLS + "/" + idKiller);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
    }
    
    public IEnumerator TryDeleteUser(HttpUserData userData, Action successAction)
    {
        var request = UnityWebRequest.Delete(MAIN + USERS + "/" + userData.id);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            successAction?.Invoke();
            
        }
    }
    
    public IEnumerator TryUpdatePassword(int id, string newPassword, Action successAction)
    {
        var user = new
        {
            id = id,
            password = newPassword
        };
        
        string ser = JsonConvert.SerializeObject(user);

        var request = UnityWebRequest.Post(MAIN + UPDATE_PASSWORD,ser, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            successAction?.Invoke();
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
            Debug.LogError(request.error);
        }
        else
        {
            successAction?.Invoke();
        }
    }
}

[Serializable]
public struct Authentication
{
    public string username;
    public string password;
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
public struct HttpData
{
    public string page;
    public string perPage;
    public string total;
    public string totalPages;
    public List<HttpUserData> data;
}

[Serializable]
public class TokenResponse
{
    public string token;
}