using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private Animator anim;
    public void Start()
    {
        anim = this.GetComponent<Animator>();
    }
    
    public void OnSlash()
    {
        Debug.Log("Slash");
        anim.SetTrigger("isAttacking");
    }
}
