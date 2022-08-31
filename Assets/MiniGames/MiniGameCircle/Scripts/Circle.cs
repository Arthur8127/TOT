using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame_circle
{
    public class Circle : MonoBehaviour
    {
        public bool isRot;
        public float angle;


        public void Scatter()
        {
            angle = Random.Range(-180, 180);
            transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
        }

        private void Update()
        {
            if (isRot)
            {
                transform.Rotate(Vector3.forward * Time.deltaTime * -100f);
            }
        }
        public void Down()
        {
            isRot = true;
        }
        public void Up()
        {
            isRot = false;

            float z = transform.rotation.eulerAngles.z;
            float autoRot = Mathf.Round(z / 10f) * 10f;
            transform.rotation = Quaternion.Euler(Vector3.forward * autoRot);
        }
    }
}

