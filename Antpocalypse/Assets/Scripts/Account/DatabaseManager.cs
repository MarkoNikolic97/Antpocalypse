using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public static class DatabaseManager
{
    static float BaseLevelXP;   // Base levelXP
    static float A;             // Vertical scaling
    static float expBase;       // Exponential Base
    static float c;             // Horizontal scaling
    static float h;             // Horizontal Translation

    private static AccountData AccountData;

    public static AccountData Account { get => AccountData;}

    public static bool IsLoggedIn()
    {
        if(AccountData != null)
            return true;
        return false;

    }

    public static void LogOut()
    {
        AccountData = null;
    }

    public static IEnumerator UPDATE_Account()
    {
        if (IsLoggedIn())
        {

            WWWForm form = new WWWForm();
            form.AddField("username", AccountData.AccountName);
            form.AddField("level", AccountData.AccountLevel);
            form.AddField("experience", AccountData.AccountXP);
            form.AddField("currency1", AccountData.Currency1);
            form.AddField("currency2", AccountData.Currency2);

            WWW www = new WWW("http://localhost/sqlconnect/update.php", form);
            yield return www;

            string returnCode = www.text;

            if (returnCode.Equals("0"))
            {
                Debug.Log("Sucessfully updated");
            }
            else
            {
                Debug.Log(returnCode);
            }

        }

    }

    public static IEnumerator REGISTER_Account(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW("http://localhost/sqlconnect/register.php", form);
        yield return www;

        string returnCode = www.text; // 1 - Connection failed; 2 - Name check query failed; 3 - Name already exists; 4 - Insert account failed

        if (returnCode.Equals("0")) // No Error occured
        {
            Debug.Log("User created succesfully");
        }
        else
        {
            Debug.Log("Creation failed. Error Code: " + returnCode);
        }


    }

    public static IEnumerator LOGIN_Account(string username, string password, GameObject ErrorOutput)
    {
       


        ErrorOutput.SetActive(false);

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW("http://localhost/sqlconnect/login.php", form);
        yield return www;

       

        char returnCode = www.text[0]; // 5 - Name failed; 6 - Password incorrect

        //Return structure split by tab:   returnCode, level, exp, currency1, currency2 
        if (returnCode.Equals('0'))
        {
            string[] data = www.text.Split('\t');
            int level = int.Parse(data[1]);
            int xp = int.Parse(data[2]);
            int curr1 = int.Parse(data[3]);
            int curr2 = int.Parse(data[4]);

            Debug.Log(xp);

            AccountData = new AccountData(username, level, xp, curr1, curr2);
            GameObject.FindGameObjectWithTag("MenuController").GetComponent<MenuController>().LoadMainMenu();

        }
        else
        {

            ErrorOutput.GetComponent<TextMeshProUGUI>().text = returnCode.ToString();
            ErrorOutput.SetActive(true);
        }

       

    }

    public static void LoginDemoAccount()
    {
        AccountData = new AccountData("Demo Profile", 30, 100, 100, 100);
        GameObject.FindGameObjectWithTag("MenuController").GetComponent<MenuController>().LoadMainMenu();
    }

    public static int GetNeededXP(int level)
    {
        return Mathf.RoundToInt(A * Mathf.Pow((level / c) - h, expBase) + BaseLevelXP);
    }


    public static void PullGlobalAccountData()
    {
        Debug.Log("Not Implemented");

        BaseLevelXP = 100;
        A = 0.1f;
        expBase = 1.6f;
        c = 1;
        h = 0;
    }

}
