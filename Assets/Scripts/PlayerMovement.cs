using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private GameObject player;
    public Vector3 newPlayerPosition;
    private float speed;
    //private float movementModifier;
    private float inputPolarity;
    private bool movementCompleted;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        speed = 0.1f;
        //movementModifier = 0.01f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            StartCoroutine(MoveOneGrid());
        }
    }

    //void MoveFree()
    //{
    //    if (Mathf.Abs(Input.GetAxis("Vertical")) == 1)
    //    {
    //        player.transform.Translate(Vector3.up * movementModifier * Input.GetAxis("Vertical"));
    //    }
    //    else if (Mathf.Abs(Input.GetAxis("Horizontal")) == 1)
    //    {
    //        player.transform.Translate(Vector3.right * movementModifier * Input.GetAxis("Horizontal"));
    //    }
    //    ;
    //}



    //void setNewLocation()
    //{
    //    //determine direction player is moving in and set position to move towards
    //    if (Input.GetAxis("Vertical") != 0)
    //    {
    //        inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
    //        newLocation = new Vector3(transform.position.x, transform.position.y + inputPolarity, transform.position.z);
    //    }
    //    else if (Input.GetAxis("Horizontal") != 0)
    //    {
    //        inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
    //        newLocation = new Vector3(transform.position.x + inputPolarity, transform.position.y, transform.position.z);
    //    }
    //}

    void SetTargetLocation()
    {
        //determine direction player is moving in and set position to move towards
        if (Input.GetAxis("Vertical") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
            newPlayerPosition = new Vector3(transform.position.x, transform.position.y + inputPolarity, transform.position.z);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
            newPlayerPosition = new Vector3(transform.position.x + inputPolarity, transform.position.y, transform.position.z);
        }
    }

    void MoveTowardTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, newPlayerPosition, Time.fixedDeltaTime * speed);
    }

    bool isMovementCompleted()
    {
        if (player.transform.position == newPlayerPosition)
        {
            movementCompleted = true;
            return movementCompleted;
        }
        else
        {
            MoveTowardTarget();
            return movementCompleted = false;
        }
    }
    IEnumerator MoveOneGrid()
    {
        SetTargetLocation();
        yield return new WaitUntil(isMovementCompleted);
    }
}
