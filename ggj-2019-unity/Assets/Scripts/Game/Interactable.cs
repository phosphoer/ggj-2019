using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
  public bool IsInteractable { get; protected set; } = true;

  [SerializeField]
  private MaterialInstance materialInstance = null;

  public abstract void TriggerInteraction(PlayerController fromPlayer);

  protected virtual void Awake()
  {

  }

  public void ShowInteractPrompt(PlayerController fromPlayer)
  {
    materialInstance.Material.SetColor("_HighlightColor", Color.white * 0.25f);
  }

  public void HideInteractPrompt(PlayerController fromPlayer)
  {
    materialInstance.Material.SetColor("_HighlightColor", Color.black);
  }
}