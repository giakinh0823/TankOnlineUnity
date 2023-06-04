namespace Map
{

    using System.Collections.Generic;
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

            foreach (var pos in this.Tilemap.cellBounds.allPositionsWithin)
            {
                var tile = this.Tilemap.GetTile(pos);

                if (tile == this.TeamASpawnTile)
                {
                    spawnPositionsTeamA.Add(pos);
                    this.Tilemap.SetTile(pos, null);
                }
                else if (tile == this.TeamBSpawnTile)
                {
                    spawnPositionsTeamB.Add(pos);
                    this.Tilemap.SetTile(pos, null);
                }
            }

            this.SpawnPositionsTeamA = spawnPositionsTeamA.ToArray();
            this.SpawnPositionsTeamB = spawnPositionsTeamB.ToArray();
        }
        public Vector3 GetWorldPosition(Vector3Int randomPos)
        {
            return this.Tilemap.CellToWorld(randomPos);
        }
    }

}