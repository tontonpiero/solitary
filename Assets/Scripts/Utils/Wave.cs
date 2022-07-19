using UnityEngine;

namespace Solitary
{
    public class Wave : MonoBehaviour
    {
        [SerializeField]
        protected bool horizontal = false;

        [SerializeField]
        protected bool vertical = true;

        [SerializeField]
        protected bool randomStart = false;

        [SerializeField]
        protected float amplitude = 100.0f;

        [SerializeField]
        protected float speed = 1.0f;

        private Vector3 offset;
        private float value = 0f;

        // Use this for initialization
        void Start()
        {
            offset = transform.localPosition;
            if (randomStart)
            {
                value = Random.Range(0.0f, 1.0f);
                speed = Random.Range(0.5f, 2.0f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            value += (speed * Time.deltaTime) % Mathf.PI;
            Vector3 pos = Vector2.zero;
            if (horizontal)
                pos.x = Mathf.Cos(value) * amplitude;
            if (vertical)
                pos.y = Mathf.Sin(value) * amplitude;
            //Log.Debug(value, pos.y);
            transform.localPosition = offset + pos;
        }

        public void Stop()
        {
            transform.localPosition = offset;
            enabled = false;
        }
    }
}