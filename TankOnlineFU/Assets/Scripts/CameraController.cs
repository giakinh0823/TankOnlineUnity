using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Move(Vector3 v)
    {
        Move(v.x, v.y);
    }

    public void Move(float x, float y)
    {
        var o = gameObject;
        o.transform.position = new Vector3(x, y, o.transform.position.z);
    }
}