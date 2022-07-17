using System;
using UnityEngine;

namespace Solitary.Utils
{
    public interface IDragBehaviour
    {
        bool Enabled { get; set; }
        bool IsDragging { get; }
        Vector2 DragPosition { get; }

        event Action OnDragStarted;
        event Action OnDragComplete;
    }
}