using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;

public class TankController : MonoBehaviour
{
    // Start is called before the first frame update
    private Tank tank;

    public Sprite tankUp;
    public Sprite tankDown;
    public Sprite tankLeft;
    public Sprite tankRight;
    private TankMover tankMover;
    private CameraController cameraController;
    private SpriteRenderer _renderer;
    public GameObject camera;

    void Start()
    {
        tank = new Tank
        {
            Name = "Default",
            Direction = Direction.Down,
            Hp = 10,
            Point = 0,
            Position = new Vector3(Random.Range(0, 20), Random.Range(0, 20), 0)
        };
        gameObject.transform.position = tank.Position;
        tankMover = gameObject.GetComponent<TankMover>();
        cameraController = camera.GetComponent<CameraController>();
        _renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Direction.Right);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Direction.Up);
        }
    }

    private void Move(Direction direction)
    {
        cameraController.Move(tankMover.Move(direction));
        switch (direction)
        {
            case Direction.Down:
                _renderer.sprite = tankDown;
                break;

            case Direction.Up:
                _renderer.sprite = tankUp;
                break;
            case Direction.Left:
                _renderer.sprite = tankLeft;
                break;
            case Direction.Right:
                _renderer.sprite = tankRight;
                break;
        }
    }
}