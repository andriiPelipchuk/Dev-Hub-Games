using UnityEngine;

namespace Assets.Scripts.Gameplay.Bubble
{
    public class BubblePhysics : MonoBehaviour
    {
        [SerializeField] private LayerMask _bubbleLayerMask;
        [SerializeField] private float _snapDistance = 0.3f;

        private BubbleGrid _grid;
        private Rigidbody _rb;

        private void Awake()
        {
            _grid = FindAnyObjectByType<BubbleGrid>();
            _rb = GetComponent<Rigidbody>();
        }

        
        private void OnCollisionEnter(Collision collision)
        {
            var bubble = GetComponent<Bubble>();
            if(bubble.inGrid) return;

            if (((1 << collision.gameObject.layer) & _bubbleLayerMask.value) != 0)
            {
                var nearestPos = _grid.FindNearestGridPosition(transform.position);
                if (Vector2.Distance(transform.position, nearestPos.Item1) <= _snapDistance)
                {
                    _rb.linearVelocity = Vector3.zero;
                    _rb.isKinematic = true;
                    transform.position = nearestPos.Item1;
                    transform.SetParent(_grid.transform);

                    bubble.inGrid = true;
                    _grid.AddBubbleToGrid(bubble, nearestPos.Item1, nearestPos.Item2, nearestPos.Item3);
                }

            }
        }

    }
}