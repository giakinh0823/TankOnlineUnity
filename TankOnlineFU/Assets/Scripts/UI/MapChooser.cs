namespace UI
{

    using System.Collections.Generic;
    using System.Linq;
    using Entity;
    using Map;
    using Tank;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Tilemaps;
    using UnityEngine.UI;

    public class MapChooser : MonoBehaviour
    {
        public static string     ForcePlayingMap { get; set; }
        public static MapChooser Instance        { get; private set; }

        #region Serialize Fields

        [field: SerializeField]
        public TMP_Dropdown Dropdown { get; private set; }

        [field: SerializeField]
        public Button PlayButton { get; private set; }

        [field: SerializeField]
        public Button CreateButton { get; private set; }

        [field: SerializeField]
        public Button EditButton { get; private set; }

        [field: SerializeField]
        public Button DeleteButton { get; private set; }

        [field: SerializeField]
        public MapBuilder MapBuilder { get; private set; }

        [field: SerializeField]
        public GameObject TankA { get; private set; }

        [field: SerializeField]
        public GameObject TankB { get; private set; }

        #endregion

        public Grid             CurrentMap { get; private set; }
        public HashSet<Vector3> SpawnPosA  { get; private set; }
        public HashSet<Vector3> SpawnPosB  { get; private set; }

        private void Awake()
        {
            Instance = this;

            this.Refresh();

            this.Dropdown.onValueChanged.AddListener(this.OnDropdownValueChanged);
            this.PlayButton.onClick.AddListener(this.OnClickStartGame);
            this.CreateButton.onClick.AddListener(OnClickCreateMap);
            this.EditButton.onClick.AddListener(this.OnClickEditMap);
            this.DeleteButton.onClick.AddListener(this.OnClickDeleteMap);
        }
        private void OnDropdownValueChanged(int _)
        {
            if (this.Dropdown.options.Count == 0)
            {
                this.PlayButton.interactable   = false;
                this.EditButton.interactable   = false;
                this.DeleteButton.interactable = false;
                return;
            }

            if (this.Dropdown.options[this.Dropdown.value] is not null)
            {
                this.PlayButton.interactable   = true;
                this.EditButton.interactable   = true;
                this.DeleteButton.interactable = true;
            }
        }

        private static void OnClickCreateMap()
        {
            SceneManager.LoadScene("MapBuilderScene");
        }

        private void OnClickEditMap()
        {
            BuilderTools.ForceBuildMap = this.Dropdown.options[this.Dropdown.value].text;
            SceneManager.LoadScene("MapBuilderScene");
        }

        private void OnClickDeleteMap()
        {
            var mapName = this.Dropdown.options[this.Dropdown.value].text;
            MapDataUtils.SaveMapData(mapName, null);
            this.Refresh();
            this.Dropdown.value = 0;
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(ForcePlayingMap)) return;
            this.SpawnMap(ForcePlayingMap);
            ForcePlayingMap = null;
        }

        private void OnClickStartGame()
        {
            var mapName = this.Dropdown.options[this.Dropdown.value].text;
            this.SpawnMap(mapName);
        }

        private void SpawnMap(string mapName)
        {
            this.PreSpawnMap();
            (this.CurrentMap, this.SpawnPosA, this.SpawnPosB) = this.MapBuilder.Deserialize(MapDataUtils.GetMapData(mapName));
            this.PostSpawnMap();
            this.gameObject.SetActive(false);
        }

        private void PreSpawnMap()
        {
            if (!this.CurrentMap) return;
            Destroy(this.CurrentMap.gameObject);
            foreach (var tankController in FindObjectsOfType<TankController>())
            {
                Destroy(tankController.gameObject);
            }
        }

        private void PostSpawnMap()
        {
            var randomPosA = this.SpawnPosA.ElementAt(Random.Range(0, this.SpawnPosA.Count));
            var randomPosB = this.SpawnPosB.ElementAt(Random.Range(0, this.SpawnPosB.Count));

            Instantiate(this.TankA, randomPosA, Quaternion.identity).GetComponent<TankController>().Keymap = new ControlKeymap()
            {
                Up    = KeyCode.W,
                Down  = KeyCode.S,
                Left  = KeyCode.A,
                Right = KeyCode.D,
                Fire  = KeyCode.Space,
            };

            Instantiate(this.TankB, randomPosB, Quaternion.identity).GetComponent<TankController>().Keymap = new ControlKeymap()
            {
                Up    = KeyCode.UpArrow,
                Down  = KeyCode.DownArrow,
                Left  = KeyCode.LeftArrow,
                Right = KeyCode.RightArrow,
                Fire  = KeyCode.RightShift,
            };

            var bounds = new Bounds();

            foreach (var tilemap in this.CurrentMap.GetComponentsInChildren<Tilemap>())
            {
                bounds.Encapsulate(new Bounds(tilemap.cellBounds.center, tilemap.cellBounds.size));
            }

            FindObjectOfType<CameraController>().WrapBounds(bounds);
        }

        private void Refresh()
        {
            var mapNames = MapDataUtils.GetAllMapName();
            this.Dropdown.options = mapNames.Select(e => new TMP_Dropdown.OptionData(e)).ToList();

            this.OnDropdownValueChanged(0);
        }
    }

}