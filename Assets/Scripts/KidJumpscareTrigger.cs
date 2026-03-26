using UnityEngine;

public class KidJumpscareTrigger : MonoBehaviour
{
    public GameObject kidPrefab; 
    private Animator kidAnimator;
    private ParticleSystem kidParticles;

    void Start()
    {
        kidPrefab.SetActive(false);

        kidAnimator = kidPrefab.GetComponent<Animator>();

        kidParticles = kidPrefab.GetComponentInChildren<ParticleSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            kidPrefab.SetActive(true);

            kidAnimator.Play("KidJumpscare");

            if (kidParticles != null)
            {
                kidParticles.Play();
            }
        }
    }
}