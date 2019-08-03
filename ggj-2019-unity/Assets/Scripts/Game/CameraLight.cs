using UnityEngine;

public class CameraLight : MonoBehaviour
{
    //Color lerpedColor = Color.black;
	Material cameraLight;

    void Awake()
	{
		cameraLight = this.GetComponent<Renderer>().material;
	}
	
	void Update()
    {
        //lerpedColor = Color.Lerp(Color.black, Color.red, Mathf.PingPong(Time.time, 1));
		cameraLight.color = Color.Lerp(Color.black, Color.red, Mathf.PingPong(Time.time, 1));
    }
}