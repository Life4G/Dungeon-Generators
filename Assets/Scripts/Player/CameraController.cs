using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    public Transform movePoint;
    public Transform targetObject;

    public bool isTracked = true;

    void Start()
    {
        movePoint.parent = null;
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            isTracked = false;
            movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            isTracked = false;
            movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            isTracked = true;
        }
        if (isTracked)
        { 
            Vector3 newCameraPos = Vector3.MoveTowards(transform.position, targetObject.position, moveSpeed * Time.deltaTime);
            newCameraPos.z = transform.position.z;
            transform.position = newCameraPos;
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

    }
}
