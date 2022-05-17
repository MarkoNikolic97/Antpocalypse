using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthOrbTalent : Talent
{
    public struct Orb
    {
        public GameObject orb;
        public float angle;
    }

    [Header("Skill Specific Properties")]
    public string OrbObject_Path = "Elementalist/EarthBall";

    [Header("Rank 1")]
    public int nOrbs;
    public float damage;
    public float range;
    public float speed;

    [Header("Rank 2")]
    public int nOrbs_rank2;
    public float damage_rank2;
    public float range_rank2;
    public float speed_rank2;

    [Header("Rank 3")]
    public int nOrbs_rank3;
    public float damage_rank3;
    public float range_rank3;
    public float speed_rank3;

    [Header("Rank 4")]
    public int nOrbs_rank4;
    public float damage_rank4;
    public float range_rank4;
    public float speed_rank4;

    [Header("Rank 5")]
    public int nOrbs_rank5;
    public float damage_rank5;
    public float range_rank5;
    public float speed_rank5;

    int nOrbsActual;
    float damageActual, rangeActual, speedActual;

    Orb[] orbs;

    int currentlyInitializedTalenLevel = 0;
    private GameObject OrbObject;
    // Start is called before the first frame update
    void Start()
    {
        OrbObject = Resources.Load<GameObject>(OrbObject_Path);
    }

    // Update is called once per frame
    void Update()
    {
        // Handle orb spinning
        if (orbs == null)
            return;


        
        for (int i = 0; i < orbs.Length; i++)
        {
            orbs[i].angle += speed * Time.deltaTime;
            orbs[i].orb.transform.position = GetOrbPosition(orbs[i].angle, rangeActual);
        }

        //Debug.Log("EarthOrb has killed" + TotalEnemiesKilled + "enemies");
    }

    public override void ApplyTalent(PlayerModel player)
    {
        base.ApplyTalent(player);

        switch (talentLevel)
        {
            case 1:
                nOrbsActual = nOrbs;
                damageActual = damage;
                rangeActual = range;
                speedActual = speed;
                break;
            case 2:
                nOrbsActual = nOrbs_rank2;
                damageActual = damage_rank2;
                rangeActual = range_rank2;
                speedActual = speed_rank2;
                break;
            case 3:
                nOrbsActual = nOrbs_rank3;
                damageActual = damage_rank3;
                rangeActual = range_rank3;
                speedActual = speed_rank3;
                break;
            case 4:
                nOrbsActual = nOrbs_rank4;
                damageActual = damage_rank4;
                rangeActual = range_rank4;
                speedActual = speed_rank4;
                break;
            case 5:
                nOrbsActual = nOrbs_rank5;
                damageActual = damage_rank5;
                rangeActual = range_rank5;
                speedActual = speed_rank5;
                break;

        }

        InitializeOrbs(talentLevel);
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public void InitializeOrbs(int talentLevel)
    {
        if (currentlyInitializedTalenLevel == talentLevel) // Nothing needs to be done because the Talent already operates at this level
            return;
        currentlyInitializedTalenLevel = talentLevel;

        if (orbs != null) // Destroy all orbs that already exist
        {
            for (int i = 0; i < orbs.Length; i++)
            {
                Destroy(orbs[i].orb);
            }
        }
        
        orbs = new Orb[nOrbsActual];

        float angleSpacing = (Mathf.PI * 2) / nOrbsActual;
        for (int i = 0; i < nOrbsActual; i++) // Initialize each orb with its starting angle
        {
            Orb newOrb = new Orb();
            newOrb.orb = Instantiate(OrbObject);

            float angle = angleSpacing * i;
            newOrb.angle = angle;
            newOrb.orb.transform.position = GetOrbPosition(angle, rangeActual);
            newOrb.orb.GetComponent<EarthOrb>().damage = damageActual;
            newOrb.orb.GetComponent<EarthOrb>().talent = this;

            orbs[i] = newOrb;
        }

    }


    Vector3 GetOrbPosition(float angle, float r) // r = radius/distance of orb from player
    {
        float x, y;

        x = Mathf.Cos(angle) * r;
        y = Mathf.Sin(angle) * r;

        Vector3 pos = new Vector3(x, y, 0) + playerModel.transform.position;

        return pos;
    }
}
