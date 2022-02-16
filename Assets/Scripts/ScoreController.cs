using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{

    Text ScoreText;
    string ScoreTextPrefix = "Score: ";
    float score;
    // Start is called before the first frame update
    void Start()
    {
        ScoreText = this.gameObject.GetComponent<Text>();
        score = 0;
    }


    public void UpdateScore(float amount)
    {
        Debug.Log("score is " + score + " amount is " + amount);
        score += amount;
        Debug.Log("score is " + score + " amount is " + amount);
        ScoreText.text = ScoreTextPrefix + score;
        Debug.Log("score is " + score + " amount is " + amount);
    }
}
