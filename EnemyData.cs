using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public int health_points = 3;
    private ServerObjectManager serverObjectManager;
    private SpriteRenderer spriteRenderer;
    private bool canEnemyTakeDamage = true;



    void Start()
    {
        serverObjectManager = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void DecreaseEnemyHealthPoints(int amount)
    {
        health_points -= amount;
        if (health_points <= 0)
        {
            EnemyDeath();
        }
        StartCoroutine(ChangeColorAndEnableInvincibilityOnTakeDamage());
    }

    private void EnemyDeath()
    {
        serverObjectManager.DestroyObject(gameObject);
    }

    // Change color indicating a hit and make player invincible for 1 seconds
    IEnumerator ChangeColorAndEnableInvincibilityOnTakeDamage()
    {

        Color previousColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        canEnemyTakeDamage = false;
        yield return new WaitForSeconds(1.0f);
        spriteRenderer.color = previousColor;
        canEnemyTakeDamage = true;

    }


}
