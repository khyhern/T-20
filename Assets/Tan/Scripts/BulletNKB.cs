using UnityEngine;

public class BulletNKB : MonoBehaviour
{

    public GameObject hitEffect;

    public int damage;


    // Add a public method to set bullet damage
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    void OnCollisionEnter2D(Collision2D collision)     //Triggers when collision occurs
    {
        //For damaging enemies use this void collsion thing
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if (enemy != null)                  //If collided object contains an EnemyAI.cs
            enemy.TakeDamage(damage);       //Apply damage to enemy


        //Quaternion = default rotation/no rotation 
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);    //play effect at current position of bullet
        Destroy(gameObject);
        Destroy(effect, 1f);        //Hit effect destruction delayed by 1 seconds  
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); // Destroy Game Object when it leaves camera view
    }
}