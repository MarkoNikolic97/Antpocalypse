using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    PlayerModel pModel;

    Vector3 target;
    float speed;
    public float lifeTime = 10f;
    Class sender;

    public void InitProjectile(PlayerModel p, Vector3 target, float speed, Class sender)
    {
        pModel = p;
        this.target = target;
        this.speed = speed;
        this.sender = sender;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }

        transform.up = (target - transform.position).normalized;
        Vector2 dir = (target - transform.position).normalized;
        GetComponent<Rigidbody2D>().AddForce(dir * speed);
    }

    float elapsedTime;
    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifeTime)
        {
            Destroy(gameObject);
            
        }
        
    }




    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Something was hit");
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Debug.Log("Enemy is Hit");
            //((IDamageable)enemy).TakeDamage(pModel.GetAttackDamage());
            bool destroyed;
            ((IDamageable)enemy).TakeDamage(pModel.GetAttackDamage(), out destroyed);
            if (destroyed)
            {
                sender.TotalEnemiesKilled_AutoAttack++;
            }

            Destroy(gameObject);
        }
        
    }

    public void SetProjectileSprite()
    {
        // NOT IMPLEMENTED
    }

}
