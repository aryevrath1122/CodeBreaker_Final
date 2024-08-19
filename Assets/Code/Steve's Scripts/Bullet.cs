using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet when it collides with another object
        Destroy(gameObject);
    }

    // Alternatively, use OnTriggerEnter2D if you're using triggers instead of collisions
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     Destroy(gameObject);
    // }
}

