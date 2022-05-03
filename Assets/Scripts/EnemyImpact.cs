using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImpact : MonoBehaviour
{
    //Position shorthand
    private Vector2 playerPos;
    private Vector2 enemyPos;

    //Readonly
    private readonly int zeroAxis = 0;
    private readonly float deathSpeed = 0.2f;

    //For finding player location/impact source
    private string impactSource;
    private float leftOrRight;
    private float aboveOrBelow;

    //For death animation
    private Vector2 deadEnemyPosition;
    private float randomAngle;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Sets player and enemy positions for easy reference
        playerPos = collision.gameObject.transform.position;
        enemyPos = gameObject.transform.position;

        impactSource = FindImpactSource();
        deadEnemyPosition = CalcCorpsePosition(impactSource);
        //Randomizes rotation for each individual death "animation".
        randomAngle = Random.Range(-0.8f, 0.8f);
        StartCoroutine(DestroyEnemy(gameObject, deadEnemyPosition));
    }

    string FindImpactSource()
    {
        //Compares player location to enemy location. Right/Above = 1, Left/Below = -1
        leftOrRight = Mathf.Round(playerPos.x - enemyPos.x);
        aboveOrBelow = Mathf.Round(playerPos.y - enemyPos.y);

        //Used to feed origin of impact to death animation
        if (leftOrRight != zeroAxis)
        {
            if (leftOrRight > zeroAxis)
            {
                return "right";
            }
            return "left";
        }
        else if (aboveOrBelow != zeroAxis)
        {
            if (aboveOrBelow > zeroAxis)
            {
                return "above";
            }
            return "below";
        }
        return "ERROR";
    }

    Vector2 CalcCorpsePosition(string direction)
    {
        int impact = 1;

        //create copy of enemyPos to prevent changes to original during calculations.
        Vector2 deadPosition = enemyPos;

        switch (direction)
        {
            case "right":
                deadPosition.x -= impact;
                break;
            case "left":
                deadPosition.x += impact;
                break;
            case "above":
                deadPosition.y -= impact;
                break;
            case "below":
                deadPosition.y += impact;
                break;

        }
        return deadPosition;
    }

    //This function gets looped!
    bool IsDeathMovementDone(Vector2 deadPosition)
    {
        //enemyPos is declared to ensure that it is constantly updated during function loop.
        enemyPos = gameObject.transform.position;

        //If corpse has reached its target
        if ((enemyPos.x == deadPosition.x) && (enemyPos.y == deadPosition.y))
        {
            return true;
        };
        //otherwise, keep moving towards target and rotating.
        gameObject.transform.position = Vector2.MoveTowards(enemyPos, deadPosition, deathSpeed * Time.fixedDeltaTime); //has to use gameObject.transform.position, otherwise a copy is altered instead of the actual object.
        gameObject.transform.RotateAround(enemyPos, Vector3.forward, randomAngle);

        return false;
    }

    IEnumerator DestroyEnemy(GameObject bowlingPin, Vector2 deadPosition)
    {
        //trigger loop of IsDeathMovementDone (until true)
        yield return new WaitUntil(() => IsDeathMovementDone(deadPosition));
        Destroy(bowlingPin);
    }
}