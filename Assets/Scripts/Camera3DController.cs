using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3DController : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de déplacement de la caméra

    void Update()
    {
        // Déplacement de la caméra avec les touches fléchées
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}
