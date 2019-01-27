using UnityEngine;
using Rewired;

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

  [SerializeField]
  private Transform headTransform = null;

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
  private float lookSensitivity = 1;

  [SerializeField]
  private float maxLookAngle = 60;

  [SerializeField]
  private float interactRange = 1.0f;

  [SerializeField]
  private LayerMask interactMask = Physics.DefaultRaycastLayers;

  private Rewired.Player playerInput;
  private new Rigidbody rigidbody;
  private Interactable focusedItem;
  private InteractablePickUp heldItem;
  private Transform heldItemOriginalParent;
  private Vector3 startPosition;
  private Quaternion startRotation;
  private Quaternion startHeadRotation;

  private void Awake()
  {
    rigidbody = GetComponent<Rigidbody>();
  }

  private void Start()
  {
    startPosition = transform.position;
    startRotation = transform.rotation;
    startHeadRotation = headTransform.rotation;
  }

  private void Update()
  {
    if (playerInput == null)
    {
      gameObject.SetActive(false);
      return;
    }

    if (!ControlsEnabled)
    {
      return;
    }

    // Gather input state
    float walkAxis = playerInput.GetAxis(InputConsts.Action.Walk);
    float strafeAxis = playerInput.GetAxis(InputConsts.Action.Strafe);
    float lookHorizontalAxis = playerInput.GetAxis(InputConsts.Action.LookHorizontal);
    float lookVerticalAxis = -playerInput.GetAxis(InputConsts.Action.LookVertical);
    bool isInteractPressed = playerInput.GetButtonDown(InputConsts.Action.Interact);

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
      animator.SetTrigger("Hit");
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
        focusedItem.TriggerInteraction(this);
        return;
      }
    }

    // Update held item 
    if (heldItem != null)
    {
      if (isInteractPressed)
      {
        DropHeldItem();
      }
    }
  }

  public void ResetToStartPosition()
  {
    transform.SetPositionAndRotation(startPosition, startRotation);
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
      heldItem.transform.SetParent(heldItemOriginalParent, worldPositionStays: true);
      heldItem.EnablePhysics();
      heldItem = null;
    }
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
}