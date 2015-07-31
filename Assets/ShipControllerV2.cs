using UnityEngine;
using System.Collections;

public class ShipControllerV2 : MonoBehaviour
{

	Rigidbody _rigidbody;

	//----------------------------------
	
	Vector3 desiredVelocity = Vector3.zero;
	Vector3 desiredVelocityPerpendicular = Vector3.zero;

	Vector3 desiredForceToApply = Vector3.zero;
	Vector3 forceToApplyLinear = Vector3.zero;
	Vector3 lastForceToApplyLinear = Vector3.zero;
	public float maxLinealForce = 30000f;

	public float maxVelocity = 40f;
	public float maxVerticalVelocity = 20f;
	public float maxHorizontalVelocity = 20f;

	//----------------------------------

	public PropellerController[] propellers;	
	
	//----------------------------------
	
	float desiredWYaw = 0f;
	public float maxWYaw = 2f;

	//----------------------------------
	
	float desiredWPitch = 0f;
	public float maxWPitch = 2f;

	//----------------------------------
	
	float desiredWRoll = 0f;
	public float maxWRoll = 2f;

	//----------------------------------
	//----------------------------------

	void Start ()
	{
		_rigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
	void Update ()
	{
		moveLinear ();
		movePerpendicular ();
		rotateYaw ();
		rotatePitch ();
		rotateRoll ();

		distributeToPropellers();
	}	

	void FixedUpdate ()
	{
		_rigidbody.AddForce(forceToApplyLinear);
		

	}


	void distributeToPropellers ()
	{
		//iterate the propellers
		foreach (PropellerController propeller in propellers)
		{
			//clear
			propeller.desiredVelocity = Vector3.zero;

			//linear velocity
			propeller.desiredVelocity += desiredVelocity;

			//perpendicular velocity
			propeller.desiredVelocity += desiredVelocityPerpendicular;

			Vector3 r = propeller.transform.position - transform.position;
			Vector3 w = Vector3.zero;

			//velocity related to Yall(Y) rotation
			w = desiredWYaw * transform.up;
			propeller.desiredVelocity += Vector3.Cross(w,r); 

			//velocity related to Pitch(X) rotation
			w = desiredWPitch * transform.right;
			propeller.desiredVelocity += Vector3.Cross(w,r);

			//velocity related to Roll(Z) rotation
			w = desiredWRoll * transform.forward;
			propeller.desiredVelocity += Vector3.Cross(w,r);

		}
	}

	void OnDrawGizmos ()
	{
	}
	
	void moveLinear ()
	{
		//Linear displacement ----------------
		lastForceToApplyLinear = forceToApplyLinear;

		desiredForceToApply = Vector3.zero;

		desiredVelocity = transform.forward * Input.GetAxis ("RT") * maxVelocity - transform.forward * Input.GetAxis ("LT") * maxVelocity;
		//Debug.Log(desiredVelocity);

		Vector3 aux = (desiredVelocity - Vector3.Project(_rigidbody.velocity, transform.forward));
		aux = _rigidbody.mass * aux;
		aux = aux / Time.fixedDeltaTime;
		desiredForceToApply += aux * 2f;
		
		//Clamp the total force to respect the the maxForce parameter once projected onto the plane.
		forceToApplyLinear = Vector3.ClampMagnitude(desiredForceToApply, maxLinealForce);

		if (Vector3.Dot( forceToApplyLinear, lastForceToApplyLinear) < 0f )
		{
			forceToApplyLinear = Vector3.zero;
		}		
	}
	
	void movePerpendicular ()
	{
		//Perpendicular displacement ----------------
		
		desiredVelocityPerpendicular = (transform.right * Input.GetAxis ("Horizontal2") * maxHorizontalVelocity) + (transform.up * Input.GetAxis ("Vertical2") * maxVerticalVelocity);

	}
	
	void rotateYaw ()
	{
		//Yaw (Y) Rotation ----------------
		
		//calculate desired velocity with RB and LB.
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

		//calculate desired velocity with left stick horizontal axis.
		desiredWYaw = Input.GetAxis ("Horizontal") * maxWYaw;
	}
	
	void rotatePitch ()
	{
		//Pitch (X) Rotation ----------------

		desiredWPitch = Input.GetAxis ("Vertical") * maxWPitch;
	}
	
	void rotateRoll ()
	{
		//Roll (Z) Rotation ----------------
		
		//calculate desired velocity with RB and LB
		if (Input.GetButton ("RB") && !Input.GetButton ("LB")) {
			desiredWRoll = -maxWRoll;
		} else {
			if (Input.GetButton ("LB") && !Input.GetButton ("RB")) {
				desiredWRoll = maxWRoll;
			} else {
				desiredWRoll = 0f;
			}
		}		

		//calculate desired velocity with left stick horizontal axis.
		//desiredWRoll = -Input.GetAxis("Horizontal") * maxWRoll;
	}
}
