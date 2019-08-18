using UnityEngine;
using Rewired;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
  public Rewired.Player PlayerInput
  {
    get { return playerInput; }
    set { playerInput = value; }
  }

  public Camera WorldCamera => playerCameraWorld;
  public Camera ViewportCamera => playerCameraViewport;
  public bool ControlsEnabled { get; set; }
  
  public Transform fakeRoomSpot;

  [SerializeField]
  private Transform headTransform = null;

  [SerializeField]
  private GameObject normalHead = null;
  
  [SerializeField]
  private GameObject alternateHead = null;

  [SerializeField]
  private Transform heldItemRoot = null;

  [SerializeField]
  private Camera playerCameraWorld = null;

  [SerializeField]
  private Camera playerCameraViewport = null;

  [SerializeField]
  private Animator animator = null;
  
  [SerializeField]
  private float moveSpeed = 1;
  
  [SerializeField]
  private float moveSpeedReg = 1;

  [SerializeField]
  private float moveSpeedDash = 1;

  [SerializeField]
  private float lookSensitivity = 1;

  [SerializeField]
  private float maxLookAngle = 60;

  [SerializeField]
  private float interactRange = 1.0f;

  [SerializeField]
  private LayerMask interactMask = Physics.DefaultRaycastLayers;

  [SerializeField]
  private AnimationCurve slapHeadRollAnim = null;

  [SerializeField]
  private GameObject fxSlapPrefab = null;
  
  [SerializeField]
  private GameObject fxHeartPrefab = null;

  [SerializeField]
  private SoundBank sfxSlap = null;

  [SerializeField]
  private SoundBank sfxInteract = null;

  private Rewired.Player playerInput;
  private new Rigidbody rigidbody;
  private Interactable focusedItem;
  private InteractablePickUp heldItem;
  private Transform heldItemOriginalParent;
  private Vector3 startPosition;
  private Vector3 endPosition;
  private Quaternion startRotation;
  private Quaternion startHeadRotation;
  private bool isSlapCancelled;

  private void Awake()
  {
    rigidbody = GetComponent<Rigidbody>();
  }

  private void Start()
  {
    startPosition = transform.position;
    startRotation = transform.rotation;
    startHeadRotation = headTransform.rotation;
	endPosition = fakeRoomSpot.position;
  }

  private void Update()
  {
    if (playerInput == null || !ControlsEnabled)
    {
      return;
    }

    // Gather input state
    float walkAxis = playerInput.GetAxis(InputConsts.Action.Walk);
    float strafeAxis = playerInput.GetAxis(InputConsts.Action.Strafe);
    float lookHorizontalAxis = playerInput.GetAxis(InputConsts.Action.LookHorizontal);
    float lookVerticalAxis = -playerInput.GetAxis(InputConsts.Action.LookVertical);
    bool isInteractPressed = playerInput.GetButtonDown(InputConsts.Action.Interact);
	bool isSelectPressed = playerInput.GetButtonDown(InputConsts.Action.Select);

    // Calculate movement
    Vector3 walkDirection = headTransform.forward.WithY(0).normalized;
    Vector3 strafeDirection = headTransform.right.WithY(0).normalized;
    Vector3 moveVec = (walkDirection * walkAxis + strafeDirection * strafeAxis).normalized;
    rigidbody.position += moveVec * moveSpeed * Time.deltaTime;

    // Calculate look
    headTransform.Rotate(0, lookHorizontalAxis * lookSensitivity * Time.unscaledDeltaTime, 0, Space.World);
    headTransform.Rotate(lookVerticalAxis * lookSensitivity * Time.unscaledDeltaTime, 0, 0, Space.Self);

    // Clamp x rotation
    Vector3 flatForward = headTransform.forward.WithY(0).normalized;
    float rotationSign = Mathf.Sign(headTransform.forward.y);
    float xRot = Vector3.Angle(flatForward, headTransform.forward);
    float deltaFromMax = Mathf.Max(xRot - maxLookAngle, 0);
    if (deltaFromMax > 0)
    {
      headTransform.Rotate(deltaFromMax * rotationSign, 0, 0, Space.Self);
    }

    // Update animation state
    if (isInteractPressed)
    {
      isSlapCancelled = false;
      animator.SetTrigger("Hit");
      StartCoroutine(SlapAnim());
    }
	
	//Hacking in a way to swap *HOOD* head for *SECRET SUDA* head (sorry for the slop!)
	if (isSelectPressed && alternateHead != null && normalHead.activeSelf)
    {
		normalHead.SetActive(false);
		alternateHead.SetActive(true);
		GameObject heartFx = Instantiate(fxHeartPrefab);
		heartFx.transform.position = headTransform.position;
    }

    animator.SetBool("IsWalking", moveVec.sqrMagnitude > 0.1f);
    animator.transform.forward = Mathfx.Damp(animator.transform.forward, flatForward, 0.5f, Time.deltaTime * 5.0f);

    // Look for interactables
    RaycastHit raycastHit;
    Interactable newFocusedInteractable = null;
    if (Physics.Raycast(headTransform.position, headTransform.forward, out raycastHit, interactRange, interactMask))
    {
      // Update the current focused interactable if we hit one
      Interactable interactable = raycastHit.collider.GetComponentInParent<Interactable>();
      if (interactable != null && interactable.IsInteractable)
      {
        newFocusedInteractable = interactable;
      }
    }

    SetFocusedInteractable(newFocusedInteractable);

    // Update focused item 
    if (focusedItem != null)
    {
      // If it becomes un-interactable defocus it 
      if (!focusedItem.IsInteractable)
      {
        SetFocusedInteractable(null);
        return;
      }

      // Trigger interactions with focused interactables 
      if (focusedItem.IsInteractable && isInteractPressed)
      {
        StartCoroutine(PickUpAnim());
        isSlapCancelled = true;
        return;
      }
    }

    // Update held item 
    if (heldItem != null)
    {
      if (isInteractPressed)
      {
        StartCoroutine(PutDownAnim());
        isSlapCancelled = true;
      }
	  
	  //Experimenting with changing moveSpeed when carrying / not carrying items
	  moveSpeed = moveSpeedReg;
	} 
	  else
	{
		moveSpeed = moveSpeedDash;
	}
  }

  public void ResetToFakeStartPosition()
  {
	this.rigidbody.velocity = Vector3.zero;
	this.rigidbody.angularVelocity = Vector3.zero;
    transform.SetPositionAndRotation(endPosition, startRotation);
    headTransform.rotation = startHeadRotation;
  }

  public void HoldItem(InteractablePickUp item)
  {
    if (heldItem == null)
    {
      heldItem = item;
      heldItemOriginalParent = heldItem.transform.parent;
      heldItem.transform.SetParent(heldItemRoot, worldPositionStays: true);
      heldItem.transform.localPosition = Vector3.zero;
      heldItem.DisablePhysics();
    }
  }

  public void DropHeldItem()
  {
    if (heldItem != null)
    {
      DecorItem decorItem = heldItem.GetComponent<DecorItem>();
      if (decorItem != null)
      {
        DecorEvaluator.EvaluateDroppedItem(decorItem);
      }

      heldItem.transform.SetParent(heldItemOriginalParent, worldPositionStays: true);
      heldItem.EnablePhysics();
      heldItem = null;
    }
  }

  public void ReceiveSlap()
  {
    animator.SetTrigger("Recoil");
    DropHeldItem();
    StartCoroutine(GotSlappedAnim());

    GameObject slapFx = Instantiate(fxSlapPrefab);
    slapFx.transform.position = headTransform.position;

    CommentaryManager.Instance.CommentaryReaction();
  }

  private void SetFocusedInteractable(Interactable interactable)
  {
    if (focusedItem != null)
    {
      focusedItem.HideInteractPrompt(this);
    }

    focusedItem = interactable;

    if (focusedItem != null)
    {
      focusedItem.ShowInteractPrompt(this);
    }
  }

  private IEnumerator PickUpAnim()
  {
    AudioManager.Instance.PlaySound(sfxInteract);

    yield return new WaitForSeconds(0.25f);

    if (focusedItem != null)
    {
      focusedItem.TriggerInteraction(this);
    }
  }

  private IEnumerator PutDownAnim()
  {
    AudioManager.Instance.PlaySound(sfxInteract);

    yield return new WaitForSeconds(0.25f);
    DropHeldItem();
  }

  private IEnumerator GotSlappedAnim()
  {
    const float duration = 0.5f;
    for (float timer = 0; timer < duration; timer += Time.deltaTime)
    {
      float headRoll = slapHeadRollAnim.Evaluate(timer / duration);
      headTransform.Rotate(headRoll * 2, headRoll * 1.0f, 0, Space.Self);
      yield return null;
    }
  }

  private IEnumerator SlapAnim()
  {
    yield return new WaitForSeconds(0.25f);

    if (!isSlapCancelled && heldItem == null)
    {
      Collider[] colliders = Physics.OverlapSphere(heldItemRoot.position, 0.5f);
      foreach (Collider col in colliders)
      {
        Rigidbody rb = col.GetComponentInParent<Rigidbody>();
        if (rb != null && rb != rigidbody)
        {
          rb.AddExplosionForce(150, heldItemRoot.position, 2.0f, 0.25f, ForceMode.Impulse);
          AudioManager.Instance.PlaySound(sfxSlap);

          PlayerController player = rb.GetComponent<PlayerController>();
          if (player != null)
          {
            player.ReceiveSlap();
          }
        }
      }
    }
  }
}