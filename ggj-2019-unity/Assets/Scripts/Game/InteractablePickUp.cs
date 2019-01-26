using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractablePickUp : Interactable
{
  private new Rigidbody rigidbody;

  protected override void Awake()
  {
    base.Awake();
    rigidbody = GetComponent<Rigidbody>();
  }

  public override void TriggerInteraction(PlayerController fromPlayer)
  {
    fromPlayer.HoldItem(this);
  }

  public void DisablePhysics()
  {
    IsInteractable = false;
    rigidbody.isKinematic = true;
    GetComponentInChildren<Collider>().enabled = false;
  }

  public void EnablePhysics()
  {
    IsInteractable = true;
    rigidbody.isKinematic = false;
    GetComponentInChildren<Collider>().enabled = true;
  }
}