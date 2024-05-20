using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public float moveSpeed = 10.0f; // Скорость движения камеры
    public float zoomSpeed = 4.0f;  // Скорость масштабирования камеры
    public float rotateSpeed = 100.0f; // Скорость вращения камеры

    void Update()
    {
        // Движение камеры с помощью клавиш
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, vertical, 0);
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Масштабирование камеры с помощью колесика мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2.0f, 20.0f);

        // Вращение камеры с помощью правой кнопки мыши
        if (Input.GetMouseButton(1))
        {
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");
            transform.RotateAround(transform.position, Vector3.up, rotateHorizontal * rotateSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, transform.right, -rotateVertical * rotateSpeed * Time.deltaTime);
        }
    }
}
