using UnityEngine;

namespace Solitary
{
    public class Rotation : MonoBehaviour
    {
        [SerializeField] protected Vector3 values;
        [SerializeField] protected bool randomStart = false;

        public float Speed = 1f;

        private void Start()
        {
            if (randomStart)
            {
                Vector3 rotation = transform.rotation.eulerAngles;
                if (values.x != 0f) rotation.x = UnityEngine.Random.Range(0f, 360f);
                if (values.y != 0f) rotation.y = UnityEngine.Random.Range(0f, 360f);
                if (values.z != 0f) rotation.z = UnityEngine.Random.Range(0f, 360f);
                transform.rotation = Quaternion.Euler(rotation);
            }
        }

        void Update()
        {
            transform.Rotate(values * Time.deltaTime * Speed);
        }

        public void SetSpeed(float speed)
        {
            this.Speed = speed;
        }
    }
}