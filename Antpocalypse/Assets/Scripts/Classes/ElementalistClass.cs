using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementalistClass : Class
{
    public int totalEnemiesKilled_Blink, totalStructuresDestroyed_Blink;
    public int enemiesKilled_LastBlink, structuresDestroyed_LastBlink;
    public float totalDistanceCovered_Blink;

    // Elementalist Ult Properties
    public float blinkRange, explosionRadius, explosionDamage, explosionForce;
    public int maxUltCharges;

    int currentUltCharges;

    public ElementalistClass()
    {
        Name = "Elementalist";
        Description = "Master of Elements";
    }
    public ElementalistClass(PlayerModel p)
    {


        playerModel = p;
        projectile = Resources.Load("Projectile") as GameObject;
        // SET PROJECTILE SPRITE TO CORRECT SPRITE FOR THIS CLASS

        Name = "Elementalist";
        Description = "Master of Elements";

        movementSpeed = 10f;

        maxHP = 100;
        healthRegenPerSec = 0f;

        autoattackCooldown = 2f;
        autoAttackNumber = 1;
        autoattackDamage = 1;

        sizeScale = 1f;

        attackModifier = 1f;
        damageResistance = 0f;


        EXPmodifier = 1f;
        pickupRadius = 1f;

        skillTalentFrequency = 3;

        SetDefaultClassStats(p);   // Apply all default stats to the Player Model

        currentUltCharges = maxUltCharges;

        Debug.Log(Name + " is initialized");
    }

    public override void AutoAttack(Vector3 target)
    {
        base.AutoAttack(target);
    }



    public override IEnumerator DoubleClick(Vector2 position) // Playtest for TakeDamage changes
    {
        
        if (currentUltCharges > 0)
        {
            if (Vector3.Distance(playerModel.transform.position, position) <= blinkRange)
            {
                totalDistanceCovered_Blink += Vector3.Distance(playerModel.transform.position, position);
                playerModel.transform.position = position;
                currentUltCharges--;

                

                if (explosionForce > 0)
                {
                    Collider2D[] objectsInBlastRadius = Physics2D.OverlapCircleAll(playerModel.transform.position, explosionRadius);
                    int enemiesKilled = 0, structuresDestroyed = 0;

                    foreach (Collider2D collider in objectsInBlastRadius)
                    {
                        IDamageable dmgableObj = collider.GetComponent<IDamageable>();
                        Rigidbody2D physicsObj = collider.GetComponent<Rigidbody2D>();
                        if (dmgableObj != null) // Object can be damaged
                        {
                            bool destroyed;
                            if (dmgableObj is Enemy) // Blink did damage to an Enemy
                            {
                                dmgableObj.TakeDamage(explosionDamage, out destroyed);

                                Vector3 forceVector = (collider.transform.position - playerModel.transform.position).normalized;
                                ((Enemy)dmgableObj).ApplyForce(forceVector * explosionForce, ForceMode2D.Impulse);

                                if (destroyed)
                                    enemiesKilled++;


                            }
                            else // if it is not Enemy then it is a structure
                            {
                                dmgableObj.TakeDamage(explosionDamage, out destroyed);
                                if (destroyed)
                                    structuresDestroyed++;
                            }

                            

                            //dmgableObj.TakeDamage(explosionDamage);
                        }
                       
                    }
                    /////////////
                    enemiesKilled_LastBlink = enemiesKilled;
                    structuresDestroyed_LastBlink = structuresDestroyed;

                    totalEnemiesKilled_Blink += enemiesKilled;
                    totalStructuresDestroyed_Blink += structuresDestroyed;

                    Debug.Log("EnemiesKilledLastBlink: " + enemiesKilled_LastBlink);
                    Debug.Log("Total Enemies Killed Blink: " + totalEnemiesKilled_Blink);
                }
                Debug.Log("Total Distance Covered: " + totalDistanceCovered_Blink);
            }

            
            yield return null;


        }

        
        

    }

    GameObject UI_CooldownObj, UI_ChargesObj;
    float elapsedUltTime;
    public override void Update()
    {
        UI_ChargesObj.GetComponent<TextMeshProUGUI>().text = currentUltCharges.ToString();
        if (currentUltCharges < maxUltCharges)
        {
            elapsedUltTime += Time.deltaTime;
            UI_CooldownObj.GetComponent<Image>().fillAmount = elapsedUltTime / ultCooldown;
            if (elapsedUltTime >= ultCooldown) // One charge is reset
            {
                currentUltCharges++;
               

                elapsedUltTime = 0;
            }
        }
    }


    public override void SetDefaultClassStats(PlayerModel model)
    {
        base.SetDefaultClassStats(model);

        maxUltCharges = 1;
        ultCooldown = 5f;
        blinkRange = 10f;
        explosionRadius = 0f;
        explosionDamage = 0.5f;
        explosionForce = 0;
    }


    public override GameObject GetUltUIResource()
    {
        return Resources.Load("Elementalist/UltElementalistUI") as GameObject;
    }

    public override void SetUltUIObject(GameObject uiObj)
    {
        base.SetUltUIObject(uiObj);

        UI_CooldownObj = ultUIObject.transform.GetChild(0).gameObject;
        UI_ChargesObj = ultUIObject.transform.GetChild(2).gameObject;
    }



}
