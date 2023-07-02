namespace UI
{
    using System;
    using System.Collections;
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
    using Random = UnityEngine.Random;

    public class MapChooser : MonoBehaviour
    {
        public static string     ForcePlayingMap { get; set; }
        public static MapChooser Instance        { get; private set; }

        #region Serialize Fields

        [field: SerializeField] public GameObject      PlayerLife      { get; private set; }
        [field: SerializeField] public GameObject      PlayerScore     { get; private set; }
        [field: SerializeField] public GameObject      PlayerResult    { get; private set; }
        [field: SerializeField] public TMP_Dropdown    Dropdown        { get; private set; }
        [field: SerializeField] public Toggle          LifeModeToggle  { get; private set; }
        [field: SerializeField] public Toggle          ScoreModeToggle { get; private set; }
        [field: SerializeField] public Button          PlayButton      { get; private set; }
        [field: SerializeField] public Button          CreateButton    { get; private set; }
        [field: SerializeField] public Button          EditButton      { get; private set; }
        [field: SerializeField] public Button          DeleteButton    { get; private set; }
        [field: SerializeField] public MapBuilder      MapBuilder      { get; private set; }
        [field: SerializeField] public GameObject      TankA           { get; private set; }
        [field: SerializeField] public GameObject      TankB           { get; private set; }
        [field: SerializeField] public RectTransform   TankALife       { get; private set; }
        [field: SerializeField] public RectTransform   TankBLife       { get; private set; }
        [field: SerializeField] public GameObject      Heart           { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Message         { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TankAScore      { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TankBScore      { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TimeLeft        { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Result          { get; private set; }
        [field: SerializeField] public Button          NextButton      { get; private set; }

        #endregion

        public Grid             CurrentMap { get; private set; }
        public HashSet<Vector3> SpawnPosA  { get; private set; }
        public HashSet<Vector3> SpawnPosB  { get; private set; }

        private       GameObject tankA;
        private       GameObject tankB;
        private const int        MaxLife   = 9999;
        private const int        TimeLimit = 10;
        private       float      timeLeft;
        private       bool       isStartGame;

        private void Awake()
        {
            Instance = this;

            this.Refresh();

            this.Dropdown.onValueChanged.AddListener(this.OnDropdownValueChanged);
            this.PlayButton.onClick.AddListener(this.OnClickStartGame);
            this.CreateButton.onClick.AddListener(OnClickCreateMap);
            this.EditButton.onClick.AddListener(this.OnClickEditMap);
            this.DeleteButton.onClick.AddListener(this.OnClickDeleteMap);
            this.NextButton.onClick.AddListener(this.OnClickNext);
            this.LifeModeToggle.isOn = this.ScoreModeToggle.isOn = false;
        }

        private void Update()
        {
            if (!this.isStartGame) return;
            if (this.LifeModeToggle.isOn)
            {
                if (this.tankA != null && this.TankALife.childCount > this.tankA.GetComponent<TankController>().Life)
                {
                    Destroy(this.TankALife.GetChild(this.TankALife.childCount - 1).gameObject);
                    if (this.TankALife.childCount - 1 > 0)
                    {
                        Destroy(this.tankA.gameObject);
                        this.SpawnTankA(this.TankALife.childCount - 1);
                    }
                }

                if (this.tankB != null && this.TankBLife.childCount > this.tankB.GetComponent<TankController>().Life)
                {
                    Destroy(this.TankBLife.GetChild(this.TankBLife.childCount - 1).gameObject);
                    if (this.TankBLife.childCount - 1 > 0)
                    {
                        Destroy(this.tankB.gameObject);
                        this.SpawnTankB(this.TankBLife.childCount - 1);
                    }
                }
            }
            else
            {
                this.timeLeft      -= Time.deltaTime;
                this.TimeLeft.text =  $"{(int)this.timeLeft}s";
                if (this.timeLeft <= 0)
                {
                    this.isStartGame = false;
                    this.Result.text = int.Parse(this.TankAScore.text) > int.Parse(this.TankBScore.text) ? "Player1 Win" : int.Parse(this.TankAScore.text) < int.Parse(this.TankBScore.text) ? "Player2 Win" : "Draw";
                    this.Result.transform.parent.gameObject.SetActive(true);
                }

                if (this.tankA != null && this.tankA.GetComponent<TankController>().Life + int.Parse(this.TankBScore.text) < MaxLife)
                {
                    this.TankBScore.text = $"{int.Parse(this.TankBScore.text) + 1}";
                    Destroy(this.tankA.gameObject);
                    this.SpawnTankA(MaxLife - int.Parse(this.TankBScore.text));
                }

                if (this.tankB != null && this.tankB.GetComponent<TankController>().Life + int.Parse(this.TankAScore.text) < MaxLife)
                {
                    this.TankAScore.text = $"{int.Parse(this.TankAScore.text) + 1}";
                    Destroy(this.tankB.gameObject);
                    this.SpawnTankB(MaxLife - int.Parse(this.TankAScore.text));
                }
            }
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
            if (!this.LifeModeToggle.isOn && !this.ScoreModeToggle.isOn)
            {
                this.Message.text = "Please select a game mode";
                return;
            }

            if (this.LifeModeToggle.isOn && this.ScoreModeToggle.isOn)
            {
                this.Message.text = "Please select only one game mode";
                return;
            }

            var mapName = this.Dropdown.options[this.Dropdown.value].text;
            this.SpawnMap(mapName);
        }

        private void SpawnMap(string mapName)
        {
            this.PreSpawnMap();
            (this.CurrentMap, this.SpawnPosA, this.SpawnPosB) = this.MapBuilder.Deserialize(MapDataUtils.GetMapData(mapName));
            this.PostSpawnMap();

            for (var i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (this.LifeModeToggle.isOn)
            {
                this.PlayerLife.SetActive(true);
                for (var i = 0; i < this.tankA.GetComponent<TankController>().Life; i++)
                {
                    var heartA = Instantiate(this.Heart, this.TankALife);
                    heartA.GetComponent<RectTransform>().localPosition = new Vector3(-75 + i * 75, 25, 0);
                    var heartB = Instantiate(this.Heart, this.TankBLife);
                    heartB.GetComponent<RectTransform>().localPosition = new Vector3(-75 - i * 75, 25, 0);
                }
            }
            else
            {
                this.timeLeft = TimeLimit;
                this.PlayerScore.SetActive(true);
                this.TankAScore.text = "0";
                this.TankBScore.text = "0";
            }
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
            if (this.LifeModeToggle.isOn)
            {
                this.SpawnTankA(3);
                this.SpawnTankB(3);
            }
            else
            {
                this.SpawnTankA(MaxLife);
                this.SpawnTankB(MaxLife);
            }

            var bounds = new Bounds();

            foreach (var tilemap in this.CurrentMap.GetComponentsInChildren<Tilemap>())
            {
                bounds.Encapsulate(new Bounds(tilemap.cellBounds.center, tilemap.cellBounds.size));
            }

            FindObjectOfType<CameraController>().WrapBounds(bounds);

            this.isStartGame = true;
        }

        private void SpawnTankA(int life)
        {
            var randomPosA = this.SpawnPosA.ElementAt(Random.Range(0, this.SpawnPosA.Count));

            this.tankA = Instantiate(this.TankA, randomPosA, Quaternion.identity);
            this.tankA.GetComponent<TankController>().Keymap = new ControlKeymap()
            {
                Up    = KeyCode.W,
                Down  = KeyCode.S,
                Left  = KeyCode.A,
                Right = KeyCode.D,
                Fire  = KeyCode.Space,
            };
            this.tankA.GetComponent<TankController>().Life = life;
        }

        private void SpawnTankB(int life)
        {
            var randomPosB = this.SpawnPosB.ElementAt(Random.Range(0, this.SpawnPosB.Count));

            this.tankB = Instantiate(this.TankB, randomPosB, Quaternion.identity);
            this.tankB.GetComponent<TankController>().Keymap = new ControlKeymap()
            {
                Up    = KeyCode.UpArrow,
                Down  = KeyCode.DownArrow,
                Left  = KeyCode.LeftArrow,
                Right = KeyCode.RightArrow,
                Fire  = KeyCode.RightShift,
            };
            this.tankB.GetComponent<TankController>().Life = life;
        }

        private void Refresh()
        {
            var mapNames = MapDataUtils.GetAllMapName();
            this.Dropdown.options = mapNames.Select(e => new TMP_Dropdown.OptionData(e)).ToList();

            this.OnDropdownValueChanged(0);
        }

        private void OnClickNext()
        {
            for (var i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);
            }

            this.PlayerLife.SetActive(false);
            this.PlayerScore.SetActive(false);
            this.PlayerResult.SetActive(false);
            this.ScoreModeToggle.isOn = false;
            this.Message.text         = "";
            this.isStartGame          = false;
        }

        public void LifeModeEnd()
        {
            for (var i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);
            }

            this.PlayerLife.SetActive(false);
            this.PlayerScore.SetActive(false);
            this.PlayerResult.SetActive(false);
            this.LifeModeToggle.isOn = false;
            this.Message.text        = "";
            this.isStartGame         = false;
        }
    }
}