using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Solitary
{

    public class UIView : MonoBehaviour
    {
        static public List<UIView> Views = new List<UIView>();
        static public event Action OnListChanged;

        static public GameObject GetFocusedView()
        {
            return Views.Count > 0 ? Views[Views.Count - 1].gameObject : null;
        }

        public UnityEvent OnBack = new UnityEvent();
        public UnityEvent OnGainFocus = new UnityEvent();
        public UnityEvent OnLoseFocus = new UnityEvent();

        private void OnEnable()
        {
            OnListChanged += HandleListChanged;
            Views.Add(this);
            OnListChanged?.Invoke();
            OnGainFocus?.Invoke();
        }

        private void HandleListChanged()
        {
            if (GetFocusedView() == gameObject)
            {
                OnGainFocus?.Invoke();
            }
            else
            {
                OnLoseFocus?.Invoke();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && GetFocusedView() == gameObject) OnBack?.Invoke();
        }

        private void OnDisable()
        {
            Views.Remove(this);
            OnListChanged?.Invoke();
            OnListChanged -= HandleListChanged;
        }

        private void OnDestroy()
        {
            OnBack?.RemoveAllListeners();
        }
    }

}