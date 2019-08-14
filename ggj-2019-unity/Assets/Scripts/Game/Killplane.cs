using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killplane : MonoBehaviour
{
	public Transform respawn;
	
	void OnCollisionEnter(Collision col)
	{
		col.gameObject.transform.position = respawn.transform.position;
		col.rigidbody.velocity = Vector3.zero;
		col.rigidbody.angularVelocity = Vector3.zero;
	}
}
