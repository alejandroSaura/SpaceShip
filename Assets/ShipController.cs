using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

	Vector3 desiredVelocity = Vector3.zero;
	Vector3 desiredForceToApply = Vector3.zero;
	Vector3 forceToApplyLinear = Vector3.zero;
	Vector3 lastForceToApplyLinear = Vector3.zero;

	Vector3 desiredVelocityPerpendicular = Vector3.zero;
	Vector3 desiredForceToApplyPerpendicular = Vector3.zero;
	Vector3 forceToApplyPerpendicular = Vector3.zero;
	Vector3 lastForceToApplyPerpendicular = Vector3.zero;


	Vector3 smoothedForceToApplyPerpendicular = Vector3.zero;


	public float maxVelocity = 5f;
	public float maxVerticalVelocity = 1f;
	public float maxHorizontalVelocity = 1f;

	public float maxLinealForce = 5f;
	public float maxTotalForcePerpendicular = 1f;


	//Propellers: 0 - topLeft, 1 - topRight, 2 - bottomRight, 3 - bottomLeft
	public GameObject[] propellers;

	public float propellersApertureWidth;
	public float propellersApertureHeight;

	Vector3 topLeft = Vector3.zero;
	Vector3 topRight = Vector3.zero;
	Vector3 bottomRight = Vector3.zero;
	Vector3 bottomLeft = Vector3.zero;


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
		moveLinear ();
		movePerpendicular ();
		rotateYaw ();
		rotatePitch ();
		rotateRoll ();
	}

	void FixedUpdate ()
	{
		_rigidbody.AddForce(smoothedForceToApplyPerpendicular + forceToApplyLinear);

		//_rigidbody.AddForce(forceToApplyLinear);
		_rigidbody.AddRelativeTorque(Vector3.up * torqueToApplyYaw + Vector3.right * torqueToApplyPitch + Vector3.forward * torqueToApplyRoll);
	}


	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
		//Debug.DrawLine(transform.position, (transform.position + Vector3.right * forceToApplyPerpendicular.x/3000f + Vector3.forward * forceToApplyPerpendicular.z/3000f + Vector3.up * forceToApplyPerpendicular.y/3000f), Color.blue);
		Debug.DrawLine(transform.position, (transform.position + Vector3.right * smoothedForceToApplyPerpendicular.x/3000f + Vector3.forward * smoothedForceToApplyPerpendicular.z/3000f + Vector3.up * smoothedForceToApplyPerpendicular.y/3000f), Color.blue);

		//Gizmos.DrawWireSphere(transform.position, maxTotalForcePerpendicular/8000f);

		Gizmos.color = Color.red;
		Debug.DrawLine(transform.position, (transform.position + forceToApplyLinear/5000f), Color.red);	
		Gizmos.color = Color.yellow;
		Debug.DrawLine(transform.position, (transform.position + topLeft/3000f), Color.yellow);
		Debug.DrawLine(transform.position, (transform.position + topRight/3000f), Color.yellow);
		Debug.DrawLine(transform.position, (transform.position + bottomLeft/3000f), Color.yellow);
		Debug.DrawLine(transform.position, (transform.position + bottomRight/3000f), Color.yellow);



	}



	void moveLinear ()
	{
		//Linear displacement ----------------

		lastForceToApplyLinear = forceToApplyLinear;

		forceToApplyLinear = Vector3.zero;
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

		lastForceToApplyPerpendicular = forceToApplyPerpendicular;

		forceToApplyPerpendicular = Vector3.zero;
		desiredForceToApplyPerpendicular = Vector3.zero;
		
		//Take the input to calculate desired velocity.
		desiredVelocityPerpendicular = (transform.right * Input.GetAxis("Horizontal2") * maxHorizontalVelocity) + (transform.up * Input.GetAxis("Vertical2") * maxVerticalVelocity);
		
		//Calculate the force to apply needed to reach the desired velocity:

		Vector3 aux = (desiredVelocityPerpendicular - _rigidbody.velocity);
		aux = _rigidbody.mass * aux;
		aux = aux / Time.fixedDeltaTime;
		desiredForceToApplyPerpendicular += aux * 2f;
		//desiredForceToApply += (_rigidbody.mass * (desiredVelocity - _rigidbody.velocity)) / Time.fixedDeltaTime;	
		
		//Fight the gravity
		//desiredForceToApply += -Physics.gravity * _rigidbody.mass;

		//project the desired force onto the perpendicular plane
		Vector3 normal = transform.forward;
		Vector3 projectedDesiredForceToAply = desiredForceToApplyPerpendicular - Vector3.Dot(desiredForceToApplyPerpendicular, normal) * normal;

		//Vvector v1_projected = v1 - Dot(v1, n) * n;
		
		//Clamp the total force to respect the the maxForce parameter once projected onto the plane.	
		forceToApplyPerpendicular = Vector3.ClampMagnitude(projectedDesiredForceToAply, maxTotalForcePerpendicular);

		if (Vector3.Dot( forceToApplyPerpendicular, lastForceToApplyPerpendicular) < 0f )
		{
			forceToApplyPerpendicular = Vector3.zero;
		}

		smoothedForceToApplyPerpendicular = Vector3.Lerp(smoothedForceToApplyPerpendicular, forceToApplyPerpendicular, Time.deltaTime * 5f);

		distributeForceToPropellers ();

	}

	void distributeForceToPropellers ()
	{
		float magnitude = smoothedForceToApplyPerpendicular.magnitude / (float)propellers.Length;
		Vector3 totalForceDir = smoothedForceToApplyPerpendicular.normalized;
		Vector3 normalLongitudinal = transform.forward;
		Vector3 normalTransversal = Vector3.Cross(normalLongitudinal, totalForceDir).normalized;


		topLeft = -(totalForceDir * magnitude) + (normalLongitudinal * propellersApertureHeight) - (normalTransversal * propellersApertureWidth);
		topRight = -(totalForceDir * magnitude) + (normalLongitudinal * propellersApertureHeight) + (normalTransversal * propellersApertureWidth);
		bottomRight = -(totalForceDir * magnitude) - (normalLongitudinal * propellersApertureHeight) + (normalTransversal * propellersApertureWidth);
		bottomLeft = -(totalForceDir * magnitude) - (normalLongitudinal * propellersApertureHeight) - (normalTransversal * propellersApertureWidth);

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
		desiredWYaw = Input.GetAxis("Horizontal") * maxWYaw;

		
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
		desiredWPitch = Input.GetAxis("Vertical") * maxWPitch;

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
