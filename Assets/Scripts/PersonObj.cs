/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PersonObj : MonoBehaviour, IDisposable
{
    public GameObject personObject;
    string personId;

    //public GameObject personPrefab;
     

    // Interpolation variables
    private const float interpolationTime = 0.1f;
    [SerializeField] private Dictionary<string, List<Vector3>> positionHistory = new Dictionary<string, List<Vector3>>();
    private List<float> positions = new List<float>();
    [SerializeField] private float moveSpeed = 0.2f;
    string direction;
    int personMovingLine;
    float currentPosition;
    string status;
    bool dataUpdated = false;

    Dictionary<string, DateTime> timeHistory = new Dictionary<string, DateTime>();

    // Camera Bounds
    private float minX = -8.5f, maxX = 8.5f;
    Thread thread;
    // Start is called before the first frame update
    void Start()
    {
        //itializeGame(personId);
    }

    public void InitializeGame(GameObject personObject)
    {
        try
        {
            this.personObject = personObject;
            if (this.personObject == null)
            {
                throw new Exception("Do not assigne person ID");
            }

            this.direction = "Forward";
            this.personMovingLine = 1;
            this.currentPosition = minX;
            this.status = "Stopped";
            dataUpdated = true;

            //this.personId = personId;
            //personObject = Instantiate(personPrefab, new Vector3(currentPosition, 0f, 0), Quaternion.identity);
            this.personObject.SetActive(true);

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        finally
        {
            thread = new Thread(UpdateObj);
            thread.Start();
        }
        

    }

    private void UpdateObj()
    { 
        while(true)
        {
            if (!dataUpdated)
            {
                Thread.Sleep(1000 / 60);
                continue;
            }

            UpdatePersonData();

            dataUpdated = false;
            //Thread.Sleep(Time.deltaTime)
        }
    }


    public void SetPersonData(string direction, int personMovingLine, float currentPosition, string status)
    {
        //this.personId = personId;
        this.direction = direction;
        this.personMovingLine = personMovingLine; 
        this.currentPosition = currentPosition;
        this.status = status;
        dataUpdated = true;
    }
        public void UpdatePersonData()
    {
        // If the person ID is null, do nothing
        //if (personId == null)
        //{
        //    throw new Exception("Do not assigne person ID");
        //}
         

        // Get the GameObject for the person
        if (personObject == null)
        {
            throw new Exception("Do not have game object");
        }

        //TODO : Use this in other method MA
        //// If the person is outside the camera bounds, destroy it and do nothing else
        //if (personObject.transform.position.x < minX || personObject.transform.position.x > maxX)
        //{
        //    DestroyObject(personId, personObject);
        //    return;
        //}

        // Set the sprite flip based on the direction
         
        SpriteRenderer spriteRenderer = personObject.GetComponent<SpriteRenderer>();
        //spriteRenderer.flipX = flipX;
        if (direction.Equals("Backward", System.StringComparison.InvariantCultureIgnoreCase))
        {

            spriteRenderer.flipX = true;
        }
        if (direction.Equals("Forward", System.StringComparison.InvariantCultureIgnoreCase))
        {
            spriteRenderer.flipX = false;
        }

        // Set the person moving line opacity
        UnityEngine.Color color = spriteRenderer.color;
        spriteRenderer.color = SetColor(personMovingLine, color);

         
        // Smoothly interpolates to next position
         
            StartCoroutine(InterpolatePositionCoroutine(personId, new Vector3(currentPosition, 0f, 0), personMovingLine, status)); 

        // Update the status
        if (status.Equals("Moving", System.StringComparison.InvariantCultureIgnoreCase))
        {
            //TODO : Change for faster value changes 
            personObject.SetActive(true);
        }
        else
        { 
            //personObject.SetActive(false);
        }
        
    }

    private UnityEngine.Color SetColor(int personMovingLine, UnityEngine.Color currentColor)
    {
        UnityEngine.Color color = currentColor;
        switch (personMovingLine)
        {
            case 1:
                color.a = 1f;
                break;
            case 2:
                color.a = 0.9f;
                break;
            case 3:
                color.a = 0.8f;
                break;
            case 4:
                color.a = 0.7f;
                break;
            default:
                color.a = 1f;
                break;
        }
        return color;
    }

    // This method interpolates between the last and current positions to smooth out movement

    private IEnumerator InterpolatePositionCoroutine(string personId, Vector3 currentPosition, int personMovingLine, string status)
    {

        //Clean this code 
        float transformSpeedCoeficcent = 0;
        var pH = positionHistory.ContainsKey(personId) ? positionHistory[personId] : null;
        if (pH != null)
        {
            Vector3 pP = pH[pH.Count - 1];
            float positionDifference = pP.x - currentPosition.x;

            if (positionDifference < 0)
            {
                positionDifference = positionDifference * (-1);


            }
            if (timeHistory.Count > 0)
            {
                DateTime? lastTime = timeHistory.ContainsKey(personId) ? timeHistory[personId] : null;
                if (lastTime != null)
                {
                    float timeDiff = (float)(DateTime.Now - (DateTime)lastTime).TotalMilliseconds;

                    //speed = distance/time 
                    transformSpeedCoeficcent = positionDifference / timeDiff;

                    timeHistory[personId] = DateTime.Now;
                }
                else
                    timeHistory.Add(personId, DateTime.Now);
            }
            else
                timeHistory.Add(personId, DateTime.Now);
        }

        if (!positionHistory.ContainsKey(personId))
        {
            positionHistory[personId] = new List<Vector3>();
        }
        positionHistory[personId].Add(currentPosition);
        if (positionHistory[personId].Count > 10)
        {
            positionHistory[personId].RemoveAt(0);
        }

        // Gets instance of the person
         
        // Gets current color
        UnityEngine.Color currentColor = personObject.GetComponent<SpriteRenderer>().color;

        // Cache the current position of the person
        Vector3 startPos = personObject.transform.position;

        // Figures out the distance between the current position and where they are going
        float factor = Mathf.Abs(currentPosition.x - startPos.x);

        // Changes speed of animator as a function of the speed the person is moving ing
        personObject.GetComponent<AnimationSpeedController>().ChangeAnimationSpeed(Mathf.Clamp(factor * moveSpeed * ((transformSpeedCoeficcent == 0) ? 1 : transformSpeedCoeficcent)
            , 0f, 1.5f));

        // Interpolates as a function of the distance that needs to be covered
        float progress = 0;

        while (progress <= factor)
        {
            if (factor == 0)
            {
                yield break;
            }

            personObject.transform.position = Vector3.Lerp(startPos, currentPosition, progress / factor * ((transformSpeedCoeficcent == 0) ? 1 : transformSpeedCoeficcent));
            progress += Time.deltaTime;

            yield return null;
        }

        // Makes sure the object goes to the final position
        personObject.transform.position = currentPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Dispose()
    {
        thread.Abort();
        Destroy(personObject);
    }
}
*/