using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;

public class DrawManager : MonoBehaviour
{
    [SerializeField]
    public Image drawImage;

    [SerializeField]
    public GameObject canvas;

    [SerializeField]
    public int lineLength = 200;

    private ArrayList positionLog;
    private UILineRenderer lineRenderer;

    private Vector2[] ovalTemplate;

    private Vector2[] lineTemplate;

    private ArrayList templates;
    private PlayerController player;

    [SerializeField]
    private float minumumDistance;

    [SerializeField]
    private float pointPercentageThreshold;

    [SerializeField]
    private float checklistThreshold;

    [SerializeField]
    private GameObject[] templateGameObjects;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        positionLog = new ArrayList();
        lineRenderer = GetComponent<UILineRenderer>();

        ovalTemplate = GameObject.Find("OvalTemplate").GetComponent<UILineRenderer>().Points;
        lineTemplate = GameObject.Find("LineTemplate").GetComponent<UILineRenderer>().Points;


        templates = new ArrayList();
        foreach (GameObject template in templateGameObjects) 
        {
            templates.Add(template.GetComponent<UILineRenderer>().Points);
        }

        lineRenderer.Points = new Vector2[lineLength];

        foreach (GameObject template in templateGameObjects)
        {
            template.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            draw();
        }
        else if (positionLog.Count != 0)
        {
            //Debug.Log("positionLog count is: " + positionLog.Count);
            int spell = determineSpell();
            if (spell != -1)
            {
                player.castSpell(spell);
            }
            else
            {
                StartCoroutine("FlashTemplates");
            }
            clear();
        }
    }

    public void draw()
    {
        //Instantiate(drawImage, Input.mousePosition, Quaternion.identity, canvas.transform);
        positionLog.Add(new Vector2(Input.mousePosition.x - 0.5f * Screen.width, Input.mousePosition.y - 0.5f * Screen.height));
        int positionLogIndex = positionLog.Count - 1;
        int linePointsIndex = lineLength - 1;
        while (positionLogIndex >= 0 && linePointsIndex >= 0)
        {
            lineRenderer.Points.SetValue((Vector2)positionLog[positionLogIndex], linePointsIndex);
            linePointsIndex--;
            positionLogIndex--;
        }
        while(linePointsIndex >= 0 && positionLog.Count > 0)
        {
            lineRenderer.Points.SetValue((Vector2)positionLog[0], linePointsIndex);
            linePointsIndex--;
        }

        lineRenderer.SetAllDirty();
    }

    public void clear()
    {
        foreach (Transform child in canvas.transform)
        {
            if (!child.gameObject.tag.Equals("Permanent"))
            {
                Destroy(child.gameObject);
            }
        }
        lineRenderer.Points = new Vector2[lineLength];
        positionLog.Clear();
    }

    public int determineSpell()
    {
        ArrayList templatePointChecklist = new ArrayList();
        foreach (Vector2[] checklistPoints in templates)
        {
            templatePointChecklist.Add(new bool[checklistPoints.Length]);
        }

        float[] points = new float[templates.Count];
        foreach(Vector2 position in positionLog)
        {
            float[] scoreOfTemplates = getScore(position, templatePointChecklist);
            //we consider a point in the postionLog being closer to one Template than the other as a "point"
            if (scoreOfTemplates.Min() <= minumumDistance)
            {
                points[Array.IndexOf(scoreOfTemplates, scoreOfTemplates.Min())] += 1;
            }
        }

        //Debug.Log("points are: " + string.Join(", ", points));
        //The template with the most points, wins!
        Debug.Log("Point percentage is " + (points.Max() / positionLog.Count));
        if ((points.Max()/positionLog.Count) >= pointPercentageThreshold)
        {
            //ensure that enough of the template has been covered
            int winningTemplateIndex = Array.IndexOf(points, points.Max());
            float checklistPercentage = determineChecklistPercentage(winningTemplateIndex, templatePointChecklist);
            Debug.Log("checklist percentage is" + checklistPercentage);
            if (checklistPercentage > checklistThreshold)
            {
                return winningTemplateIndex;
            }
            else
            {
                return -1;
            }

        }
        else
        {
            return -1;
        }
    }

    public float[] getScore(Vector2 position, ArrayList checklistPoints)
    {
        float[] scores = new float[templates.Count];
        for(int i = 0; i < templates.Count; i++)
        {
            float[] distances = ((Vector2[])templates[i])
                .Select(templatePosition => Vector2.Distance(position, templatePosition)).ToArray();
            //the closest distance for each template is the score of that template
            //Debug.Log("distances are: " + string.Join(", ", distances));
            //assuming no distances are exactly the same
            ((bool[])checklistPoints[i])[Array.IndexOf(distances, distances.Min())] = true;

            scores[i] = distances.Min();
        }
        //Debug.Log("scores are: " + string.Join(", ", scores));
        return scores;
    }

    private IEnumerator FlashTemplates()
    {
        foreach (GameObject template in templateGameObjects)
        {
            template.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        foreach (GameObject template in templateGameObjects)
        {
            template.SetActive(false);
        }
    }

    private float determineChecklistPercentage(int index, ArrayList templatePointChecklist)
    {
        bool[] checklist = (bool[])templatePointChecklist[index];

        //get percentage
        int numChecks = 0;
        foreach (bool check in checklist)
        {
            if (check)
            {
                numChecks++;
            }
        }
        return (float)numChecks / checklist.Length;
    }
}
