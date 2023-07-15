using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100.0f;

    public void decreaseHealth(float amt)
    {
        maxHealth -= amt;
        if(maxHealth<=0)
        {
            Destroy(gameObject);
        }
        return;
    }
}
