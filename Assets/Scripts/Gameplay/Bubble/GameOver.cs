using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Bubble
{
    public class GameOver : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var bubble = other.gameObject.GetComponent<Bubble>();
            if ( bubble.inGrid)
            {
                GameEvents.TriggerGameOver();
            }
        }
    }
}