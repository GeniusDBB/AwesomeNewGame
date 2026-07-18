using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustParticles;

    public void PlayDust()
    {
        dustParticles.Play();
    }
}