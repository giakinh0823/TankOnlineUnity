using System;
using Entity;
using UnityEngine;

namespace DefaultNamespace
{
    public class TankFirer : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Sprite spriteRight;
        public Sprite spriteLeft;
        public Sprite spriteUp;
        public Sprite spriteDown;
        public int speed;
        public int maxRange;

        private void Start()
        {
            speed = 1;
            maxRange = 10;
        }

        private void Update()
        {
        }

        public void Fire(Bullet b)
        {
            
        }
    }
}