using UnityEditor;

namespace Assets.SimpleLocalization.Scripts.Editor
{
    [CustomEditor(typeof(LocalizationSettings))]
    public class LocalizationSettingsEditor : UnityEditor.Editor 
    {
        public override void OnInspectorGUI()
        {
            var settings = (LocalizationSettings) target;

            DrawDefaultInspector();
            settings.DisplayButtons();
            settings.DisplayWarnings();
        }
    }
}