using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Bubble
{
    public class MoveBubble : MonoBehaviour
    {

        [SerializeField] private float _moveSpeed = 5f;

        private void LateUpdate()
        {
            transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        }
    }
}