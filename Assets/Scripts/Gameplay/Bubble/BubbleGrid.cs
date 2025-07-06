using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Bubble
{
    public class BubbleGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 50;
        [SerializeField] private int _emptyHeight = 10;
        [SerializeField] private float _bubbleSpacing = 0.6f;
        [SerializeField] private Bubble _bubblePrefab;

        private Bubble[,] _grid;

        private void Start()
        {
            _grid = new Bubble[_width, _height];
            GenerateLevel();
        }
        public (Vector3, int, int) FindNearestGridPosition(Vector3 worldPos)
        {
            Vector3 nearestPos = Vector3.zero;
            float minDistance = 0.3f;
            bool foundPosition = false;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y] == null)
                    {
                        Vector3 gridPos = GetWorldPosition(x, y);
                        float distance = Vector3.Distance(worldPos, gridPos);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestPos = gridPos;
                            foundPosition = true;
                            return (nearestPos, x, y); 
                        }
                    }
                }
            }

            if (!foundPosition)
            {
                Debug.LogWarning("No empty grid positions available!");
            }
            return (Vector3.positiveInfinity, 0, 0);
        }

        public void AddBubbleToGrid(Bubble bubble, Vector3 worldPos, int x, int y)
        {
            if (worldPos != Vector3.positiveInfinity)
            {
                _grid[x, y] = bubble;
                bubble.GridCoordinates = new Vector2Int(x, y);
                CheckMatches(x, y, bubble.Color);
            }
        }
        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            Vector2 localPos = worldPos - (Vector3)transform.position;

            int gridY = Mathf.RoundToInt(-localPos.y / _bubbleSpacing);
            int gridX;

            if (gridY % 2 == 0)
                gridX = Mathf.RoundToInt(localPos.x / _bubbleSpacing);
            else
                gridX = Mathf.RoundToInt((localPos.x - _bubbleSpacing / 2f) / _bubbleSpacing);

            return new Vector2Int(gridX, gridY);
        }

        public void CheckMatches(int startX, int startY, Color targetColor)
        {
            List<Bubble> matchedBubbles = FindConnectedBubbles(startX, startY, targetColor);

            if (matchedBubbles.Count >= 3)
            {
                foreach (Bubble bubble in matchedBubbles)
                {
                    RemoveBubble(bubble);
                }
            }
        }


        private void GenerateLevel()
        {
            for (int y = 0; y < _height - _emptyHeight; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (Random.Range(0, 5) > 1)
                    {
                        SpawnBubble(x, y);
                    }
                }
            }
        }

        private void SpawnBubble(int x, int y)
        {
            Vector2 pos = GetWorldPosition(x, y);
            Bubble bubble = Instantiate(_bubblePrefab, pos, Quaternion.identity, transform);
            bubble.inGrid = true;
            bubble.SetColor();
            _grid[x, y] = bubble;
        }

        private Vector2 GetWorldPosition(int x, int y)
        {
            float offsetX = (y % 2 == 0) ? 0 : _bubbleSpacing / 2;
            return (Vector2)transform.position + new Vector2(x * _bubbleSpacing + offsetX,-y * _bubbleSpacing);
        }
        public List<Vector2Int> GetNeighbors(Vector2Int pos)
        {
            int x = pos.x;
            int y = pos.y;

            Vector2Int[] evenRowDirs = new Vector2Int[]
            {
        new Vector2Int(+1, 0), new Vector2Int(0, +1), new Vector2Int(-1, +1),
        new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1)
            };

            Vector2Int[] oddRowDirs = new Vector2Int[]
            {
        new Vector2Int(+1, 0), new Vector2Int(+1, +1), new Vector2Int(0, +1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(+1, -1)
            };

            Vector2Int[] directions = (y % 2 == 0) ? evenRowDirs : oddRowDirs;

            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = new Vector2Int(x + dir.x, y + dir.y);
                if (IsInsideGrid(neighborPos.x, neighborPos.y))
                {
                    neighbors.Add(neighborPos);
                }
            }

            return neighbors;
        }
        bool IsInsideGrid(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _width && y < _height;
        }
        public Bubble GetBubbleAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return null;

            return _grid[x, y]; 
        }
        public List<Bubble> FindConnectedBubbles(int startX, int startY, Color targetColor)
        {
            List<Bubble> connected = new List<Bubble>();
            Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            toCheck.Enqueue(new Vector2Int(startX, startY));
            visited.Add(new Vector2Int(startX, startY));

            while (toCheck.Count > 0)
            {
                Vector2Int current = toCheck.Dequeue();
                Bubble currentBubble = GetBubbleAt(current.x, current.y);

                if (currentBubble == null || currentBubble.Color != targetColor)
                    continue;

                connected.Add(currentBubble);

                foreach (Vector2Int dir in GetNeighbors(current))
                {
                    if (!visited.Contains(dir))
                    {
                        Bubble neighbor = GetBubbleAt(dir.x, dir.y);
                        if (neighbor != null && neighbor.Color == targetColor)
                        {
                            toCheck.Enqueue(dir);
                            visited.Add(dir);
                        }
                    }
                }
            }

            return connected;
        }

        private void FloodFill(int x, int y, Color targetColor, bool[,] visited, List<Bubble> result)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height) return;
            if (visited[x, y] || _grid[x, y] == null || _grid[x, y].Color != targetColor) return;

            visited[x, y] = true;
            result.Add(_grid[x, y]);

            FloodFill(x + 1, y, targetColor, visited, result);
            FloodFill(x - 1, y, targetColor, visited, result);
            FloodFill(x, y + 1, targetColor, visited, result);
            FloodFill(x, y - 1, targetColor, visited, result);
            FloodFill(x + (y % 2 == 0 ? 1 : -1), y + 1, targetColor, visited, result);
            FloodFill(x + (y % 2 == 0 ? 1 : -1), y - 1, targetColor, visited, result);
        }

        private void RemoveBubble(Bubble bubble)
        {
            Vector2Int coords = bubble.GridCoordinates;
            _grid[coords.x, coords.y] = null;

            var collider = bubble.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            var rb = bubble.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            GameEvents.TriggerBubblePopped();
            StartCoroutine(DeleteBubble(bubble));
        }

        private IEnumerator DeleteBubble(Bubble bubble)
        {
            yield return new WaitForSeconds(3f);
            Destroy(bubble.gameObject);
        }
    }

}