using System;

namespace Modules.Localization
{
    [Serializable]
    public class LocalizeParameter
    {
        public string Value;
        public bool Localize = false;

        private LocalizationManager manager;

        public LocalizeParameter(string value)
        {
            Value = value;
            Localize = false;
        }

        public LocalizeParameter(string value, bool localize)
        {
            Value = value;
            Localize = localize;
        }

        public override string ToString()
        {
            return Localize ? Manager.Localize(Value) : Value;
        }

        public LocalizationManager Manager
        {
            get
            {
                if (manager == null) manager = LocalizationManager.Instance;
                return manager;
            }
            set
            {
                manager = value;
            }
        }
    }
}