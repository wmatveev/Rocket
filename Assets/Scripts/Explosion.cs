using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Animator animator;
    void Start()
    {
        if (Random.value >= 0.5f)
        {
            if (Random.value >= 0.5f)
            {
                animator.SetInteger("type", 1);
            }
            else
            {
                animator.SetInteger("type", 2);
            }
        }
        else
        {
            if (Random.value >= 0.5f)
            {
                animator.SetInteger("type", 3);
            }
            else
            {
                animator.SetInteger("type", 4);
            }
        }
        Destroy(gameObject, 0.3f);
    }
}
