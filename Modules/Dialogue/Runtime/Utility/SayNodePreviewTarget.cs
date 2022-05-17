using TMPro;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    public class SayNodePreviewTarget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        public void SetText(string text)
        {
            label.text = text;
            EditorUtility.SetDirty(label);
        }
    }
}