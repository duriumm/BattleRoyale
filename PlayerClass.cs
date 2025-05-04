using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    // Each class has its own attributes. Based on what class is chosen we will modify the players
    // speed, health, armor and other values with this script




    // Base values for any player class
    private float playerMovementSpeed = 8.0f; 
    private int playerHealthPoints = 3;
    public WeaponParent playerWeaponParent;

    public GameObject[] playerWeaponGameobjects;


    public enum ChosenClass
    {
        Warrior,
        Ranger,
        Mage
    };
    public ChosenClass chosenClass;

    void Start()
    {

    }
    // Set the weapon sprite you want to show when using this function
    // Child sprites will be hidden along with colliders etc
    public void SetPlayerWeaponSprites(string weaponSpriteToShow)
    {
        foreach (var gameObjectWeapon in playerWeaponGameobjects)
        {
            // Only deactivate weapons that are not our playin weapons
            if(gameObjectWeapon.name.ToLower() != weaponSpriteToShow.ToLower()) 
            {
                gameObjectWeapon.SetActive(false);
            }

        }
    }
    public void SetPlayerDataBasedOnClass(string chosenClassAsString)
    {
        var playerMovement = transform.parent.GetComponent<CharacterMovement>();
        var playerHealth = GetComponent<PlayerHealth>();
        var heartManager = GameObject.Find("HeartContainer").GetComponent<HeartManager>();

        switch (chosenClassAsString)
        {
            case "Warrior":
                print("Chosen Warrior class");
                chosenClass = ChosenClass.Warrior;
                playerMovementSpeed = 6.0f;
                playerMovement.originalMovementSpeed = playerMovementSpeed;
                playerMovement.moveSpeed = playerMovementSpeed;

                // TODO: set hp to normal 6 when done testing
                playerHealthPoints = 10;
                playerHealth.health_points = playerHealthPoints;
                heartManager.AddHeartIcons(playerHealthPoints);
                heartManager.AddHeartGameobjectsToHeartList();
                playerWeaponParent.isMeleeWeaponEquipped = true;
                SetPlayerWeaponSprites("playerSpear");
                break;

            case "Ranger":
                chosenClass = ChosenClass.Ranger;
                playerMovementSpeed = 7.0f;
                playerMovement.originalMovementSpeed = playerMovementSpeed;
                playerMovement.moveSpeed = playerMovementSpeed;

                playerHealthPoints = 3;
                playerHealth.health_points = playerHealthPoints;
                heartManager.AddHeartIcons(playerHealthPoints);
                heartManager.AddHeartGameobjectsToHeartList();
                playerWeaponParent.isMeleeWeaponEquipped = false;
                SetPlayerWeaponSprites("playerBow");
                break;

            case "Mage":
                chosenClass = ChosenClass.Mage;
                playerMovementSpeed = 8.0f;
                playerMovement.originalMovementSpeed = playerMovementSpeed;
                playerMovement.moveSpeed = playerMovementSpeed;

                playerHealthPoints = 2;
                playerHealth.health_points = playerHealthPoints;
                heartManager.AddHeartIcons(playerHealthPoints);
                heartManager.AddHeartGameobjectsToHeartList();
                playerWeaponParent.isMeleeWeaponEquipped = false;
                SetPlayerWeaponSprites("playerWand");
                break;

            default:
                chosenClass = ChosenClass.Warrior;
                break;
        }
    }
}
