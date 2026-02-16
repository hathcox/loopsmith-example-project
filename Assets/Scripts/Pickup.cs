using UnityEngine;

namespace CubeCollector
{
    public class Pickup : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (GameManager.Instance == null)
            {
                Debug.LogError("[Pickup] GameManager.Instance is null — cannot collect pickup.");
                return;
            }

            GameManager.Instance.CollectPickup(this);
        }
    }
}
