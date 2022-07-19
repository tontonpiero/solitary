using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Solitary
{

    public class BackButtonHandler : MonoBehaviour
    {
        static public List<BackButtonHandler> Handlers = new List<BackButtonHandler>();
        static public event Action OnListChanged;

        static public GameObject GetFocusedObject()
        {
            return Handlers.Count > 0 ? Handlers[Handlers.Count - 1].gameObject : null;
        }

        public UnityEvent OnBack = new UnityEvent();

        private void OnEnable()
        {
            Handlers.Add(this);
            OnListChanged?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && GetFocusedObject() == gameObject) OnBack?.Invoke();
        }

        private void OnDisable()
        {
            Handlers.Remove(this);
            OnListChanged?.Invoke();
        }

        private void OnDestroy()
        {
            OnBack?.RemoveAllListeners();
        }
    }

}