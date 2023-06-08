namespace Map
{
    using System.Collections.Generic;
    using System.Linq;
    using Entity;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class MapBuilder : MonoBehaviour
    {
        #region Serialize Fields

        [field: SerializeField]
        public Tile Brick { get; private set; }

        [field: SerializeField]
        public Tile Steel { get; private set; }

        [field: SerializeField]
        public Tile Water { get; private set; }

        [field: SerializeField]
        public Tile Grass { get; private set; }

        [field: SerializeField]
        public Tile SpawnA { get; private set; }

        [field: SerializeField]
        public Tile SpawnB { get; private set; }

        #endregion

        public MapData Serialize(Tilemap tilemap)
        {
            var result = new MapData();

            var tileToLayer = new Dictionary<TileBase, Layers>()
            {
                { this.Brick, Layers.Brick },
                { this.Steel, Layers.Steel },
                { this.Water, Layers.Water },
                { this.Grass, Layers.Grass },
            };


            tilemap.CompressBounds();
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(position);

                if (tile == null) continue;

                switch (tile)
                {
                    case var _ when tile == this.SpawnA:
                        result.SpawnPointsA.Add(position);
                        break;
                    case var _ when tile == this.SpawnB:
                        result.SpawnPointsB.Add(position);
                        break;
                    default:
                        if (tileToLayer.TryGetValue(tile, out var layer))
                        {
                            result.Tiles.Add(position, layer);
                        }

                        break;
                }
            }

            return result;
        }

        public (Grid map, HashSet<Vector3> spawnPosA, HashSet<Vector3> spawnPosB) Deserialize(MapData mapData)
        {
            var map = new GameObject("Map").AddComponent<Grid>();

            var layerToTilemap = new Dictionary<Layers, (Tilemap map, TileBase tile)>()
            {
                { Layers.Brick, (CreateSubMap(Layers.Brick, SortingLayers.BelowPlayer), this.Brick) },
                { Layers.Steel, (CreateSubMap(Layers.Steel, SortingLayers.BelowPlayer), this.Steel) },
                { Layers.Water, (CreateSubMap(Layers.Water, SortingLayers.BelowPlayer), this.Water) },
                { Layers.Grass, (CreateSubMap(Layers.Grass, SortingLayers.OnPlayer), this.Grass) },
            };

            foreach (var (position, layer) in mapData.Tiles)
            {
                layerToTilemap[layer].map.SetTile(
                    position,
                    layerToTilemap[layer].tile
                );
            }

            return (
                map,
                mapData.SpawnPointsA.Select(map.CellToWorld).ToHashSet(),
                mapData.SpawnPointsB.Select(map.CellToWorld).ToHashSet()
            );

            Tilemap CreateSubMap(Layers layer, SortingLayers sortingLayer)
            {
                var subMap = new GameObject(layer.ToString(),
                                            typeof(Tilemap),
                                            typeof(TilemapRenderer),
                                            typeof(TilemapCollider2D)
                )
                {
                    gameObject =
                    {
                        layer = (int)layer
                    },
                    transform =
                    {
                        parent = map.transform
                    }
                };

                subMap.GetComponent<TilemapRenderer>().sortingLayerName = sortingLayer.ToString();

                return subMap.GetComponent<Tilemap>();
            }
        }
    }
}