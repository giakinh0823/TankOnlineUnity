using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera Camera { get; private set; }

    public void Awake()
    {
        this.Camera = this.GetComponent<Camera>();
    }

    public void WrapBounds(BoundsInt boundsInt)
    {
        this.Camera.transform.position = new Vector3(boundsInt.center.x, boundsInt.center.y, this.Camera.transform.position.z);

        var aspectRatio = (float)Screen.width / Screen.height;
        this.Camera.orthographicSize = Mathf.Max(boundsInt.size.x, boundsInt.size.y) / 2f / aspectRatio;
    }
}