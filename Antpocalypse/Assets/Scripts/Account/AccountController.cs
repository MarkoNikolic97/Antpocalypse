using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountController : MonoBehaviour
{

    /* This ToDo reffers to both AccountController and AccountDatabaseManager
     * 
     * To Do - before database implementation
     *  Check if Email is valid
     *  Double Password verification
     *  
     * 
     * To Do - Database implementation
     *  Check if Username or Email are taken
     *  Register new Account to database
     *  Pull existing account from database 
     *  Update existing account in database
     *  
     *  
     */


    AccountData account;
    public GameObject MenuController;
    public GameObject CreationPanel, LogInPanel, AccountPanel;

    public GameObject XPBar, AccountIcon, AccountName, Level;
    public GameObject Currency1Amount, Currency2Amount;
    public GameObject NewAccName, NewAccPass, NewAccRepeatPass, AccErrorReport;
    public GameObject LogInUsername, LogInPass, LogInErrorOutput;


    int usernameMinLenght = 8, passwordMinLenght = 8, usernameMaxLenght = 32;
    float updateAccountDelay = 0.5f;

    public void CreateNewAccount()
    {
        AccErrorReport.SetActive(false);

        string accName = NewAccName.GetComponent<InputField>().text;       
        string accPass = NewAccPass.GetComponent<InputField>().text;
        string accPassRepeat = NewAccRepeatPass.GetComponent<InputField>().text;

        // Check the validity of data
        if (accName.Length < usernameMinLenght || accName.Length > usernameMaxLenght) // Username validity
        {
            AccErrorReport.GetComponent<TextMeshProUGUI>().text = "Username must be between " + usernameMinLenght.ToString() + " and " + usernameMaxLenght.ToString() + " characters.";
            AccErrorReport.SetActive(true);
            return;
        }
        if (accPass.Length < passwordMinLenght) // Password lenght validity
        {
            AccErrorReport.GetComponent<TextMeshProUGUI>().text = "Password must be longer than " + passwordMinLenght + " characters.";
            AccErrorReport.SetActive(true);
            return;
        }
        if (!accPass.Equals(accPassRepeat)) // Password matching
        {
            AccErrorReport.GetComponent<TextMeshProUGUI>().text = "Passwords must match.";
            AccErrorReport.SetActive(true);
            return;
        }

        Debug.Log("Attempting to register user " + accName);

        StartCoroutine(DatabaseManager.REGISTER_Account(accName, accPass));


        CreationPanel.SetActive(false);
        LogInPanel.SetActive(true);
    }

    public void LogIn()
    {
        string username = LogInUsername.GetComponent<InputField>().text;
        string password = LogInPass.GetComponent<InputField>().text;

        if (username.Equals("Demo") || username.Equals("demo")) // Demo account should be logged in
        {
            DatabaseManager.LoginDemoAccount();
            return;
        }

        StartCoroutine(DatabaseManager.LOGIN_Account(username, password, LogInErrorOutput));

        
    }
    public void SignUp()
    {
        LogInPanel.SetActive(false);
        CreationPanel.SetActive(true);

    }

    public void UpdateAccountDataToUI() // Implement: If currentXp > xpToNextLvl  --->  currentXP -= xpToNextLvl, curentLvl++
    {
        if (!DatabaseManager.IsLoggedIn())
        {             
            return;
        }

        AccountPanel.SetActive(true);
        LogInPanel.SetActive(false);


        int currentLvl = DatabaseManager.Account.AccountLevel;
        int currentXP = DatabaseManager.Account.AccountXP;

        AccountName.GetComponent<TextMeshProUGUI>().text = DatabaseManager.Account.AccountName;
        Level.GetComponent<TextMeshProUGUI>().text = currentLvl.ToString();
        Currency1Amount.GetComponent<TextMeshProUGUI>().text = DatabaseManager.Account.Currency1.ToString();
        Currency2Amount.GetComponent<TextMeshProUGUI>().text = DatabaseManager.Account.Currency2.ToString();

        // Xp Bar Implementation
        int xpToNextLvl = DatabaseManager.GetNeededXP(currentLvl + 1);
        XPBar.GetComponent<Image>().fillAmount = (float)currentXP / (float)xpToNextLvl;
        

        

        // Account Icon implementation
    }

    // Start is called before the first frame update
    void Start()
    {
        DatabaseManager.PullGlobalAccountData();
    }


    float elapsedTime = 0;
    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= updateAccountDelay)
        {
            elapsedTime = 0;

            if (DatabaseManager.IsLoggedIn())
            {
                UpdateAccountDataToUI();
            }

            
        }



        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Pressed");
            StartCoroutine(DatabaseManager.UPDATE_Account());
        }
    }


    // Serialization and Database Fetching
   

   
}
