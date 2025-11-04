#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ActionFit_Plugin.SDK.Max
{
    [CustomEditor(typeof(MaxAdsConfig))]
    public class MaxAdsConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MaxAdsConfig config = (MaxAdsConfig)target;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Test Device CSV 변환", EditorStyles.boldLabel);

            if (config.testDeviceCSV != null)
            {
                if (GUILayout.Button("📥 CSV에서 Test Device ID 가져오기"))
                {
                    string text = config.testDeviceCSV.text;
                    string[] lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                    var ids = new System.Collections.Generic.List<string>();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;
                        string[] parts = line.Split(',');
                        if (i == 0 && parts[0].ToLower().Contains("owner")) continue;
                        if (parts.Length > 1)
                        {
                            string id = parts[1].Trim();
                            if (!string.IsNullOrEmpty(id))
                                ids.Add(id);
                        }
                    }

                    Undo.RecordObject(config, "Import Test Keys");
                    config.maxTestDeviceIds = ids.ToArray();
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssets();

                    Debug.Log($"✅ {ids.Count}개의 Test Device ID가 설정되었습니다.");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("CSV를 위 필드에 드래그하세요.", MessageType.Info);
            }
        }
    }
}
#endif
