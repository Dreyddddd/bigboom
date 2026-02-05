using UnityEngine;

namespace BigBoom.Gameplay.Terrain
{
    [RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D))]
    public class DestructibleTerrain : MonoBehaviour
    {
        [SerializeField] private Texture2D alphaMask;
        [SerializeField] private float pixelsPerUnit = 32f;

        private SpriteRenderer _spriteRenderer;
        private PolygonCollider2D _polygonCollider;
        private Texture2D _runtimeTexture;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
        }

        public void Generate(int width, int height, float noiseScale, float baseHeight)
        {
            _runtimeTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            _runtimeTexture.filterMode = FilterMode.Point;

            for (var x = 0; x < width; x++)
            {
                var noise = Mathf.PerlinNoise(x * noiseScale, 0f);
                var groundHeight = Mathf.RoundToInt((baseHeight + noise * 0.35f) * height);

                for (var y = 0; y < height; y++)
                {
                    var isSolid = y <= groundHeight;
                    var color = isSolid ? Color.green : Color.clear;

                    if (alphaMask != null && isSolid)
                    {
                        color *= alphaMask.GetPixelBilinear((float)x / width, (float)y / height);
                    }

                    _runtimeTexture.SetPixel(x, y, color);
                }
            }

            _runtimeTexture.Apply();
            var sprite = Sprite.Create(_runtimeTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            _spriteRenderer.sprite = sprite;
            _polygonCollider.pathCount = 1;
            _polygonCollider.SetPath(0, new[]
            {
                new Vector2(-width / pixelsPerUnit / 2f, -height / pixelsPerUnit / 2f),
                new Vector2(-width / pixelsPerUnit / 2f, height / pixelsPerUnit / 2f),
                new Vector2(width / pixelsPerUnit / 2f, height / pixelsPerUnit / 2f),
                new Vector2(width / pixelsPerUnit / 2f, -height / pixelsPerUnit / 2f)
            });
        }

        public void CarveCircle(Vector2 worldPosition, float radius)
        {
            if (_runtimeTexture == null)
            {
                return;
            }

            var local = transform.InverseTransformPoint(worldPosition);
            var textureCenter = new Vector2(_runtimeTexture.width / 2f, _runtimeTexture.height / 2f) + local * pixelsPerUnit;
            var pixelRadius = Mathf.RoundToInt(radius * pixelsPerUnit);

            for (var x = -pixelRadius; x <= pixelRadius; x++)
            {
                for (var y = -pixelRadius; y <= pixelRadius; y++)
                {
                    if (x * x + y * y > pixelRadius * pixelRadius)
                    {
                        continue;
                    }

                    var px = Mathf.RoundToInt(textureCenter.x + x);
                    var py = Mathf.RoundToInt(textureCenter.y + y);
                    if (px < 0 || py < 0 || px >= _runtimeTexture.width || py >= _runtimeTexture.height)
                    {
                        continue;
                    }

                    _runtimeTexture.SetPixel(px, py, Color.clear);
                }
            }

            _runtimeTexture.Apply();
            // Production: rebuild collider from alpha contour. Here simplified for project baseline.
        }
    }
}
