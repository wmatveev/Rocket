using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    public float currentHealth;
    [SerializeField] private GameObject healtBar;
    private float healthBarScale = 1f, currentHealthBarScale = 1f;
    [SerializeField] private float delta;
    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        if (healthBarScale < currentHealthBarScale)
        {
            currentHealthBarScale -= delta;
        }
        if (healthBarScale > currentHealthBarScale)
        {
            currentHealthBarScale += delta;
        }
        healtBar.transform.localScale = new Vector2(currentHealthBarScale, 1);
    }
    private void OnEnable()
    {
        ResetHealth();
    }
    public void TakeHit(int damage)
    {
        currentHealth -= damage;
        healthBarScale = currentHealth / health;
    }

    public void SetHealth(int bonusHealth)
    {
        currentHealth += bonusHealth;
        // if (currentHealth > health) {
        //     currentHealth = health;
        // }
    }

    public void ResetHealth()
    {
        currentHealth = health;
        healthBarScale = 1f;
        currentHealthBarScale = 1f;
    }
}
