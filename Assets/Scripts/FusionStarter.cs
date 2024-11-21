using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class FusionStarter : MonoBehaviour
{
    [SerializeField] private HttpRequests httpRequests;
    [SerializeField] private FusionBootstrap nds;
    [SerializeField] private FusionBootstrapDebugGUI ndsGUI;
    private UIManager _uiManager;
    private bool _isMultiplePeerMode;
    private string _clientCount;
    private string _authToken = "";

    public HttpUserData currentPlayerData;

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
        if (PlayerPrefs.HasKey("token"))
        {
            _authToken = PlayerPrefs.GetString("token");
            StartCoroutine(httpRequests.TryTokenLogin(_authToken, httpUserData =>
            {
                currentPlayerData = httpUserData;
                nds.Username = currentPlayerData.username;
                ndsGUI.isAttemptingToLogIn = false;
                ndsGUI.showError = false;
                ndsGUI.isLoggedIn = true;
                ndsGUI.isRegistering = false;
                ndsGUI.isLoggingIn = false;
            }));
        }
        else
        {
            Debug.Log("No token saved");
        }
    }

    private void Start()
    {
        _isMultiplePeerMode = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
        if (_clientCount == null) {
            _clientCount = "1";
        } else {
            _clientCount = System.Text.RegularExpressions.Regex.Replace(_clientCount, "[^0-9]", "");
        }
        _clientCount = nds.AutoClients.ToString();
    }

    private void OnEnable()
    {
        if (_uiManager != null)
        {
            _uiManager.OnStartSharedModeButtonEvent += StartSharedMode;
        }
        else
        {
            Debug.LogWarning("UI Manager Found");
        }

        ndsGUI.OnLogInEvent += CheckLogin;
        ndsGUI.OnRegisterEvent += CheckRegister;
        ndsGUI.OnLogoutEvent += Logout;
        _uiManager.OnDeleteAccountEvent += DeleteAccount;
    }

    private void OnDisable()
    {
        ndsGUI.OnLogInEvent -= CheckLogin;
        ndsGUI.OnRegisterEvent -= CheckRegister;
        ndsGUI.OnLogoutEvent -= Logout;
        _uiManager.OnDeleteAccountEvent -= DeleteAccount;
    }

    private void StartSharedMode()
    {
        if (_isMultiplePeerMode) {
            StartMultipleSharedClients(nds);
        } else {
            nds.StartSharedClient();
        }
    }
    
    private void StartMultipleSharedClients(FusionBootstrap nds) {
        int count;
        try {
            count = Convert.ToInt32(_clientCount);
        } catch {
            count = 0;
        }
        nds.StartMultipleSharedClients(count);
    }
    
    public void CheckRegister(string username, string email, string password, string repeatPassword)
    {
        ndsGUI.isAttemptingToLogIn = true;
        
        if (string.IsNullOrEmpty(username))
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = "Username Required";
            return;
        }
        
        if (string.IsNullOrEmpty(email))
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = "Email Required";
            return;
        }
        
        if (string.IsNullOrEmpty(password))
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = "Password Required";
            return;
        }
        
        if (string.IsNullOrEmpty(repeatPassword) || repeatPassword != password)
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = "Password did not match";
            return;
        }

        ndsGUI.showError = false;
        
        StartCoroutine(httpRequests.TryRegister(username, email, password, s =>
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = s;
        }  ,(httpUserData) =>
        {
            if (httpUserData.username == "")
            {
                ndsGUI.isAttemptingToLogIn = false;
                ndsGUI.showError = true;
                ndsGUI.errorMessage = "Invalid Username or password.";
            }
            else
            {
                //Debug.Log($"{httpUserData.id}, {httpUserData.username}, {httpUserData.email}, {httpUserData.password}, {httpUserData.kills}, {httpUserData.deaths}");
                currentPlayerData = httpUserData;
                ndsGUI.isAttemptingToLogIn = false;
                ndsGUI.showError = false;
                ndsGUI.isLoggedIn = true;
                ndsGUI.isRegistering = false;
                ndsGUI.isLoggingIn = false;
            }
            
        }, s =>
        {
            PlayerPrefs.SetString("token", s);
            _authToken = s;
            Debug.Log("New Token: " + _authToken);
        } ));
    }
    
    public void CheckLogin(string username, string password)
    {
        ndsGUI.isAttemptingToLogIn = true;
        if (string.IsNullOrEmpty(username))
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = "Username Required";
            return;
        }
        
        if (string.IsNullOrEmpty(password))
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = "Password Required";
            return;
        }

        ndsGUI.showError = false;
        
        StartCoroutine(httpRequests.TryLogin(username, password, s =>
        {
            ndsGUI.isAttemptingToLogIn = false;
            ndsGUI.showError = true;
            ndsGUI.errorMessage = s;
        }  ,(httpUserData) =>
        {
            if (httpUserData.username == "")
            {
                ndsGUI.isAttemptingToLogIn = false;
                ndsGUI.showError = true;
                ndsGUI.errorMessage = "Invalid Username or password.";
            }
            else
            {
                //Debug.Log($"{httpUserData.id}, {httpUserData.username}, {httpUserData.email}, {httpUserData.password}, {httpUserData.kills}, {httpUserData.deaths}");
                currentPlayerData = httpUserData;
                ndsGUI.isAttemptingToLogIn = false;
                ndsGUI.showError = false;
                ndsGUI.isLoggedIn = true;
                ndsGUI.isRegistering = false;
                ndsGUI.isLoggingIn = false;
            }
            
        }, s =>
        {
            PlayerPrefs.SetString("token", s);
            _authToken = s;
            Debug.Log("New Token: " + _authToken);
        } ));
        
    }

    private void Logout()
    {
        PlayerPrefs.DeleteKey("token");
    }

    private void DeleteAccount()
    {
        PlayerPrefs.DeleteKey("token");
        nds.ShutdownAll();
    }
    
}
