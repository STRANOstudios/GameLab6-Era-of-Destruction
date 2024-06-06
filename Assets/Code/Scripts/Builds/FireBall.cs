using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == 6)
        {
            Destroy(gameObject);
        }
    }
}
