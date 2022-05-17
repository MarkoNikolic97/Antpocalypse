using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : MonoBehaviour, IDamageable
{
    HiveModel model;
    float health;

    public void TakeDamage(float damage, out bool destroyed)
    {
        destroyed = false;
        health -= damage;
        if (health <= 0)
        {
            destroyed = true;
            Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        health = 100;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        CompositeCollider2D collider = transform.parent.GetComponent<CompositeCollider2D>();
        Destroy(GetComponent<BoxCollider2D>());
        collider.GenerateGeometry();

        GameObject world = GameObject.FindGameObjectWithTag("WORLD");
        if (world != null)
        {
            model = world.GetComponent<WorldController>().hiveObject.GetComponent<HiveModel>();
            Vector2Int wallDataPosition = model.GetComponent<HiveController>().WorldToDataPosition(transform.position);

            model.RemoveObstacle(wallDataPosition);
        }
        

        

    }
}
