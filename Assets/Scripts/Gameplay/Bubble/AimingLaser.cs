using UnityEngine;

namespace Assets.Scripts.Gameplay.Bubble
{
    public class AimingLaser : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private int _maxBounces = 5;
        [SerializeField] private float _maxLength = 20f;
        [SerializeField] private LayerMask _reflectionMask;

        public void DrawTrajectory(Vector3 startPos, Vector3 direction)
        {
            Vector3 currentPosition = startPos;
            Vector3 currentDirection = direction;

            _lineRenderer.positionCount = 1;
            _lineRenderer.SetPosition(0, currentPosition);

            for (int i = 0; i < _maxBounces; i++)
            {
                Ray ray = new Ray(currentPosition, currentDirection);

                if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, _maxLength, _reflectionMask))
                {
                    Vector3 hitPoint = hit.point;
                    _lineRenderer.positionCount += 1;
                    _lineRenderer.SetPosition(i + 1, hitPoint);


                    currentPosition = hitPoint;
                    currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                }
                else
                {
                    _lineRenderer.positionCount += 1;
                    _lineRenderer.SetPosition(i + 1, currentPosition + currentDirection * _maxLength);
                    break;
                }
            }
        }
    }
}