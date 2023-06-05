namespace Map
{

#if UNITY_EDITOR
    using UI;
    using UnityEditor;
    using UnityEngine;

    public class CreateMapUtils
    {
        [MenuItem("Assets/Create/Map")]
        public static void CreateMap()
        {
            var prefab = Resources.Load<GameObject>(MapController.GetMapPath("MapBase"));
            var clone  = Object.Instantiate(prefab);
            var path = EditorUtility.SaveFilePanelInProject(
                "Save Map",
                "Map-0",
                "prefab",
                "Save Map"
            );

            if (string.IsNullOrEmpty(path)) return;

            PrefabUtility.SaveAsPrefabAsset(clone, path);
            Object.DestroyImmediate(clone);
        }

        [CustomEditor(typeof(Map), true)]
        public class MapInspector : Editor
        {
            public Map Map => (Map)this.target;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.Space(20);

                if (GUILayout.Button("Play this map"))
                {
                    MapChooser.ForcePlayingMap = this.Map.gameObject.name;
                    EditorApplication.EnterPlaymode();
                }
            }
        }
    }
#endif

}