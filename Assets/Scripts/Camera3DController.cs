using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3DController : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de d�placement de la cam�ra

    void Update()
    {
        // D�placement de la cam�ra avec les touches fl�ch�es
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}
