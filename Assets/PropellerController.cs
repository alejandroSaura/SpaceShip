using UnityEngine;
using System.Collections;

public class PropellerController : MonoBehaviour {

	public float maxForce = 30000f;

	public Vector3 desiredVelocity = Vector3.zero;
	Vector3 desiredForceToApply = Vector3.zero;
	Vector3 perpendicularForceToApply = Vector3.zero;

	Rigidbody _rigidbody;

	GameObject spaceShip;

	// Use this for initialization
	void Start () {
		_rigidbody = gameObject.GetComponent<Rigidbody>();
		spaceShip = transform.parent.parent.gameObject;
	}

	void OnDrawGizmos ()
	{
		//Debug.DrawLine(transform.position, (transform.position + Vector3.right * forceToApplyPerpendicular.x/3000f + Vector3.forward * forceToApplyPerpendicular.z/3000f + Vector3.up * forceToApplyPerpendicular.y/3000f), Color.blue);
		Vector3 aux = desiredVelocity / 3f;
		Debug.DrawLine(transform.position, (transform.position + aux), Color.blue);

		Vector3 aux2 = perpendicularForceToApply / 400f;
		Debug.DrawLine(transform.position, (transform.position + aux2), Color.red);

	}
	
	// Update is called once per frame
	void Update () 
	{
		desiredForceToApply = Vector3.zero;

		Vector3 deltaVelocity = (desiredVelocity - _rigidbody.velocity);
		deltaVelocity = _rigidbody.mass * deltaVelocity;
		deltaVelocity = deltaVelocity / Time.fixedDeltaTime;
		desiredForceToApply += deltaVelocity * 8f;	

		//project the desired force onto the perpendicular plane
		Vector3 normal = spaceShip.transform.forward;
		perpendicularForceToApply = desiredForceToApply - Vector3.Dot(desiredForceToApply, normal) * normal;

		perpendicularForceToApply = Vector3.ClampMagnitude(perpendicularForceToApply, maxForce);

		if(perpendicularForceToApply.magnitude > 1)
		{
			transform.GetChild(0).transform.rotation = Quaternion.LookRotation(-perpendicularForceToApply);
		}
		else
		{
			transform.GetChild(0).transform.rotation = Quaternion.LookRotation(-spaceShip.transform.up);
		}

	}

	void FixedUpdate ()
	{
		_rigidbody.AddForce(perpendicularForceToApply);		
	}


}
