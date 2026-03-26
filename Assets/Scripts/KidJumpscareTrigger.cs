using UnityEngine;

public class KidJumpscareTrigger : MonoBehaviour
{
    public GameObject kidPrefab;      // Assign your FBX model here
    private Animator kidAnimator;
    private ParticleSystem kidParticles;

    void Start()
    {
        // Disable the kid model at start
        kidPrefab.SetActive(false);

        // Get Animator
        kidAnimator = kidPrefab.GetComponent<Animator>();

        // Get ParticleSystem inside the kid prefab (if any)
        kidParticles = kidPrefab.GetComponentInChildren<ParticleSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Enable the kid model
            kidPrefab.SetActive(true);

            // Play the jumpscare animation
            kidAnimator.Play("KidJumpscare");  // exact clip name inside Animator Controller

            // Play particle system if found
            if (kidParticles != null)
            {
                kidParticles.Play();
            }
        }
    }
}