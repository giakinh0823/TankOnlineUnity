namespace UI
{

    using System.Collections.Generic;
    using System.Linq;
    using Map;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Tilemaps;
    using UnityEngine.UI;

    public class BuilderTools : MonoBehaviour
    {
        public static string ForceBuildMap { get; set; }

        [field: SerializeField]
        public List<Tile> Tiles { get; private set; }

        [field: SerializeField]
        public Tilemap Tilemap { get; private set; }

        [field: SerializeField]
        public MapBuilder MapBuilder { get; private set; }

        [field: SerializeField]
        public Button SaveButton { get; private set; }

        [field: SerializeField]
        public Button BackButton { get; private set; }

        [field: SerializeField]
        public Button PlayButton { get; private set; }

        [field: SerializeField]
        public TMP_InputField TextFieldMapName { get; private set; }

        private Dictionary<Tile, Button> TileToButton { get; } = new ();

        public Tile CurrentItem { get; private set; }

        private void Awake()
        {
            foreach (var tile in this.Tiles)
            {
                this.TileToButton.Add(tile, this.CreateItem(tile));
            }

            if (this.TileToButton.Any())
                this.TileToButton.FirstOrDefault().Value.onClick.Invoke();

            this.SaveButton.onClick.AddListener(this.OnClickSaveMap);
            this.PlayButton.onClick.AddListener(this.OnClickPlayMap);
            this.BackButton.onClick.AddListener(OnClickBack);

            if (!string.IsNullOrWhiteSpace(ForceBuildMap))
            {
                this.LoadMap(ForceBuildMap);
                ForceBuildMap = null;
            }
        }

        private void LoadMap(string mapName)
        {
            var (grid, spawnPosA, spawnPosB) = this.MapBuilder.Deserialize(MapDataUtils.GetMapData(mapName));

            var deserializedTilemaps = grid.GetComponentsInChildren<Tilemap>();

            foreach (var deserializedTilemap in deserializedTilemaps)
            {
                foreach (var cellPos in deserializedTilemap.cellBounds.allPositionsWithin)
                {
                    var tile = deserializedTilemap.GetTile(cellPos);
                    if (tile != null)
                    {
                        this.Tilemap.SetTile(cellPos, tile);
                    }
                }
            }

            foreach (var cellPos in spawnPosA.Select(pos => grid.WorldToCell(pos)))
            {
                this.Tilemap.SetTile(cellPos, this.MapBuilder.SpawnA);
            }

            foreach (var cellPos in spawnPosB.Select(pos => grid.WorldToCell(pos)))
            {
                this.Tilemap.SetTile(cellPos, this.MapBuilder.SpawnB);
            }

            this.TextFieldMapName.text = mapName;

            Destroy(grid.gameObject);
        }

        private static void OnClickBack()
        {
            SceneManager.LoadScene("GameplayScene");
        }

        private void OnClickPlayMap()
        {
            var mapData = this.MapBuilder.Serialize(this.Tilemap);

            if (!mapData.SpawnPointsA.Any() || !mapData.SpawnPointsB.Any())
            {
                return;
            }

            MapChooser.ForcePlayingMap = "Temp-Map";
            MapDataUtils.SaveMapData(MapChooser.ForcePlayingMap, mapData);
            SceneManager.LoadScene("GameplayScene");
        }

        private void OnClickSaveMap()
        {
            var mapName = this.TextFieldMapName.text;
            var mapData = this.MapBuilder.Serialize(this.Tilemap);

            if (string.IsNullOrWhiteSpace(mapName) || !mapData.SpawnPointsA.Any() || !mapData.SpawnPointsB.Any())
            {
                return;
            }

            MapDataUtils.SaveMapData(mapName, mapData);
        }

        private void Update()
        {
            this.Tilemap.ClearAllEditorPreviewTiles();

            if (Input.mousePosition.y < 200) return;

            var mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            var cellPos  = this.Tilemap.WorldToCell(mousePos);

            this.Tilemap.SetEditorPreviewTile(cellPos, this.CurrentItem);

            if (Input.GetMouseButton(0))
            {
                this.Tilemap.SetTile(cellPos, this.CurrentItem);
            }
            else if (Input.GetMouseButton(1))
            {
                this.Tilemap.SetTile(cellPos, null);
            }

            const float cameraMoveSensitive = 50.0f;
            const float cameraZoomSensitive = 30.0f;

            var editorCamera = Camera.main!;

            if (Input.GetMouseButton(2))
            {
                var mouseOffset = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                editorCamera.transform.position -= Time.deltaTime * cameraMoveSensitive * editorCamera.orthographicSize * mouseOffset;
            }

            editorCamera.orthographicSize = Mathf.Clamp(editorCamera.orthographicSize - Input.mouseScrollDelta.y * Time.deltaTime * cameraZoomSensitive, 3f, 30f);
        }

        private void OnClickButtonItem(Tile tile)
        {
            this.CurrentItem = tile;
            foreach (var (item, button) in this.TileToButton)
            {
                button.interactable = item != this.CurrentItem;
            }
        }

        private Button CreateItem(Tile tile)
        {
            var item = new GameObject(tile.name)
            {
                transform =
                {
                    parent        = this.transform,
                    localScale    = Vector3.one,
                    localPosition = Vector3.zero
                }
            };

            var image  = item.AddComponent<Image>();
            var button = item.AddComponent<Button>();

            image.sprite = tile.sprite;
            button.onClick.AddListener(() => this.OnClickButtonItem(tile));

            return button;
        }
    }

}