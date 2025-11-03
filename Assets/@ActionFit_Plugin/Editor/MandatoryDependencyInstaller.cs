#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ActionFit_Plugin.Editor
{
    public class MandatoryDependencyInstaller : EditorWindow
    {
        private Vector2 _scroll;

        private class PackageInfoUI
        {
            public string Name;
            public string Description;
            public string Source;
            public string Status = "대기 중";
            public bool IsInstalling = false;
            public bool IsInstalled = false;
            public AddRequest Request;
        }

        private List<PackageInfoUI> _packageList = new();
        private ListRequest _listRequest;

        // 🔄 전체 다운로드 기능용 변수
        private bool _isBatchInstalling = false;
        private Queue<PackageInfoUI> _batchQueue = new();

        [MenuItem("ActionFit/Mandatory/Installer")]
        public static void ShowWindow()
        {
            GetWindow<MandatoryDependencyInstaller>("필수 패키지");
        }

        private void OnEnable()
        {
            _packageList = new List<PackageInfoUI>
            {
                new()
                {
                    Name = "* Vibration",
                    Description = "진동 기능 제어 플러그인.",
                    Source = "https://github.com/BenoitFreslon/Vibration.git"
                },
                new()
                {
                    Name = "* JSAM (Audio Manager)",
                    Description = "간단한 오디오 매니저 플러그인",
                    Source = "https://github.com/jackyyang09/Simple-Unity-Audio-Manager.git#master"
                },
                new()
                {
                    Name = "* UniTask",
                    Description = "C# async/await를 최적화한 유틸리티",
                    Source = "com.cysharp.unitask"
                },
                new()
                {
                    Name = "* Addressables",
                    Description = "Unity의 에셋 관리 시스템",
                    Source = "com.unity.addressables"
                },
                new()
                {
                    Name = "* Localization",
                    Description = "Unity Localize 시스템",
                    Source = "com.unity.localization"
                }
            };

            _listRequest = Client.List(true);
            EditorApplication.update += WaitForPackageList;
        }

        private void OnDisable()
        {
            EditorApplication.update -= WaitForPackageList;
            EditorApplication.update -= UpdateProgress;
        }

        private void WaitForPackageList()
        {
            if (!_listRequest.IsCompleted) return;

            EditorApplication.update -= WaitForPackageList;

            foreach (var pkg in _packageList)
            {
                if (IsPackageAlreadyInstalled(pkg.Source, _listRequest.Result))
                {
                    pkg.Status = "설치됨"; 
                    pkg.IsInstalled = true;
                }
            }

            EditorApplication.update += UpdateProgress;
            Repaint();
        }

        private bool IsPackageAlreadyInstalled(string pkgSource, IEnumerable<UnityEditor.PackageManager.PackageInfo> installed)
        {
            foreach (var p in installed)
            {
                if (pkgSource.Contains(p.name) || p.packageId.Contains(pkgSource) || pkgSource == p.name)
                    return true;
            }
            return false;
        }

        private void OnGUI()
        {
            // 🔹 상단 타이틀 + 전체 다운로드 버튼
            GUILayout.BeginHorizontal();
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };
            GUILayout.Label("필수 패키지 설치기", titleStyle);

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(_isBatchInstalling))
            {
                if (GUILayout.Button("전체 다운로드", GUILayout.Width(120), GUILayout.Height(30)))
                {
                    StartBatchInstallation();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            _scroll = GUILayout.BeginScrollView(_scroll);

            foreach (var pkg in _packageList)
            {
                GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(0, 0, 5, 5),
                    fixedHeight = 100
                };

                GUILayout.BeginVertical(boxStyle);
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 13,
                };
                GUILayout.Label(pkg.Name, nameStyle);

                GUIStyle descStyle = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    margin = new RectOffset(0, 0, 0, 20)
                };
                GUILayout.Label(pkg.Description, descStyle);
                GUILayout.Label(pkg.Source, EditorStyles.miniLabel);

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical(GUILayout.Width(100));
                GUILayout.FlexibleSpace();

                if (!pkg.IsInstalling && !pkg.IsInstalled && pkg.Status == "대기 중")
                {
                    GUIStyle installButtonStyle = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                        fixedHeight = 24,
                        normal = {
                            textColor = Color.white,
                            background = MakeColorTexture(new Color(0.10f, 0.55f, 1f))
                        },
                        hover = {
                            textColor = Color.white,
                            background = MakeColorTexture(new Color(0.20f, 0.65f, 1.1f))
                        },
                        active = {
                            textColor = Color.white,
                            background = MakeColorTexture(new Color(0.05f, 0.45f, 0.9f))
                        },
                        border = new RectOffset(4, 4, 4, 4),
                        margin = new RectOffset(0, 0, 4, 4),
                        padding = new RectOffset(4, 4, 2, 2)
                    };

                    if (GUILayout.Button("Install", installButtonStyle))
                    {
                        pkg.Status = "⏳ 설치 중";
                        pkg.IsInstalling = true;
                        pkg.Request = Client.Add(pkg.Source);
                    }
                }
                else
                {
                    GUIStyle statusStyle = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        fixedHeight = 24,
                        normal = {
                            textColor = Color.white,
                            background = MakeColorTexture(Color.gray)
                        }
                    };

                    GUILayout.Label(pkg.Status, statusStyle, GUILayout.Height(24));
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
        }

        private void UpdateProgress()
        {
            foreach (var pkg in _packageList)
            {
                if (pkg.IsInstalling && pkg.Request != null && pkg.Request.IsCompleted)
                {
                    pkg.IsInstalling = false;
                    pkg.IsInstalled = true;

                    if (pkg.Request.Status == StatusCode.Success)
                        pkg.Status = "설치됨";
                    else
                        pkg.Status = "실패";

                    pkg.Request = null;
                    Repaint();

                    // 🔄 배치 설치 중이면 다음 패키지 자동 설치
                    if (_isBatchInstalling)
                        InstallNextFromBatch();
                }
            }
        }

        private void StartBatchInstallation()
        {
            _isBatchInstalling = true;
            _batchQueue.Clear();

            foreach (var pkg in _packageList)
            {
                if (!pkg.IsInstalled && !pkg.IsInstalling && pkg.Status == "대기 중")
                    _batchQueue.Enqueue(pkg);
            }

            InstallNextFromBatch();
        }

        private void InstallNextFromBatch()
        {
            if (_batchQueue.Count == 0)
            {
                _isBatchInstalling = false;
                return;
            }

            var pkg = _batchQueue.Dequeue();
            pkg.Status = "⏳ 설치 중";
            pkg.IsInstalling = true;
            pkg.Request = Client.Add(pkg.Source);
        }

        private static Texture2D MakeColorTexture(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
    }
}
#endif
