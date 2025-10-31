using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

[InitializeOnLoad]
public static class ScopedRegistry
{
    static ScopedRegistry()
    {
        AddScopedRegistries();
    }

    private static void AddScopedRegistries()
    {
        string manifestPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Packages", "manifest.json");
        if (!File.Exists(manifestPath)) return;

        string json = File.ReadAllText(manifestPath);
        var manifest = JObject.Parse(json);

        var scopedRegistries = manifest["scopedRegistries"] as JArray;
        if (scopedRegistries == null)
        {
            scopedRegistries = new JArray();
            manifest["scopedRegistries"] = scopedRegistries;
        }

        bool changed = false;

        void AddIfMissing(string name, string url, List<string> scopes)
        {
            bool exists = scopedRegistries.Any(r => r["url"]?.ToString() == url);
            if (exists) return;

            scopedRegistries.Add(new JObject
            {
                ["name"] = name,
                ["url"] = url,
                ["scopes"] = new JArray(scopes)
            });
            changed = true;
        }

        AddIfMissing("package.openupm.com", "https://package.openupm.com", new List<string>
        {
            "com.cysharp", "com.google", "com.gameanalytics","jp.hadashikick","com.coffee","com.google.external-dependency-manager"
        });

        AddIfMissing("AppLovin MAX Unity", "https://unity.packages.applovin.com/", new List<string>
        {
            "com.applovin.mediation.ads", "com.applovin.mediation.adapters","com.applovin.mediation.dsp"
        });

        if (changed)
        {
            File.WriteAllText(manifestPath, manifest.ToString());
            Debug.Log("Scoped Registries 자동 추가됨 (manifest.json 수정 완료)");
            string thisScriptPath = GetThisScriptPath();
            if (File.Exists(thisScriptPath))
            {
                File.Delete(thisScriptPath);
                File.Delete(thisScriptPath + ".meta");
                AssetDatabase.Refresh();
            }
        }
    }
    private static string GetThisScriptPath()
    {
        string[] paths = Directory.GetFiles(Application.dataPath, "ScopedRegistry.cs", SearchOption.AllDirectories);
        return paths.FirstOrDefault();
    }
}
