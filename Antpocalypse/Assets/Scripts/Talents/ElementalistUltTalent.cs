using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalistUltTalent : Talent
{
    [Header("Elementalist Ult Talent Modifiers")]
    public float range;
    public int charges;
    public float CDModReduction;

    public float explosionForce;
    public float explosionDamage;
    public float explosionRadius;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public override void ApplyTalent(PlayerModel player)
    {
        ElementalistClass pClass = player.GetComponent<PlayerModel>().GetPlayerClass() as ElementalistClass;

        pClass.maxUltCharges = charges * talentLevel + 1;
        //pClass.maxUltCharges += charges;

        pClass.ultCooldown -= CDModReduction * talentLevel * pClass.ultCooldown;
        //pClass.ultCooldown -= CDModReduction;

        pClass.explosionForce += explosionForce * talentLevel;
        pClass.explosionRadius += explosionRadius * talentLevel / 2f;
        pClass.explosionDamage += explosionDamage * talentLevel;

        Debug.Log("Ult Cooldown is now: " + pClass.ultCooldown);
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }
}
