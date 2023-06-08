using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera Camera { get; private set; }

    public void Awake()
    {
        this.Camera = this.GetComponent<Camera>();
    }

    public void WrapBounds(Bounds bounds)
    {
        this.Camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, this.Camera.transform.position.z);

        var aspectRatio = (float)Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        this.Camera.orthographicSize = Mathf.Max(bounds.size.x, bounds.size.y) / 2f * aspectRatio;
    }
}