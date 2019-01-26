using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractablePickUp : Interactable
{
  private new Rigidbody rigidbody;

  private void Awake()
  {
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
    GetComponent<Collider>().enabled = false;
  }

  public void EnablePhysics()
  {
    IsInteractable = true;
    rigidbody.isKinematic = false;
    GetComponent<Collider>().enabled = true;
  }
}