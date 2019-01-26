using UnityEngine;
using Rewired;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
  [SerializeField]
  private float moveSpeed = 1;

  [SerializeField]
  private float lookSensitivity = 1;

  [SerializeField]
  private float maxLookAngle = 60;

  [SerializeField]
  private Transform headTransform = null;


  private Rewired.Player playerInput;
  private new Rigidbody rigidbody;

  private void Awake()
  {
    rigidbody = GetComponent<Rigidbody>();
  }

  private void Update()
  {
    if (!Rewired.ReInput.isReady)
      return;

    playerInput = Rewired.ReInput.players.GetPlayer(0);

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
    headTransform.Rotate(0, lookHorizontalAxis * lookSensitivity, 0, Space.World);
    headTransform.Rotate(lookVerticalAxis * lookSensitivity, 0, 0, Space.Self);

    // Clamp x rotation
    Vector3 flatForward = headTransform.forward.WithY(0).normalized;
    float rotationSign = Mathf.Sign(headTransform.forward.y);
    float xRot = Vector3.Angle(flatForward, headTransform.forward);
    float deltaFromMax = Mathf.Max(xRot - maxLookAngle, 0);
    if (deltaFromMax > 0)
    {
      headTransform.Rotate(deltaFromMax * rotationSign, 0, 0, Space.Self);
    }
  }
}