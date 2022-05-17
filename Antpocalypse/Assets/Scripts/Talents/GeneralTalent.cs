using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralTalent : Talent
{
    [Header("General Talent Stat modifiers")]
    public float maxHP_MOD = 0;
    public float healthRegenPerSec_MOD = 0;

    public float autoattackCooldown_MOD = 0;
    public int autoAttackNumber_MOD = 0;

    public float movementSpeed_MOD = 0;
    public float sizeScale_MOD = 0;

    public float attackModifier_MOD = 0;
    public float damageResistance_MOD = 0;

    public float itemCooldown_MOD, itemRadius_MOD, itemDuration_MOD;

    public float EXPmodifier_MOD = 0;
    public float pickupRadius_MOD = 0;


    public override void ApplyTalent(PlayerModel player)
    {
        // Change this formulas for optimal gameplay
        player.maxHP += (maxHP_MOD * talentLevel);
        player.healthRegenPerSec += (healthRegenPerSec_MOD * talentLevel);

        player.autoattackCooldown += autoattackCooldown_MOD * talentLevel;
        player.autoAttackNumber += autoAttackNumber_MOD * talentLevel;

        player.movementSpeed += movementSpeed_MOD * talentLevel;
        player.sizeScale += sizeScale_MOD;

        
    }

    public override void LevelUp()
    {
        base.LevelUp();

        /*
        if (talentLevel < maxLevel)
        {
            talentLevel++;
        }
        */
        /*/ Experiment to see if this is no longer needed. See TalentManager.GenerateTalentList
        if (talentLevel >= maxLevel)
        {
            minPopupChance = 0;
            maxPopupChance = 0;
        }
        */
    }
}
