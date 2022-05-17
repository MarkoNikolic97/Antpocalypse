using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talent : MonoBehaviour
{
    protected PlayerModel playerModel;
    protected PlayerController playerControl;
   


    public int talentLevel;

    public string Name, Description;
    public int maxLevel;
    public float minPopupChance, maxPopupChance;

    int totalEnemiesKilled, totalStructuresDestroyed;
    int enemiesKilled_LastInstance, structuresDestroyed_LastInstance;

    public int TotalEnemiesKilled { get => totalEnemiesKilled; set => totalEnemiesKilled = value; }
    public int TotalStructuresDestroyed { get => totalStructuresDestroyed; set => totalStructuresDestroyed = value; }
    public int EnemiesKilled_LastInstance { get => enemiesKilled_LastInstance; set => enemiesKilled_LastInstance = value; }
    public int StructuresDestroyed_LastInstance { get => structuresDestroyed_LastInstance; set => structuresDestroyed_LastInstance = value; }

    public virtual void ApplyTalent(PlayerModel player)
    {
        playerModel = player;
        playerControl = player.GetComponent<PlayerController>();
    }

    public virtual void LevelUp()
    {
        if (talentLevel < maxLevel)
        {
            talentLevel++;
        }
    }

    // Popup chances depends on talentLevel and MaxTalentLevel

    public float GetMinChance()
    {
        return minPopupChance;
    }

    public float GetMaxChance()
    {
        return maxPopupChance;
    }

    public int GetCurrentTalentLevel()
    {
        return talentLevel;
    }

   
}
