using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [CreateAssetMenu(fileName = "Actor", menuName = "Kite/Actor", order = 800)]
    public class Actor : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Color color = Color.white;

        public string DisplayName
        {
            get { return displayName; }
        }

        public Color Color
        {
            get { return color; }
        }
    }
}