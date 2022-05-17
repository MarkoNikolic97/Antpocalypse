using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandoUltTalent : Talent
{

    [Header("Commando Ult Talent Modifiers")]
    public float speed;
    public int charges;
    public float CDModReduction;

    
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
        CommandoClass pClass = player.GetComponent<PlayerModel>().GetPlayerClass() as CommandoClass;

        // Update Talent Properties
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }
}
