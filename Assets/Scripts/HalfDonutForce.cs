using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfDonutForce : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        float force = 20f;
        Vector3 direction = collision.contacts[0].point - collision.collider.transform.position;
        direction = -direction.normalized;
        if (collision.collider.gameObject.tag == "Player")
        {
            collision.collider.GetComponent<Player>().Stun(20);
        }
        else if (collision.collider.gameObject.tag == "Opponent")
        {
            collision.collider.GetComponent<Opponent>().Stun(20);
        }
        collision.collider.attachedRigidbody.AddForce(direction * force);
    }
}
