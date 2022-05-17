using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class
{
    public string Name, Description;

    [Header("Stats")]
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

    public float EXPmodifier = 1f;
    public float pickupRadius;

    public int skillTalentFrequency;
    public float ultCooldown;

    protected GameObject autoAttackSprite;
    protected PlayerModel playerModel;

    protected GameObject projectile;
    protected GameObject ultUIObject;

    int totalEnemiesKilled_AutoAttack = 0;

    public int TotalEnemiesKilled_AutoAttack { get => totalEnemiesKilled_AutoAttack; set => totalEnemiesKilled_AutoAttack = value; }

    public virtual IEnumerator DoubleClick(Vector2 position) { yield return null; }

    public virtual void AutoAttack(Vector3 target)
    {
        GameObject po = GameObject.Instantiate(projectile, playerModel.transform.position, Quaternion.identity);
        Projectile p = po.GetComponent<Projectile>();
        p.InitProjectile(playerModel, target, 1000f, this);
    }

    public virtual void Update() { }

    public virtual void SetDefaultClassStats(PlayerModel model)
    {

        model.movementSpeed = movementSpeed;

        model.autoattackCooldown = autoattackCooldown;
        model.autoAttackNumber = autoAttackNumber;

        model.maxHP = maxHP;
        model.healthRegenPerSec = healthRegenPerSec;

        model.sizeScale = sizeScale;

        model.attackModifier = attackModifier;
        model.damageResistance = damageResistance;
        model.autoattackDamage = autoattackDamage;

        model.EXPmodifier = EXPmodifier;
        model.pickupRadius = pickupRadius;

    }


    public virtual GameObject GetUltUIResource() { return null; }
    public virtual void SetUltUIObject(GameObject uiObj)
    {
        ultUIObject = uiObj;
    }
}
