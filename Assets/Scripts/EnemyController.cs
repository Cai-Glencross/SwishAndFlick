using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    public float enemySpeed;

    [SerializeField]
    private float sinkSpeed;

    [SerializeField]
    private string weakness;

    [SerializeField]
    private float fadeIncrement;

    [SerializeField]
    private GameObject enemyMeshObj;

    [SerializeField]
    private float scoreWorth;

    [SerializeField]
    private TextMeshPro textMeshPro;

    private ScoreController scoreController;

    private Material enemyMaterial;

    private Animator enemyAnim;

    private Vector3 playerPosition;
    private GameObject playerObject;

    [SerializeField]
    private float attackDamage;

    [SerializeField]
    private AudioClip attackingSound;

    [SerializeField]
    private AudioClip dyingSound;

    private AudioSource audioSource;
    

    private bool isFading;
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player").gameObject;
        playerPosition = playerObject.transform.position;
        enemyAnim = this.GetComponent<Animator>();

        isFading = false;

        textMeshPro.SetText("+" + scoreWorth);
        textMeshPro.gameObject.SetActive(false);

        enemyMaterial = enemyMeshObj.GetComponent<SkinnedMeshRenderer>().material;

        scoreController = FindObjectOfType<ScoreController>();

        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionToPlayer = (playerPosition - this.transform.position).normalized;
        directionToPlayer = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
        if (directionToPlayer.magnitude > 0)
        {
            enemyAnim.SetBool("IsFlying", true);
        }
        this.transform.position += directionToPlayer * (enemySpeed * Time.deltaTime);

        if (isFading)
        {
            if(enemyMaterial.shader.name == "PBRMaskTint")
            {
                Color tmpColor = enemyMaterial.GetColor("_Color01");
                enemyMaterial.SetColor("_Color01", new Color(tmpColor.r,
                                                tmpColor.g,
                                                tmpColor.b,
                                                Mathf.Max(tmpColor.a - fadeIncrement * Time.deltaTime, -0.1f)));
            }
            else
            {
                Color tmpColor = enemyMaterial.GetColor("_Color");
                enemyMaterial.SetColor("_Color", new Color(tmpColor.r,
                                                tmpColor.g,
                                                tmpColor.b,
                                                Mathf.Max(tmpColor.a - fadeIncrement * Time.deltaTime, -0.1f)));
            }

            textMeshPro.color = new Color(textMeshPro.color.r,
                textMeshPro.color.g,
                textMeshPro.color.b,
                Mathf.Max(textMeshPro.color.a - fadeIncrement * Time.deltaTime, -0.1f));

            this.transform.position += Vector3.down * sinkSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (weakness.Equals(collision.gameObject.tag))
        {

            //increment score
            if (!isFading)
            {
                scoreController.UpdateScore(scoreWorth);
            }
            isFading = true;
            enemyAnim.SetBool("IsDying", true);
            StartCoroutine(WaitAndDie(2.0f));
            enemySpeed = 0;
            //Destroy(collision.gameObject);
            collision.gameObject.GetComponent<SpellProjectile>().SuccessfulHit();
            textMeshPro.gameObject.SetActive(true);
        }
        else if (collision.gameObject.tag.Equals("Player"))
        {
            enemyAnim.SetBool("IsAttacking", true);
        }
        else if(!collision.gameObject.tag.Equals("Terrain"))
        {
            //Destroy(collision.gameObject);
            SpellProjectile spellProjectile = collision.gameObject.GetComponent<SpellProjectile>();
            if (spellProjectile)
            {
                spellProjectile.FailedHit();
            }
        }
    }

    private IEnumerator WaitAndDie(float waitTime)
    {
        audioSource.PlayOneShot(dyingSound);
        yield return new WaitForSeconds(waitTime);
        Destroy(this.gameObject);
       
    }

    public void DealDamage()
    {
        enemySpeed = 0;
        playerObject.GetComponent<PlayerController>().damage(attackDamage);
        audioSource.PlayOneShot(attackingSound);
    }




}
