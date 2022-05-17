using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandoClass : Class
{
    // Commando Ult Properties

    public CommandoClass()
    {
        Name = "Commando";
        Description = "Military specialist. Brandishes bombs and a blunderbuss!";

    }
    public CommandoClass(PlayerModel p)
    {
        playerModel = p;
        projectile = Resources.Load("Projectile") as GameObject;
        // SET PROJECTILE SPRITE TO CORRECT SPRITE FOR THIS CLASS

        Name = "Commando";
        Description = "Military specialist";

        movementSpeed = 10f;

        maxHP = 100;
        healthRegenPerSec = 0f;

        autoattackCooldown = 2f;
        autoAttackNumber = 0;
        autoattackDamage = 20f;

        sizeScale = 1f;

        attackModifier = 1f;
        damageResistance = 0f;


        EXPmodifier = 1f;
        pickupRadius = 1f;

        skillTalentFrequency = 3;

        SetDefaultClassStats(p);   // Apply all default stats to the Player Model


        Debug.Log(Name + " is initialized");

    }

    public override void AutoAttack(Vector3 target)
    {
        base.AutoAttack(target);
    }

    public override IEnumerator DoubleClick(Vector2 position)
    {
        return base.DoubleClick(position);
    }

    public override void Update()
    {
        base.Update();
        
    }

    public override void SetDefaultClassStats(PlayerModel model)
    {
        base.SetDefaultClassStats(model);

        // Set Default Ult PRoperties
    }

    public override GameObject GetUltUIResource()
    {
        return Resources.Load("Commando/UltCommandoUI") as GameObject;
    }

    public override void SetUltUIObject(GameObject uiObj)
    {
        base.SetUltUIObject(uiObj);

        // Set other References to Ult UI Object
    }
}
