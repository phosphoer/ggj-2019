using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractablePickUp : Interactable
{
  private new Rigidbody rigidbody;
  private Vector3 startScale;

  protected override void Awake()
  {
    base.Awake();
    rigidbody = GetComponent<Rigidbody>();
    startScale = transform.localScale;
  }

  public override void TriggerInteraction(PlayerController fromPlayer)
  {
    fromPlayer.HoldItem(this);
  }

  public void DisablePhysics()
  {
    IsInteractable = false;
    rigidbody.isKinematic = true;
    transform.localScale *= 0.5f;
    GetComponentInChildren<Collider>().enabled = false;
  }

  public void EnablePhysics()
  {
    IsInteractable = true;
    rigidbody.isKinematic = false;
    transform.localScale = startScale;
    GetComponentInChildren<Collider>().enabled = true;
  }
}