using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 100f;
    public string target;
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(target))
        {
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            enemyHealth.decreaseHealth(damage);
        }
    }
    void Update()
    {

    }
}