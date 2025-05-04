using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using System;
using static UnityEditor.Rendering.CameraUI;


/// <summary>
/// This class is doing most of the networking for the player like movement and rotation.
/// </summary>
public class PlayerNetwork : NetworkBehaviour
{
    private CharacterMovement characterMovement;

    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;
    public GameObject projectileToFire;
    private ShootProjectile shootProjectile;

    private ServerObjectManager serverObjectManager;
    private MeleeHit meleeHit;

    private List<EnemyMovement> allEnemiesInScenesMovement;

    private SafeZone sceneSafeZone;


    public GameObject testSpawnObjectForServer;

    public GameObject[] listOfSpawnPoints;
    private Vector2 playerSpawnPosition;

    public GameObject[] prefabsToSpawnInScene;

    private CloudSave cloudSave;
    private LobbyManager lobbyManager;

    private PlayerClass playerClass;
    public string chosenPlayerClass = "";
    public string chosenPlayerWeapon = "";

    public GameObject playerBow;
    public GameObject playerWand;

    public List<GameObject> playersInSceneList = new List<GameObject>();


    // Network variable                                                   Everyone can read this variable,           Only the owner can write to it
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Creating this custom class can be good if we want to send lots of data when networking. We can only send this and not classes. Therefor we use a struct
    private NetworkVariable<MyCustomData> customData = new NetworkVariable<MyCustomData>(
        new MyCustomData
        {
            _int = 56,
            _bool = true,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message; // 50 chars with 128bytes

        // Use this to serialize the data you are sending (making it private instead of having it public)
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    // This is the "Awake" of networking. Whenever we (player) spawn this runs. We attach ourselves to the customData which we can then see when changed
    public override void OnNetworkSpawn()
    {


        customData.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };

        # region Server/Host spawning code
        if (IsServer)
        {
            print("WE ARE SERVER, activating all enemies in scene");
            // Activating all the enemies movement in the scene
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                print(enemy.name);
                enemy.GetComponent<EnemyMovement>().enabled = true;
                enemy.GetComponent<EnemyMovement>().AddPlayerToTransformsList(transform);

            }

            // FOR NOW WE DONT SPAWN EXIT PORTALS, HELPS DEBUGGING
            //// Spawn exit portals
            //foreach (var item in prefabsToSpawnInScene)
            //{
            //    GameObject go = Instantiate(item, Vector3.zero, Quaternion.identity);
            //    go.GetComponent<NetworkObject>().Spawn();
            //}
        }
        if(IsOwner)
        {
            // On spawn set MY spriterenderer + childs mask interaction to None since we want to see our sprite normally
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            foreach (SpriteRenderer r in gameObject.GetComponentsInChildren(typeof(SpriteRenderer)))
            {
                r.maskInteraction = SpriteMaskInteraction.None;
            }

            //Set this players playerdata to the safe zone so we only work with the local player

            // Add this player to the playerlist for safeZone script
            //sceneSafeZone.AddPlayerToPlayerList(gameObject);
            gameObject.name = "Player local owner";
            // Enable players own audiolistener since lobby camera liustener is disabled
            gameObject.GetComponent<AudioListener>().enabled = true; 

        }

        // Add every player that spawns in the network to this list
        playersInSceneList.Add(gameObject);

        // This triggers whenever someone else joined
        print("--- A new player or host joined! Could it be: " + OwnerClientId + " perhaps???");
        // TODO: Set all my spriterenderers to mask interaction none
        // TODO: set all other players sprite renderers to mask interaction visible inside mask
        foreach (var playerGameObject in playersInSceneList)
        {
            // Only set these values to OTHER joined players. Not your own
            if(playerGameObject.name != "Player local owner")
            {
                print("player gameobject is: " + playerGameObject.name);
                SpriteRenderer spriteRenderer = playerGameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                // Set the FOVcone gameobject inactive for OTHER players
                var children = playerGameObject.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    if (child.name == "FOVcone")
                    {
                        child.gameObject.SetActive(false);
                    }

                }
                // Set the maskInteraction to visibleinside mask for OTHER joined players
                foreach (SpriteRenderer r in playerGameObject.GetComponentsInChildren(typeof(SpriteRenderer)))
                {
                    r.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }
        }

        #endregion
    }



    // This start is only run for the client using it.
    private void Start()
    {
        if (!IsOwner) return;

        // Disable camera audio listener on join. Player has its own
        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = false;

        characterMovement = gameObject.transform.parent.GetComponent<CharacterMovement>();
        CinemachineVirtualCamera virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform; // Set this object to be followed by your camera on joining the game

        serverObjectManager = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
        meleeHit = gameObject.GetComponent<MeleeHit>();
        listOfSpawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
        SpawnPlayerOnRandomSpawnPosition();

        // DAMAGE ZONE VARIABLES. Not used now 
        //var damageZoneParent = GameObject.Find("DamageZoneParent").gameObject;
        //print("DAMAGE ZONE IS: " + damageZoneParent.name);

        //var safeZone = damageZoneParent.transform.Find("SafeZone").gameObject;
        //print("SAFE ZONE IS: " + safeZone.name);
        //sceneSafeZone = safeZone.GetComponent<SafeZone>();

        //sceneSafeZone.playerHealth = gameObject.GetComponent<PlayerHealth>();
        //sceneSafeZone.playerData = gameObject.GetComponent<PlayerData>();
        //print("SAFEZONE - Added safe zone to player health, name is: " + sceneSafeZone.name);

        // CURRENTLY WE SKIP CLOUD STUFF SINCE WE ARE NOT USING IT!!
        //cloudSave = gameObject.GetComponent<CloudSave>();
        //print("cloudsave is: " + cloudSave);

        //// Load players coins from DB on start. This can work for equipment etc in the future aswell
        //print("printing COINS FROM CLOUD");
        ////cloudSave.GetPlayersCoinsFromDB();
        //cloudSave.AssignPlayersCoinsOnStart();

        // Assign class values and class sprite to player from lobby manager
        playerClass = GetComponent<PlayerClass>();
        lobbyManager = GameObject.Find("LobbyCanvas").GetComponent<LobbyManager>();
        playerClass.SetPlayerDataBasedOnClass(lobbyManager.chosenClass.ToString());
        gameObject.GetComponent<SpriteRenderer>().sprite = lobbyManager.selectedClassSprite;
        chosenPlayerClass = lobbyManager.chosenClass.ToString();



        if (chosenPlayerClass == "Ranger")
        {
            chosenPlayerWeapon = "bow";
            shootProjectile = playerBow.GetComponent<ShootProjectile>();
        }
        else if (chosenPlayerClass == "Mage")
        {
            chosenPlayerWeapon = "wand";
            shootProjectile = playerWand.GetComponent<ShootProjectile>();
        }

    }
    private void FixedUpdate()
    {
        if (!IsOwner) return; // Return if we are not owner. = Only your own client can run
        characterMovement.MoveAndRotatePlayer(); // We need to call this every frame but only for our own character

    }
    private void Update()
    {
        if (!IsOwner) return; // Return if we are not owner. = Only your own client can run
        //characterMovement.MoveAndRotatePlayer(); // We need to call this every frame but only for our own character


        if (Input.GetKeyDown(KeyCode.X))
        {
            //// This can send a message over Rpc to the server
            //TestServerRpc();

            //// This sends message to clients from the server
            //TestClientRpc();

            //customData.Value = new MyCustomData
            //{
            //    _int = 10,
            //    _bool = false,
            //    message = "I am learning networking. This shit is hard!"
            //};

            // Test spawning object on player
            //serverObjectManager.SpawnObject(testSpawnObjectForServer, transform.position);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            serverObjectManager.DestroyObject(spawnedObjectTransform.gameObject);
            //Destroy(spawnedObjectTransform.gameObject); // Server destroys the object
            //spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true); // Objects is removed from server but gameobject is still left there 
        }
        // Ranged attack with left mouse btn
        else if (Input.GetMouseButtonDown(0))
        {
            if(chosenPlayerClass == "Warrior")
            {
                meleeHit.PrepareMeleeHit();
            }
            // Shoot projectile for mage/ranger
            else
            {
                // Charge bow/wand when starting to hold mouse button
                shootProjectile.ChargeWeapon(chosenPlayerWeapon);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (chosenPlayerClass == "Warrior")
            {
                // Warriro class doesnt use mouse button up. Just return
                return;
            }
            // On release mousebutton, shoot the projectile 
            if (shootProjectile.isWeaponReadyToShoot == false)
            {
                shootProjectile.ResetChargeAnimation(chosenPlayerWeapon);
            }
            else if (shootProjectile.isWeaponReadyToShoot == true)
            {
                shootProjectile.PrepareFireProjectile(chosenPlayerWeapon);
            }
            
        }
        // Melee attack with right mouse btn
        else if (Input.GetMouseButtonDown(1))
        {
            // Nothing
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            print("SAVING COINS TO CLOUD");
            cloudSave.SavePlayersCoinsToCloud();

        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            print("printing COINS FROM CLOUD");
            cloudSave.GetPlayersCoinsFromDB();
            //cloudSave.AssignPlayersCoinsOnStart();

        }
    }

    public void SpawnPlayerOnRandomSpawnPosition()
    {
        var rand = new System.Random();
        int index = rand.Next(listOfSpawnPoints.Length);
        playerSpawnPosition = listOfSpawnPoints[index].transform.position;
        gameObject.transform.position = playerSpawnPosition;
    }

    // ServerRpc is for sending messages FROM the client TO the server
    // Example of this is in our fire projectile code in ServerObjectSpawner where client calls the rpc function
    // which makes the host/server spawn projectile and fire it
    [ServerRpc(RequireOwnership = false)]
    public void TestServerRpc()
    { 
        Debug.Log("Calling TestServerRpc function from client number: " + OwnerClientId);
    }

    // ClientRpc is meant to be called FROM the server which then RUN ON the clients.
    // The client can NOT call a function with ClientRpc. 
    // Its ONLY means to be called from the server
    // ClientRpc can also send a message to a specific client only
    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("Calling TestClientRpc function from client number: " + OwnerClientId);
    }



}
