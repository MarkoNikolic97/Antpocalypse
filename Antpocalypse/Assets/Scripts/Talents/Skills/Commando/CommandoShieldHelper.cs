using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandoShieldHelper : MonoBehaviour
{
    public CommandoShield shield;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            StartCoroutine(shield.ShieldHit());
            bool destroyed;
            ((IDamageable)enemy).TakeDamage(0, out destroyed);
        }
    }
}
