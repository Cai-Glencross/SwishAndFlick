using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float rotationSpeed;

    [SerializeField]
    public GameObject[] spellProjectiles;

    private Vector3 rightTurn = new Vector3(0, 1, 0);

    private Vector3 leftTurn = new Vector3(0, -1, 0);

    private bool isRotatingLeft;

    private bool isRotatingRight;

    private float currentRotation;

    private Animator myAnimController;

    private float health;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private HealthBarController healthBarController;



    // Start is called before the first frame update
    void Start()
    {
        currentRotation = 0;
        isRotatingLeft = false;
        isRotatingRight = false;
        myAnimController = this.GetComponent<Animator>();

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        myAnimController.SetBool("IsAttacking", false);
        bool isRotating = (isRotatingLeft || isRotatingRight);
        if (Input.GetKeyDown(KeyCode.A) && !isRotating)
        {
            isRotatingLeft = true;
            currentRotation = 0;
        }
        else if (Input.GetKeyDown(KeyCode.D) && !isRotating)
        {
            isRotatingRight = true;
            currentRotation = 0;
        }

        if (isRotatingLeft)
        {
            isRotatingLeft = !rotate(leftTurn);
            
        }
        else if(isRotatingRight)
        {
            isRotatingRight = !rotate(rightTurn);
        }
        myAnimController.SetBool("IsRotatingLeft", isRotatingLeft);
        myAnimController.SetBool("IsRotatingRight", isRotatingRight);
    }

    private bool rotate(Vector3 turn)
    {
        bool isDone = false;
        if (rotationSpeed > 90.0 - currentRotation)
        {
            transform.Rotate(turn * (90 - currentRotation));
            isDone = true;
        }
        else
        {
            currentRotation += rotationSpeed;
            transform.Rotate(turn * rotationSpeed, Space.Self);
        }
        //Debug.Log(currentRotation);

        return isDone;
    }

    public void castSpell(int spellIndex)
    {
        Instantiate(spellProjectiles[spellIndex],
            this.transform.position + this.transform.forward * 2 + this.transform.up,
            this.transform.rotation);

        myAnimController.SetBool("IsAttacking", true);
    }

    public void damage(float amount)
    {
        Debug.Log("damaging!");
        health -= amount;
        healthBarController.UpdateHealth(health / maxHealth);
        if (health > 0)
        {
            myAnimController.SetBool("IsGettingHit", true);
            StartCoroutine("resetGetHit");
        }
        else
        {
            myAnimController.SetBool("IsDying", true);
        }


    }

    private IEnumerator resetGetHit()
    {
        yield return new WaitForSeconds(.1f);
        myAnimController.SetBool("IsGettingHit", false);


    }
}