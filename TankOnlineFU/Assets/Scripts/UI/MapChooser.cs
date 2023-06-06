namespace UI
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Map;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class MapChooser : MonoBehaviour
    {
        public static string     ForcePlayingMap { get; set; }
        public static MapChooser Instance        { get; private set; }

        [field: SerializeField]
        public TMP_Dropdown Dropdown { get; private set; }

        [field: SerializeField]
        public Button Button { get; private set; }

        [field: SerializeField]
        public MapController MapController { get; private set; }

        private void Awake()
        {
            Instance = this;

            var mapNames = GetAllMapName();
            this.Dropdown.options = mapNames.Select(e => new TMP_Dropdown.OptionData(e)).ToList();

            this.Button.onClick.AddListener(this.OnClickStartGame);
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(ForcePlayingMap)) return;
            this.MapController.SpawnMap(ForcePlayingMap);
            this.gameObject.SetActive(false);
            ForcePlayingMap = null;
        }

        private void OnClickStartGame()
        {
            var mapName = this.Dropdown.options[this.Dropdown.value].text;
            this.MapController.SpawnMap(mapName);
            this.gameObject.SetActive(false);
        }

        private static IEnumerable<string> GetAllMapName()
        {
            return Resources.LoadAll<GameObject>("Maps/").Select(e => e.name).Where(e => e.StartsWith("Map-")).ToList();
        }
    }

}