namespace Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entity;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    [Serializable]
    [JsonConverter(typeof(MapDataConverter))]
    public class MapData
    {
        public Dictionary<Vector3Int, Layers> Tiles { get; private set; } = new();

        public HashSet<Vector3Int> SpawnPointsA { get; private set; } = new();

        public HashSet<Vector3Int> SpawnPointsB { get; private set; } = new();
    }

    public class MapDataConverter : JsonConverter<MapData>
    {
        public override void WriteJson(JsonWriter writer, MapData value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(value.Tiles));
            writer.WriteRawValue(JsonConvert.SerializeObject(value.Tiles));

            writer.WritePropertyName(nameof(value.SpawnPointsA));
            writer.WriteRawValue(JsonConvert.SerializeObject(value.SpawnPointsA));

            writer.WritePropertyName(nameof(value.SpawnPointsB));
            writer.WriteRawValue(JsonConvert.SerializeObject(value.SpawnPointsB));

            writer.WriteEndObject();
        }

        public override MapData ReadJson(JsonReader reader, Type objectType, MapData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (hasExistingValue)
            {
                return existingValue;
            }

            var mapData = new MapData();

            var data = serializer.Deserialize<Dictionary<string, object>>(reader);

            foreach (var (key, value) in data)
            {
                switch (key)
                {
                    case nameof(mapData.Tiles):
                        foreach (var (tileKey, tileValue) in (JObject)value)
                        {
                            var tile  = tileKey[1..^1].Split(',').Select(int.Parse).ToArray();
                            var layer = (Layers)Enum.Parse(typeof(Layers), tileValue!.ToString());
                            mapData.Tiles.Add(new Vector3Int(tile[0], tile[1], tile[2]), layer);
                        }

                        break;
                    case nameof(mapData.SpawnPointsA):
                        foreach (var spawnPointA in (JArray)value)
                        {
                            var spawnPoint = JsonConvert.DeserializeObject<Vector3Int>(spawnPointA.ToString());
                            mapData.SpawnPointsA.Add(spawnPoint);
                        }

                        break;
                    case nameof(mapData.SpawnPointsB):
                        foreach (var spawnPointB in (JArray)value)
                        {
                            var spawnPoint = JsonConvert.DeserializeObject<Vector3Int>(spawnPointB.ToString());
                            mapData.SpawnPointsB.Add(spawnPoint);
                        }

                        break;
                }
            }

            return mapData;
        }
    }
}