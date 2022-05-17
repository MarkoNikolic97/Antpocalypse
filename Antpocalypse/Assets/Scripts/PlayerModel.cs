using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    private Class playerClass;

    // RETRUN TO PROTECTED AFTER TESTING
    public int playerLevel;
    public float health;
    public float currentExp;

    [Header("Player Stats")]
    public float maxHP;
    public float healthRegenPerSec = 0f;

    public float autoattackCooldown;
    public int autoAttackNumber = 3;
    public float autoattackDamage;

    public float movementSpeed = 1f;
    public float sizeScale = 1f;

    public float attackModifier = 1f;
    public float damageResistance = 0f; 

    public float itemCooldown, itemRadius, itemDuration;

    public float EXPmodifier;
    public float pickupRadius;


    // --------------------------- //
    protected List<Talent> equippedTalents = new List<Talent>();

   // PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        //playerClass = new ElementalistClass(this);
       
        
    }

    // Update is called once per frame
    void Update()
    {
        playerClass.Update();
    }


    // Recalculates and applies all stats taking into consideration all TALENTS
    public void CalculateStats()
    {
        // Set Default stat values from class
        playerClass.SetDefaultClassStats(this);

            
        foreach (Talent talent in equippedTalents)
        {
            talent.ApplyTalent(this);
        }
        
        Debug.Log(equippedTalents.Count);

        currentExp = 0;
        health = maxHP;

    }







    public Class GetPlayerClass(){ return playerClass; }
    public void SetPlayerClass(Class pclass) { playerClass = pclass; }

    public float GetExpForLvlUp(int playerlvl){ return 100;  }

    public float GetHP()  { return health; }

    public float GetCurrentExp(){  return currentExp; }

    public int GetPlayerLvl() { return playerLevel;  }

    public void SetHealth(float amount)
    {
        if (amount > maxHP)
        {
            maxHP = amount;
            
        }
        health = amount;
    }



    public float GetAttackDamage()
    {

        return autoattackDamage * attackModifier;

    }

   



    public List<Talent> GetEquippedTalents()
    {
        return equippedTalents;
    }

}
