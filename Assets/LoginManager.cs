using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LoginManager : MonoBehaviour
{

    public static LoginManager instance;

    [Header("Вход в аккаунта")]
    public TMP_InputField signInEmail;
    public TMP_InputField signInPass;
    public TextMeshProUGUI signInErr;

    [Header("Регистрация аккаунта")]
    public TMP_InputField regEmail;
    public TMP_InputField regLogin;
    public TMP_InputField regPass1;
    public TMP_InputField regPass2;
    public TextMeshProUGUI regErr;
    [Header("Менеджер")]
    public string url;
    public Windows loadingPanel;

    private void Awake()
    {
        instance = this;
       
    }

    public void TryRegistration()
    {
        loadingPanel.Show();
        if(CheckRegistration())
        {
            StartCoroutine(RegCarutine());
        }        
    }
    public void TrySignIn()
    {
        loadingPanel.Show();
        if(CheckSignIn())
        {
            StartCoroutine(SignInCarutine());
        }
       
    }
    private IEnumerator RegCarutine()
    {
        
        WWWForm form = new WWWForm();
        form.AddField("action", "Registration");
        form.AddField("login", regLogin.text);
        form.AddField("password", regPass1.text);
        form.AddField("email", regEmail.text);               
        WWW www = new WWW(url, form);
        yield return www;
        loadingPanel.Hide();
        if(www.text !="ok")
        {
            regErr.text = www.text;
        }
        else
        {
            signInEmail.text = regEmail.text;
            signInPass.text = regPass1.text;
            TrySignIn();
        }
    }
    private IEnumerator SignInCarutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "SingIn");
        form.AddField("password", signInPass.text);
        form.AddField("email", signInEmail.text);
        WWW www = new WWW(url, form);
        yield return www;
        Analis(www.text);
        
    }

    private bool CheckRegistration()
    {
        if(String.IsNullOrWhiteSpace(regEmail.text)|| String.IsNullOrWhiteSpace(regLogin.text)|| String.IsNullOrWhiteSpace(regPass1.text)|| String.IsNullOrWhiteSpace(regPass2.text))
        {
            // одно из полей состоит из пробелов
            regErr.text = "Все поля надо заполнить";
            loadingPanel.Hide();
            return false;
        }
        if(!regEmail.text.Contains("@"))
        {
            // email не корректный
            regErr.text = "Не корректный email";
            loadingPanel.Hide();
            return false;
        }
        if(regPass1.text != regPass2.text)
        {
            // пароли не совпадают!
            regErr.text = "Пароли не совпали";
            loadingPanel.Hide();
            return false;
        }
        return true;
    }
    private bool CheckSignIn()
    {
        if(String.IsNullOrWhiteSpace(signInEmail.text)|| String.IsNullOrWhiteSpace(signInPass.text))
        {
            // не верный логин или пароль
            signInErr.text = "Не верный логин или пароль";
            loadingPanel.Hide();
            return false;
        }
        if(!signInEmail.text.Contains("@"))
        {
            // пользователь не найден
            signInErr.text = "Пользователь не найден";
            loadingPanel.Hide();
            return false;
        }
        return true;

    }

    private void Analis(string www)
    {
        data data = JsonUtility.FromJson<data>(www);
        if (data.err != "ok")
        {
            loadingPanel.Hide();
            regErr.text = data.err;
            signInErr.text = data.err;
        }
        else
        {
            UserData.email = data.email;
            UserData.balance = data.balance;
            UserData.login = data.login;
            
        }
    }
}


public class data
{
    public  string err;
    public  string login;
    public  string email;
    public  float balance;
}
