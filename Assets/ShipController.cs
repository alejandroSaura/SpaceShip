using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

	Vector3 desiredVelocity= Vector3.zero;
	Vector3 desiredForceToApply = Vector3.zero;
	Vector3 forceToApply = Vector3.zero;

	public float maxVerticalVelocity = 1f;
	public float maxHorizontalVelocity = 1f;
	public float maxTotalForce = 1f;

	//----------------------------------

	float desiredWYaw = 0f;
	float desiredTorqueToApplyYaw = 0f;
	float torqueToApplyYaw = 0f;

	public float maxWYaw = 1f;
	public float maxTorqueYaw = 1f;

	//----------------------------------

	float desiredWPitch = 0f;
	float desiredTorqueToApplyPitch = 0f;
	float torqueToApplyPitch = 0f;
	
	public float maxWPitch = 1f;
	public float maxTorquePitch = 1f;

	//----------------------------------

	float desiredWRoll = 0f;
	float desiredTorqueToApplyRoll = 0f;
	float torqueToApplyRoll = 0f;
	
	public float maxWRoll = 1f;
	public float maxTorqueRoll = 1f;

	Rigidbody _rigidbody;

	void Start () 
	{
		_rigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
	void Update () 
	{
		movePlanar ();
		rotateYaw ();
		rotatePitch ();
		rotateRoll ();
	}


	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
		Debug.DrawLine(transform.position, (transform.position + Vector3.right * forceToApply.x/3000f + Vector3.forward * forceToApply.z/3000f + Vector3.up * forceToApply.y/3000f), Color.blue);
		Gizmos.DrawWireSphere(transform.position, maxTotalForce/3000f);
	}

	void FixedUpdate ()
	{
		_rigidbody.AddForce(forceToApply);
		_rigidbody.AddRelativeTorque(Vector3.up * torqueToApplyYaw + Vector3.right * torqueToApplyPitch + Vector3.forward * torqueToApplyRoll);
	}

	void movePlanar ()
	{
		//Planar displacement ----------------
		
		forceToApply = Vector3.zero;
		desiredForceToApply = Vector3.zero;
		
		//Take the input to calculate desired velocity.
		desiredVelocity = (transform.right * Input.GetAxis("Horizontal") * maxHorizontalVelocity) + (transform.up * Input.GetAxis("Vertical") * maxVerticalVelocity);
		
		//Calculate the force to apply needed to reach the desired velocity:

		Vector3 aux = (desiredVelocity - _rigidbody.velocity);
		aux = _rigidbody.mass * aux;
		aux = aux / Time.fixedDeltaTime;
		desiredForceToApply += aux * 2f;
		//desiredForceToApply += (_rigidbody.mass * (desiredVelocity - _rigidbody.velocity)) / Time.fixedDeltaTime;	
		
		//Fight the gravity
		//desiredForceToApply += -Physics.gravity * _rigidbody.mass;
		
		//Clamp the total force to respect the the maxForce parameter.
		forceToApply = Vector3.ClampMagnitude(desiredForceToApply, maxTotalForce);
	}

	void rotateYaw ()
	{
		//Yaw (Y) Rotation ----------------
		
		torqueToApplyYaw = 0f;
		desiredTorqueToApplyYaw = 0f;
		desiredWYaw = 0f;
		
		//Take the input to calculate desired velocity.
//		if (Input.GetButton("RB") && !Input.GetButton("LB"))
//		{
//			desiredWYaw = maxWYaw;
//		}else{
//			if (Input.GetButton("LB") && !Input.GetButton("RB"))
//			{
//				desiredWYaw = -maxWYaw;
//			}else
//			{
//				desiredWYaw = 0f;
//			}
//		}		
		desiredWYaw = Input.GetAxis("Horizontal2") * maxWYaw;

		
		//I = m * r'2, T= I * alpha; Suposed r = 1;
		desiredTorqueToApplyYaw = ((desiredWYaw - transform.InverseTransformDirection(_rigidbody.angularVelocity).y)/Time.fixedDeltaTime) * _rigidbody.mass;
		torqueToApplyYaw = Mathf.Clamp(desiredTorqueToApplyYaw, -maxTorqueYaw, maxTorqueYaw);
	}

	void rotatePitch ()
	{
		//Pitch (X) Rotation ----------------
		
		torqueToApplyPitch = 0f;
		desiredTorqueToApplyPitch = 0f;
		desiredWPitch = 0f;
		
		//Take the input to calculate desired velocity.
		desiredWPitch = Input.GetAxis("Vertical2") * maxWPitch;

		//I = m * r'2, T= I * alpha; Suposed r = 1;
		desiredTorqueToApplyPitch = ((desiredWPitch - transform.InverseTransformDirection(_rigidbody.angularVelocity).x)*0.1f/Time.fixedDeltaTime) * _rigidbody.mass;
		torqueToApplyPitch = Mathf.Clamp(desiredTorqueToApplyPitch, -maxTorquePitch, maxTorquePitch);
		//Debug.Log (transform.InverseTransformDirection(_rigidbody.angularVelocity).x);
	}

	public void rotateRoll ()
	{
		//Roll (Z) Rotation ----------------
		
		torqueToApplyRoll = 0f;
		desiredTorqueToApplyRoll = 0f;
		desiredWRoll = 0f;
		
		//Take the input to calculate desired velocity.
		//desiredWRoll = Input.GetAxis("Horizontal2") * maxWRoll;

		if (Input.GetButton("RB") && !Input.GetButton("LB"))
		{
			desiredWRoll = -maxWRoll;
		}else{
			if (Input.GetButton("LB") && !Input.GetButton("RB"))
			{
				desiredWRoll = maxWRoll;
			}else
			{
				desiredWRoll = 0f;
			}
		}		
		
		//I = m * r'2, T= I * alpha; Suposed r = 1;
		desiredTorqueToApplyRoll = ((desiredWRoll - transform.InverseTransformDirection(_rigidbody.angularVelocity).z)*0.1f/Time.fixedDeltaTime) * _rigidbody.mass;
		torqueToApplyRoll = Mathf.Clamp(desiredTorqueToApplyRoll, -maxTorqueRoll, maxTorqueRoll);
		//Debug.Log (transform.InverseTransformDirection(_rigidbody.angularVelocity).x);
	}
}
