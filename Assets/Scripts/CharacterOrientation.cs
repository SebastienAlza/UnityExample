using UnityEngine;

public class CharacterOrientation : MonoBehaviour
{
	// Variables publiques pour configurer les seuils de direction
	public float rotationOffset = 0f;
	public Vector2 dirNord = new Vector2(-45, 45);
	public Vector2 dirEst = new Vector2(45, 135);
	public Vector2 dirOuest = new Vector2(-135, -45);
	public Vector3 dirSud = new Vector3(135, 180, 180);
	private Vector2 _playerRotation = Vector2.zero;
	private Animator animator;
	public Camera mainCamera;
	public GameObject targetPrefab;
	public Quaternion fixRotation;
	private GameObject targetInstance;
	public float fixedHeightAboveGround = 0.1f; // Hauteur fixe au-dessus du sol en mètres
	public LayerMask raycastLayers;
	private string lastDirection;

	private static readonly int Vertical = Animator.StringToHash("Vertical");
	private static readonly int Horizontal = Animator.StringToHash("Horizontal");

	private void Start()
	{
		animator = GetComponent<Animator>();
		lastDirection = "";
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}

		targetInstance = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
		targetInstance.SetActive(false);
	}

	private void Update()
	{
		transform.rotation = fixRotation;
		//if (Input.GetMouseButtonDown(0))
		//{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100f, raycastLayers)) // Utiliser le LayerMask ici
			{
				// Utiliser la position horizontale du point de collision, mais une hauteur verticale fixe
				Vector3 targetPosition = new Vector3(hit.point.x, fixedHeightAboveGround, hit.point.z);

				// Dessiner le rayon dans la scène pour le visualiser
				//Debug.DrawLine(ray.origin, hit.point, Color.red, 2f);

				//Debug.Log("Raycast hit at: " + hit.point);
				//Debug.Log("Target position set to: " + targetPosition);

				PlaceTargetAt(targetPosition);
				//OrientTowardsPosition(hit.point);
			}
			else
			{
				// Dessiner un rayon pour montrer qu'il n'y a pas eu de collision
				Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.blue, 2f);
				Debug.Log("Raycast didn't hit any object.");
			}
		HandleKeyboardInput();
		//}
	}

	private void PlaceTargetAt(Vector3 position)
	{
		targetInstance.SetActive(true);
		targetInstance.transform.position = position;
	}

	private void HandleKeyboardInput()
	{
		ResetRotation();
		animator.SetBool("walk", false);
		if (Input.GetKey(KeyCode.W))
		{
			_playerRotation.y = 1;
			animator.SetBool("walk", true);
		}
		if (Input.GetKey(KeyCode.A))
		{
			_playerRotation.x = -1;
			animator.SetBool("walk", true);
		}
		if (Input.GetKey(KeyCode.S))
		{
			_playerRotation.y = -1;
			animator.SetBool("walk", true);
		}
		if (Input.GetKey(KeyCode.D))
		{
			_playerRotation.x = 1;
			animator.SetBool("walk", true);
		}

		if (Mathf.Approximately(_playerRotation.x, 0)
			&& Mathf.Approximately(_playerRotation.y, 0))
		{
			_playerRotation.y = -1;
		}

		UpdateParams();
	}


	private void OrientTowardsPosition(Vector3 position)
	{
		Vector3 directionToTarget = position - transform.position;
		directionToTarget.y = 0;

		Vector2 forward = new Vector2(transform.forward.x, transform.forward.z).normalized;
		float angle = Vector2.SignedAngle(forward, new Vector2(directionToTarget.x, directionToTarget.z));

		float adjustedAngle = NormalizeAngle(angle + rotationOffset);

		if (IsAngleWithinRange(adjustedAngle, dirNord))
		{
			SetDirection("dirNord");
		}
		else if (IsAngleWithinRange(adjustedAngle, dirEst))
		{
			SetDirection("dirEst");
		}
		else if (IsAngleWithinRange(adjustedAngle, dirOuest))
		{
			SetDirection("dirOuest");
		}
		else if (IsAngleWithinRange(adjustedAngle, dirSud))
		{
			SetDirection("dirSud");
		}
	}

	private bool IsAngleWithinRange(float angle, Vector2 range)
	{
		return angle >= range.x && angle <= range.y;
	}

	private float NormalizeAngle(float angle)
	{
		while (angle > 180f) angle -= 360f;
		while (angle < -180f) angle += 360f;
		return angle;
	}

	private void SetDirection(string direction)
	{
		if (lastDirection != direction)
		{
			if (!string.IsNullOrEmpty(lastDirection))
			{
				animator.SetBool(lastDirection, false);
			}

			animator.SetBool(direction, true);
			lastDirection = direction;
		}
	}

	private void ResetRotation()
	{
		_playerRotation.x = 0;
		_playerRotation.y = 0;
	}

	private void UpdateParams()
	{
		animator.SetFloat(Horizontal, _playerRotation.x);
		animator.SetFloat(Vertical, _playerRotation.y);
	}
}