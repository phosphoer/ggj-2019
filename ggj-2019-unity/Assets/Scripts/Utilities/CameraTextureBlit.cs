using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTextureBlit : MonoBehaviour
{
  public Camera Camera
  {
    get
    {
      if (camera == null)
        camera = GetComponent<Camera>();

      return camera;
    }
  }

  public RenderTexture InputTexture;

  private new Camera camera;

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    Graphics.Blit(InputTexture, destination);
  }
}