using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private GameObject player;
    public Vector3 newLocation;
    private float speed;
    //private float movementModifier;
    private float inputPolarity;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        speed = 3;
        //movementModifier = 0.01f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveGrid();
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



    void setNewLocation()
    {
        //determine direction player is moving in and set position to move towards
        if (Input.GetAxis("Vertical") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
            newLocation = new Vector3(transform.position.x, transform.position.y + inputPolarity, transform.position.z);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
            newLocation = new Vector3(transform.position.x + inputPolarity, transform.position.y, transform.position.z);
        }
    }

    // Pseudo-grid based movement
    // Want to update to make it lock into grid strictly
    void MoveGrid()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) != 0)
        {
            setNewLocation();
            transform.position = Vector3.MoveTowards(transform.position, newLocation, Time.deltaTime * speed);
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) != 0)
        {
            setNewLocation();
            transform.position = Vector3.MoveTowards(transform.position, newLocation, Time.deltaTime * speed);
        }
    }
}
