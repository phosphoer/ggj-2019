using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
  public bool IsInteractable { get; protected set; } = true;

  public abstract void TriggerInteraction(PlayerController fromPlayer);

  public void ShowInteractPrompt(PlayerController fromPlayer)
  {

  }

  public void HideInteractPrompt(PlayerController fromPlayer)
  {

  }
}