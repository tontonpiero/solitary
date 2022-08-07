using BeautifulTransitions.Scripts.Transitions.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Solitary
{
    [RequireComponent(typeof(UIView))]
    public class BasePopup : MonoBehaviour
    {
        [SerializeField] private List<TransitionBase> transitions;

        public void Show()
        {
            gameObject.SetActive(true);
            transitions.ForEach(t => t.TransitionIn());
            OnShown();
        }

        virtual protected void OnShown() { }

        public void Hide()
        {
            if (transitions.Count > 0)
            {
                transitions.ForEach(t => t.TransitionOut());
            }
            else
            {
                gameObject.SetActive(false);
            }
            OnHidden();
        }

        virtual protected void OnHidden() { }

    }
}
