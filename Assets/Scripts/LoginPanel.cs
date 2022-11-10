using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class LoginPanel : MonoBehaviour
{
    [Header("Data")]
    public string url;
    public enum CurrentPanel { signIn = 0, reg = 1 };
    public CurrentPanel currentPanel;
    
    [Header("Registration panel")]
    public TMP_InputField regEmail;
    public TMP_InputField regPass1;
    public TMP_InputField regPass2;
    public TextMeshProUGUI regErr;
    [Header("SignIn panel")]
    public TMP_InputField signInEmail;
    public TMP_InputField signInPass;
    public TextMeshProUGUI signInErr;
    [Header("Windows")]
    public Windows regWindows;
    public Windows signInWindows;
    public TextMeshProUGUI currentPanelTitile;
    public KeyCode navigationKey;
    private int currentNav;
    public List<TMP_InputField> regAllField;
    public List<TMP_InputField> signInField;


    private void Start()
    {
        GetComponent<Windows>().Show();
        currentNav = 0;
        SelectInput();
    }

    private void Update()
    {
        if(Input.GetKeyDown(navigationKey))
        {
            currentNav++;
            SelectInput();
        }
    }

    private void SelectInput()
    {
        if (currentPanel == CurrentPanel.reg)
        {
            if (currentNav >= regAllField.Count) currentNav = 0;
            regAllField[currentNav].Select();
        }
        else
        {
            if (currentNav >= signInField.Count) currentNav = 0;
            signInField[currentNav].Select();
        }
    }
    public void OnShow()
    {
        ChangePanel();

    }

    public void ChangePanel()
    {
        if (currentPanel == CurrentPanel.signIn)
        {
            currentPanel = CurrentPanel.reg;            
            signInWindows.Hide();
            regWindows.Show();
            currentPanelTitile.text = regWindows.titile;
        }

        else
        {
            currentPanelTitile.text = signInWindows.titile;
            currentPanel = CurrentPanel.signIn;
            regWindows.Hide();
            signInWindows.Show();
        }
        currentNav = 0;
        SelectInput();
    }

    private string CheckError()
    {
        if (currentPanel == CurrentPanel.reg)    
        {
            if (String.IsNullOrWhiteSpace(regEmail.text) || String.IsNullOrWhiteSpace(regPass1.text) || String.IsNullOrWhiteSpace(regPass2.text)) return "Все поля должны быть заполнены.";
            if (regPass1.text != regPass2.text) return "Пароли не совпадают.";
            if(!regEmail.text.Contains("@")) return "Email введен не корректно";
            return null;
        }
        else  
        {
            if (String.IsNullOrWhiteSpace(signInEmail.text) || String.IsNullOrWhiteSpace(signInPass.text)) return "Все поля должны быть заполнены.";          
            if (!signInEmail.text.Contains("@")) return "Email введен не корректно.";
            return null;            
        }
    }
    private TextMeshProUGUI GetText()
    {
        if (currentPanel == CurrentPanel.reg) return regErr;
        else return signInErr;
    }

    public void TrySignIn()
    {
        
        if(CheckError() == null)
        {
            if (currentPanel == CurrentPanel.reg)
                StartCoroutine(RegCarutine());
            else
                StartCoroutine(SiginInCarutine());
        }
        else
        {
            GetText().text = CheckError();
        }
    }

    private IEnumerator RegCarutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "Registration");
        form.AddField("password", regPass1.text);
        form.AddField("email", regEmail.text);
        WWW www = new WWW(url, form);
        yield return www;
        if (www.text == "ok")
        {
            ChangePanel();
            signInEmail.text = regEmail.text;
            signInPass.text = regPass1.text;
            TrySignIn();
        }
        else regErr.text = www.text;
        Debug.Log(www.text);
        www.Dispose();
    }
    private IEnumerator SiginInCarutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "SingIn");
        form.AddField("email", signInEmail.text);
        form.AddField("password", signInPass.text);
        WWW www = new WWW(url, form);
        yield return www;
        Debug.Log(www.text);
        Data data = JsonUtility.FromJson<Data>(www.text);
        if(data.err != "ok") signInErr.text = data.err;
        else
        {
            UserData.email = data.email;
            UserData.login = data.login;
            UserData.balance = data.balance;
            SceneManager.LoadScene(1);
        }
        www.Dispose();
    }
}

public class Data
{
    public  string err;
    public  string login;
    public  string email;
    public  float balance;
}
