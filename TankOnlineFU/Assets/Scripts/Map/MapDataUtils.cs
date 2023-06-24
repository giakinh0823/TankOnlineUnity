namespace Map
{

    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using UnityEngine;

    public static class MapDataUtils
    {
        public const string MapDataPrefix = "MD";

        public static HashSet<string> GetAllMapName()
        {
            var keyMaps = $"{MapDataPrefix}-Maps";

            try
            {
                if (PlayerPrefs.HasKey(keyMaps))
                {
                    var valueMaps = PlayerPrefs.GetString(keyMaps);
                    return JsonConvert.DeserializeObject<HashSet<string>>(valueMaps);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            PlayerPrefs.SetString(keyMaps, "[]");
            PlayerPrefs.Save();
            return new HashSet<string>();
        }

        public static MapData GetMapData(string name)
        {
            var keyMapData = $"{MapDataPrefix}-Map-{name}";

            if (!PlayerPrefs.HasKey(keyMapData)) return null;

            var valueMapData = PlayerPrefs.GetString(keyMapData);
            return JsonConvert.DeserializeObject<MapData>(valueMapData);
        }

        public static void SaveMapData(string name, MapData mapData)
        {
            var keyMaps      = $"{MapDataPrefix}-Maps";
            var keyMapData   = $"{MapDataPrefix}-Map-{name}";
            var valueMapData = JsonConvert.SerializeObject(mapData);
            var allMapNames  = GetAllMapName();

            if (mapData is not null)
            {
                allMapNames.Add(name);
                PlayerPrefs.SetString(keyMapData, valueMapData);
            }
            else
            {
                allMapNames.Remove(name);
                PlayerPrefs.DeleteKey(keyMapData);
            }

            PlayerPrefs.SetString(keyMaps, JsonConvert.SerializeObject(allMapNames));
            PlayerPrefs.Save();
        }
    }

}