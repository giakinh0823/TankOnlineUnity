namespace Map
{

    using System.Collections.Generic;
    using Photon.Pun;
    using Tank;
    using UnityEngine;

    public class MapController : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject TankPrefab { get; private set; }

        public Map CurrentMap { get; private set; }

        private void Awake()
        {
            this.CurrentMap = this.SpawnMap("Map-1");
        }

        private void Start()
        {
            var spawnedPosTeamA = new List<Vector3Int>(this.CurrentMap.SpawnPositionsTeamA);
            var spawnedPosTeamB = new List<Vector3Int>(this.CurrentMap.SpawnPositionsTeamB);

            var randomPos = GetRandomFromList(spawnedPosTeamA);

            var photonGameObject = PhotonNetwork.Instantiate(this.TankPrefab.name, this.CurrentMap.GetWorldPosition(randomPos), Quaternion.identity);
            FindObjectOfType<CameraController>().MainTank = photonGameObject.GetComponent<TankController>();

            T GetRandomFromList<T>(IReadOnlyList<T> list)
            {
                return list[Random.Range(0, list.Count)];
            }
        }

        public static string GetMapPath(string mapName)
        {
            return "Maps/" + mapName;
        }

        public Map SpawnMap(string mapName)
        {
            var mapPrefab = Resources.Load(GetMapPath(mapName));

            if (mapPrefab == null)
            {
                Debug.LogError("Map prefab not found");
                return null;
            }

            var mapGameObject = Instantiate(mapPrefab, this.transform);

            Resources.UnloadUnusedAssets();

            return (mapGameObject as GameObject)?.GetComponent<Map>();
        }
    }

}