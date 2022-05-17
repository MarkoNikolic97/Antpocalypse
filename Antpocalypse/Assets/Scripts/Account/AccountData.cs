using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountData
{
    
    string accountName;

    int accountLevel;  
    int accountXP;
    string accountIcon;

    int currency1, currency2;

    public AccountData(string accountName, int accountLevel, int accountXP, int currency1, int currency2)
    {
       
        this.accountName = accountName;
        this.accountLevel = accountLevel;
        this.accountXP = accountXP;
        this.currency1 = currency1;
        this.currency2 = currency2;

        accountIcon = "default";

        //accountXP = 60;
    }

    public int AccountLevel { get => accountLevel; set => accountLevel = value; }
    public int AccountXP { get => accountXP; set => accountXP = value; }
    public string AccountIcon { get => accountIcon; set => accountIcon = value; }
    public string AccountName { get => accountName; set => accountName = value; }
    public int Currency1 { get => currency1; set => currency1 = value; }
    public int Currency2 { get => currency2; set => currency2 = value; }
}
