using UnityEngine;

public class MaterialInstance : MonoBehaviour
{
  public Material Material
  {
    get
    {
      if (materialInstance == null)
      {
        materialInstance = Instantiate(renderer.sharedMaterial);
        renderer.sharedMaterial = materialInstance;
      }

      return materialInstance;
    }
  }

  [SerializeField]
  private new Renderer renderer = null;

  private Material materialInstance;

  private void OnDestroy()
  {
    Destroy(materialInstance);
  }
}