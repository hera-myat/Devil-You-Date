using UnityEngine;

public class TriggerWalk : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogError("Animator not found!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided, triggering animation");
            anim.SetTrigger("Walk");
        }
    }
}