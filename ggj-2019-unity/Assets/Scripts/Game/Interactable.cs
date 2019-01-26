using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
  public bool IsInteractable { get; protected set; } = true;

  [SerializeField]
  private GameObject highlightVisual = null;

  public abstract void TriggerInteraction(PlayerController fromPlayer);

  protected virtual void Awake()
  {
    highlightVisual.SetActive(false);
  }

  public void ShowInteractPrompt(PlayerController fromPlayer)
  {
    highlightVisual.SetActive(true);
  }

  public void HideInteractPrompt(PlayerController fromPlayer)
  {
    highlightVisual.SetActive(false);
  }
}