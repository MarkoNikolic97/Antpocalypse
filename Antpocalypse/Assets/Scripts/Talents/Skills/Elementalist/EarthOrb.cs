using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthOrb : MonoBehaviour
{

    public float damage;
    public EarthOrbTalent talent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damagableObject = collision.GetComponent<IDamageable>();
        if (damagableObject != null) // this object can be damaged
        {
            bool destroyed;
            damagableObject.TakeDamage(damage, out destroyed);
            if (damagableObject is Enemy)
            {              
                if (destroyed)
                    talent.TotalEnemiesKilled++;
            }
            else // else it is a structure
            {
                if (destroyed)
                    talent.TotalStructuresDestroyed++;
            }


            
            //Debug.Log("EarthOrb has damaged: " + collision.gameObject.name);
        }
    }


}
