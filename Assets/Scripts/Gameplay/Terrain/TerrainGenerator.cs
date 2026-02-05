using UnityEngine;

namespace BigBoom.Gameplay.Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private DestructibleTerrain destructibleTerrain;
        [SerializeField, Min(64)] private int width = 1024;
        [SerializeField, Min(64)] private int height = 512;
        [SerializeField] private float noiseScale = 0.01f;
        [SerializeField] private float baseHeight = 0.4f;

        public void GenerateRoundTerrain()
        {
            destructibleTerrain.Generate(width, height, noiseScale, baseHeight);
        }
    }
}
