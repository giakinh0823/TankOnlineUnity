namespace Map
{

    using Entity;
    using Tank;
    using UnityEngine;

    public class MapController : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject TankPrefabA { get; private set; }

        [field: SerializeField]
        public GameObject TankPrefabB { get; private set; }

        public Map CurrentMap { get; private set; }

        private void PostSpawnMap()
        {
            var randomPosA = this.CurrentMap.SpawnPositionsTeamA[
                Random.Range(0, this.CurrentMap.SpawnPositionsTeamA.Length)
            ];

            var randomPosB = this.CurrentMap.SpawnPositionsTeamB[
                Random.Range(0, this.CurrentMap.SpawnPositionsTeamB.Length)
            ];

            var tankA = Instantiate(
                this.TankPrefabA,
                this.CurrentMap.GetWorldPosition(randomPosA),
                Quaternion.identity
            ).GetComponent<TankController>();

            var tankB = Instantiate(
                this.TankPrefabB,
                this.CurrentMap.GetWorldPosition(randomPosB),
                Quaternion.identity
            ).GetComponent<TankController>();

            tankA.Keymap = new ControlKeymap
            {
                Up    = KeyCode.W,
                Down  = KeyCode.S,
                Left  = KeyCode.A,
                Right = KeyCode.D,
                Fire  = KeyCode.Space
            };

            tankB.Keymap = new ControlKeymap
            {
                Up    = KeyCode.UpArrow,
                Down  = KeyCode.DownArrow,
                Left  = KeyCode.LeftArrow,
                Right = KeyCode.RightArrow,
                Fire  = KeyCode.RightControl
            };

            FindObjectOfType<CameraController>().WrapBounds(this.CurrentMap.Tilemap.cellBounds);
        }

        private void PreSpawnMap()
        {
            foreach (Transform children in this.transform)
            {
                Destroy(children.gameObject);
            }

            foreach (var tankController in FindObjectsOfType<TankController>()) Destroy(tankController.gameObject);
        }

        public static string GetMapPath(string mapName)
        {
            return "Maps/" + mapName;
        }

        public Map SpawnMap(string mapName)
        {
            this.PreSpawnMap();

            var mapPrefab = Resources.Load(GetMapPath(mapName));

            if (mapPrefab == null)
            {
                Debug.LogError("Map prefab not found");
                return null;
            }

            var mapGameObject = Instantiate(mapPrefab, this.transform);

            Resources.UnloadUnusedAssets();

            this.CurrentMap = (mapGameObject as GameObject)?.GetComponent<Map>();

            this.PostSpawnMap();

            return this.CurrentMap;
        }
    }

}