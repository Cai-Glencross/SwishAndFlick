using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private AudioClip successfulHitAudio;
    [SerializeField]
    private AudioClip failedHitAudio;

    private AudioSource audioSource;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(this.transform.position + this.transform.forward * speed * Time.deltaTime);
    }

    public void SuccessfulHit()
    {
        this.speed = 0;
        this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<ParticleSystem>().Stop();
        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponentInChildren<Collider>().enabled = false;

        audioSource.PlayOneShot(successfulHitAudio);
        StartCoroutine("DestroyAfterSound");
    }

    public void FailedHit()
    {
        this.speed = 0;
        this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<ParticleSystem>().Stop();
        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponentInChildren<Collider>().enabled = false;

        audioSource.PlayOneShot(failedHitAudio);
        StartCoroutine("DestroyAfterSound");
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        Destroy(this.gameObject);
    }

}
