using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImpact : MonoBehaviour
{
    private BoxCollider2D enemyCollider;
    private string direction;
    private Vector2 playerPos;
    private Vector2 enemyPos;

    //For finding impact direction
    private float leftOrRight;
    private float upOrDown;
    readonly int zeroAxis = 0; // named for legibility

    // Start is called before the first frame update
    void Start()
    {
        enemyCollider = gameObject.GetComponent<BoxCollider2D>();
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
        print(FindImpactDirection());
        Destroy(gameObject);
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

}
