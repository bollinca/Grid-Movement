using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Things to fix:
//Pins Get stuck on non-player colliders (should ignore all other colliders)
// Player movement gets stuck sometimes when pins fail to despawn (might be fixed by previous bug fix)
// Need to refactor.


public class EnemyImpact : MonoBehaviour
{
    private BoxCollider2D enemyCollider;
    private string impactDirection;
    private Vector2 playerPos;
    private Vector2 enemyPos;

    //For finding impact direction
    private float leftOrRight;
    private float upOrDown;
    readonly int zeroAxis = 0; // named for legibility
    private float bumpMoveSpeed = 0.2f;
    private Vector2 enemyPosition;
    private Vector2 deadEnemyPosition;
    private float randomAngle;

    // Start is called before the first frame update
    void Start()
    {
        enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        enemyPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerPos = collision.gameObject.transform.position;
        enemyPos = gameObject.transform.position;

        print("Collision detected!");
        impactDirection = FindImpactDirection();
        deadEnemyPosition = setCorpsePosition(impactDirection, enemyPosition);
        randomAngle = Random.Range(-0.8f, 0.8f);
        StartCoroutine(destroyEnemy(gameObject, deadEnemyPosition, randomAngle));
    }

    string FindImpactDirection()
    {
        leftOrRight = Mathf.Round(playerPos.x - enemyPos.x);
        upOrDown = Mathf.Round(playerPos.y - enemyPos.y);

        if (upOrDown == zeroAxis) //Neither up nor down
        {
            switch (leftOrRight > 0)
            {
                case true:
                    return "right";
                case false:
                    return "left";
            }
        }
        switch (upOrDown > zeroAxis)
        {
            case true:
                return "up";
            case false:
                return "down";
        }
    }

    Vector2 setCorpsePosition(string direction, Vector2 pinPosition)
    {
        int impact = 1;

        switch (direction)
        {
            case "right":
                deadEnemyPosition = new Vector2(pinPosition.x - impact, pinPosition.y);
                break;
            case "left":
                deadEnemyPosition = new Vector2(pinPosition.x + impact, pinPosition.y);
                break;
            case "up":
                deadEnemyPosition = new Vector2(pinPosition.x, pinPosition.y - impact);
                break;
            case "down":
                deadEnemyPosition = new Vector2(pinPosition.x, pinPosition.y + impact);
                break;

        }
        return deadEnemyPosition;
    }

    bool IsDeathMovementDone(GameObject bowlingPin, Vector2 deadPosition)
    {
        float enemyX = bowlingPin.transform.position.x;
        float enemyY = bowlingPin.transform.position.y;

        if ((enemyX == deadPosition.x) && (enemyY == deadPosition.y))
        {
            print("true incoming");
            return true;
        };
        print("false incoming");
        bowlingPin.transform.position = Vector2.MoveTowards(bowlingPin.transform.position, deadPosition, bumpMoveSpeed * Time.fixedDeltaTime);
        bowlingPin.transform.RotateAround(bowlingPin.transform.position, Vector3.forward, randomAngle);

        return false;
    }

    IEnumerator destroyEnemy(GameObject bowlingPin, Vector2 deadPosition, float randomAngle)
    {
        float enemyX = bowlingPin.transform.position.x;
        float enemyY = bowlingPin.transform.position.y;

        //bowlingPin.transform.RotateAround(bowlingPin.transform.position, Vector3.forward, randomAngle);
        yield return new WaitUntil(() => IsDeathMovementDone(bowlingPin, deadPosition));
        Destroy(bowlingPin);
    }

}
