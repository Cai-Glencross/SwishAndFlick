using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private Slider healthBarSlider;
    // Start is called before the first frame update
    void Start()
    {
        healthBarSlider = this.gameObject.GetComponent<Slider>();
    }

    public void UpdateHealth(float percentage)
    {
        healthBarSlider.value = percentage;
    }
}
