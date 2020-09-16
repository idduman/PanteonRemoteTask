using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickForce : MonoBehaviour
{
	public float power = 0.5f;
	private void OnCollisionStay(Collision collision)
	{
		Vector3 force = transform.parent.right * power * transform.parent.GetComponent<RotatingStick>().rotationSpeed;
		if (collision.collider.gameObject.tag == "Player")
		{
			collision.collider.GetComponent<Player>().Stun(20);
		}
		else if (collision.collider.gameObject.tag == "Opponent")
		{
			collision.collider.GetComponent<Opponent>().Stun(20);
		}
		collision.rigidbody.AddForce(force);
	}
}
