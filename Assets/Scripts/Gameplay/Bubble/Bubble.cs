using UnityEngine;

namespace Assets.Scripts.Gameplay.Bubble
{
    public class Bubble : MonoBehaviour
    {
        public Color Color { get; private set; }
        public Vector2Int GridCoordinates { get; set; }
        public bool inGrid = false;

        private MeshRenderer _meshRenderer;
        private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();

            if (_meshRenderer != null)
                _meshRenderer.material = new Material(_meshRenderer.material);
            
            else
                Debug.LogError("MeshRenderer component not found on Bubble GameObject.");
            
        }

        public void SetColor()
        {
            Color = colors[Random.Range(0, colors.Length)]; ;
            if (_meshRenderer == null)
                _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer != null)
            {
                if (_meshRenderer.material == null || _meshRenderer.material.name.EndsWith("(Instance)") == false)
                    _meshRenderer.material = new Material(_meshRenderer.sharedMaterial);

                _meshRenderer.material.color = Color;

            }
        }
        public void SetGridCoordinates(int x, int y)
        {
            GridCoordinates = new Vector2Int(x, y);
        }

    }
}

