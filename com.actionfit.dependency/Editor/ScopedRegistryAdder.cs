using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace com.actionfit.dependency.Editor
{
    [InitializeOnLoad]
    public static class ScopedRegistryAdder
    {
        static ScopedRegistryAdder()
        {
            string path = Path.Combine(EditorApplication.applicationContentsPath, "../../Packages/manifest.json");

            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var manifest = JObject.Parse(json);

            var scoped = manifest["scopedRegistries"] as JArray ?? new JArray();

            void AddIfNotExists(string name, string url, List<string> scopes)
            {
                if (scoped.Any(r => r["url"]?.ToString() == url)) return;

                scoped.Add(new JObject {
                    ["name"] = name,
                    ["url"] = url,
                    ["scopes"] = new JArray(scopes)
                });
            }

            AddIfNotExists("package.openupm.com", "https://package.openupm.com", new List<string> {
                "com.cysharp", "com.google", "com.gameanalytics"
            });

            AddIfNotExists("AppLovin MAX Unity", "https://unity.packages.applovin.com/", new List<string> {
                "com.applovin.mediation.ads", "com.applovin.mediation.adapters"
            });

            manifest["scopedRegistries"] = scoped;
            File.WriteAllText(path, manifest.ToString());
        }
    }
}