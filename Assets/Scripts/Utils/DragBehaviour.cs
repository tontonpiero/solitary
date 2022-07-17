using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitary.Utils
{
    public class DragBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDragBehaviour
    {
        public bool Enabled { get => enabled; set => enabled = value; }
        public bool IsDragging { get; private set; } = false;
        public Vector2 DragPosition { get; private set; } = Vector2.zero;

        public event Action OnDragStarted;
        public event Action OnDragComplete;

        public void OnDrag(PointerEventData eventData)
        {
            DragPosition = eventData.position;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;
            OnDragStarted?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
            OnDragComplete?.Invoke();
        }

        private void OnDestroy()
        {
            OnDragStarted = null;
            OnDragComplete = null;
        }
    }
}
