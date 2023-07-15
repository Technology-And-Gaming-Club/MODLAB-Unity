using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float damage = 100f;
    public string target;
    public Animator anim;
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag(target) && anim.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Attacking"))
        {
            Debug.Log("yay");
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            enemyHealth.decreaseHealth(damage);
        }
    }
    void Update()
    {
        
    }
}
