using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public List<Transform> playersTransformsInScene;
    public float enemyMovementSpeed = 2.5f;
    void Start()
    {

    }

    Vector2 GetClosestEnemy(List<Transform> playerTransformsInScene)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in playerTransformsInScene)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget.position;
    }

    private void FixedUpdate()
    {
        this.gameObject.transform.position = Vector2.MoveTowards(
            this.gameObject.transform.position, GetClosestEnemy(playersTransformsInScene), enemyMovementSpeed * Time.deltaTime
            );
    }

    public void AddPlayerToTransformsList(Transform playerToAddTransform)
    {
        playersTransformsInScene.Add(playerToAddTransform);
    }
}
