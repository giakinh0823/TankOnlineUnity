using Tank;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [field: SerializeField]
    public TankController MainTank { get; set; }

    private void Update()
    {
        if (this.MainTank)
            this.Move(this.MainTank.transform.position);
    }

    public void Move(Vector3 v)
    {
        this.Move(v.x, v.y);
    }

    public void Move(float x, float y)
    {
        this.transform.position = new Vector3(x, y, this.transform.position.z);
    }
}