using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Solitary
{
    public class SceneAttribute : PropertyAttribute { }

    [Serializable]
    public class Level
    {
        [Scene]
        public string Scene;

        public string Name;

        public override string ToString() => $"Level:{Name} Scene:{Scene}";
    }
}
