namespace Map
{

#if UNITY_EDITOR
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
    }
#endif

}