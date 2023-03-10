using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public float xSensitivity, ySensitivity;

    public Transform orientation;

    float xRotation, yRotation;

    public Queue<Quaternion> rotQueue;

    private void Start()
    {
        rotQueue = new Queue<Quaternion>();
    }

    private void Update()
    {
        if(GameManager.Instance.GameState == GameState.Playing){
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -70f, 70f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);

            rotQueue.Enqueue(Quaternion.Euler(xRotation, yRotation, 0));
        }
    }
}
