namespace Map
{

    using System.Collections.Generic;
    using Entity;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    [RequireComponent(typeof(TilemapCollider2D))]
    public class Map : MonoBehaviour
    {
        [field: SerializeField]
        public Tilemap Tilemap { get; private set; }

        [field: SerializeField]
        public Tile TeamASpawnTile { get; private set; }

        [field: SerializeField]
        public Tile TeamBSpawnTile { get; private set; }

        [field: SerializeField]
        public Tile BrickTile { get; private set; }

        [field: SerializeField]
        public Tile GrassTile { get; private set; }

        [field: SerializeField]
        public Tile WaterTile { get; private set; }

        [field: SerializeField]
        public Tile RockTile { get; private set; }

        public Vector3Int[] SpawnPositionsTeamA { get; private set; }
        public Vector3Int[] SpawnPositionsTeamB { get; private set; }

        private void Awake()
        {
            this.Tilemap ??= this.GetComponent<Tilemap>();

            this.AnalysisData();
        }

        public void AnalysisData()
        {
            var spawnPositionsTeamA = new List<Vector3Int>();
            var spawnPositionsTeamB = new List<Vector3Int>();

            var brickTilemap = this.CreateSubTiles("Brick", Layers.Brick, SortingLayer.NameToID("BelowPlayer"), 0);
            var grassTilemap = this.CreateSubTiles("Grass", Layers.Bush, SortingLayer.NameToID("OnPlayer"), 0);
            var waterTilemap = this.CreateSubTiles("Water", Layers.Water, SortingLayer.NameToID("BelowPlayer"), 0);
            var rockTilemap  = this.CreateSubTiles("Rock", Layers.Rock, SortingLayer.NameToID("BelowPlayer"), 0);

            foreach (var pos in this.Tilemap.cellBounds.allPositionsWithin)
            {
                var tile = this.Tilemap.GetTile(pos);

                switch (tile)
                {
                    case var _ when tile == this.TeamASpawnTile:
                        spawnPositionsTeamA.Add(pos);
                        break;
                    case var _ when tile == this.TeamBSpawnTile:
                        spawnPositionsTeamB.Add(pos);
                        break;
                    case var _ when tile == this.BrickTile:
                        brickTilemap.SetTile(pos, tile);
                        break;
                    case var _ when tile == this.GrassTile:
                        grassTilemap.SetTile(pos, tile);
                        break;
                    case var _ when tile == this.WaterTile:
                        waterTilemap.SetTile(pos, tile);
                        break;
                    case var _ when tile == this.RockTile:
                        rockTilemap.SetTile(pos, tile);
                        break;
                }

                this.Tilemap.SetTile(pos, null);
            }

            this.SpawnPositionsTeamA = spawnPositionsTeamA.ToArray();
            this.SpawnPositionsTeamB = spawnPositionsTeamB.ToArray();
        }

        public Vector3 GetWorldPosition(Vector3Int randomPos)
        {
            return this.Tilemap.CellToWorld(randomPos);
        }

        private Tilemap CreateSubTiles(string mapName, Layers layer, int sortingLayerId, int sortingInLayer)
        {
            var subTiles = new GameObject
            {
                gameObject =
                {
                    layer = (int)layer,
                    name  = mapName
                },
                transform =
                {
                    parent = this.transform
                }
            };

            var tileMap         = subTiles.AddComponent<Tilemap>();
            var tileMapRenderer = subTiles.AddComponent<TilemapRenderer>();
            subTiles.AddComponent<TilemapCollider2D>();

            tileMapRenderer.sortingLayerID = sortingLayerId;
            tileMapRenderer.sortingOrder   = sortingInLayer;

            return tileMap;
        }
    }

}