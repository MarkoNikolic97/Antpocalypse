using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float forceApplicationDuration = 3f;

    protected float maxHP;
    public float health;
    public float playerDamage;

    public bool controlEnabled = true;
    public GameObject playerFocus;
    // Start is called before the first frame update
    public virtual void Start()
    {
        health = maxHP;
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public void KillEnemy()
    {
        Destroy(gameObject);
    }

    public virtual IEnumerator CrowdControlled(float duration) { yield return null; }

    public void TakeDamage(float damage, out bool destroyed)
    {
        destroyed = false;
        health -= damage;
        if (health <= 0)
        {
            // Play anim or other needed things
            // Before destroying AntAgent remove it from AntAgents list as well as its Agent at agentIndex from HIVE simulation

            destroyed = true;
            Destroy(gameObject);
            
        }

        
    }

    public IEnumerator ApplyForce(Vector2 forceVector, ForceMode2D forceMode)
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            controlEnabled = false;

            rigidbody.AddForce(forceVector, forceMode);

            yield return new WaitForSeconds(forceApplicationDuration);

            controlEnabled = true;

        }
    }
}
