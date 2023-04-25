using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Debug = UnityEngine.Debug;

namespace InteractiveMovement.Models
{
    public class Characters
    {
        public GameObject person { get; set; }
        public bool toForaward { get; set; }
        public float currentPosition { get; set; }
        public float lastPosition { get; set; }
        public float predictedPosition { get; set; }
        public DateTime lastUpdate { get; set; }
        public bool readyToUse { get;set; }
    }
    public class PersonController : MonoBehaviour
    {
        // Prefab for the person GameObject
        public GameObject personPrefab;

        // Dictionary to keep track of person GameObjects by ID
        [SerializeField] private Dictionary<string, GameObject> personObjects = new Dictionary<string, GameObject>();
        Dictionary<string, DateTime> oldTime = new Dictionary<string, DateTime>();

        // [SerializeField] private Dictionary<string, float> personOldPositions = new Dictionary<string, float>();
        // Interpolation variables
        //  private const float interpolationTime = 0.1f;
        //[SerializeField] private Dictionary<string, List<Vector3>> positionHistory = new Dictionary<string, List<Vector3>>();
        //private List<float> positions = new List<float>();
        [SerializeField] private float moveSpeed = 0.2f;

        
        List<Characters> charactersList = new List<Characters>();

        // Camera Bounds
        private float minX = -8.5f, maxX = 8.5f;

        //positions
        float testPosition;

        private const float interpolationTime = 0.1f;
        private Dictionary<string, List<Vector3>> positionHistory = new Dictionary<string, List<Vector3>>();

        //setting gameobject off after desired time
        float timer = 0f;
        float timerOffset = 2f;
        //checking position
        // int i = 0;
        //person id inside unity, which we will exchange with magsuds
        // string unityPersonID;
        public void Start()
        {
            //for (int i = 0; i < 30; i++)
            //{
            //    Characters character = new Characters();
            //    character.person = Instantiate(personPrefab, new Vector3(-9.5f, -0.12f, 0), Quaternion.identity);
            //    character.lastPosition = -9.5f;
            //    character.currentPosition = -9.5f;
            //    character.predictedPosition = -9.0f;
            //    character.readyToUse = true;
            //    character.toForaward = true;
            //    character.lastUpdate = DateTime.Now;
            //    charactersList.Add(character);
            //}
        }
        private void FixedUpdate()
        {
         
        }
        private void SetCharacterFree(Characters character)
        {
            character.person.SetActive(false);
            character.person.transform.position = new Vector3(-8.5f, -0.15f, 0);
            character.readyToUse = true;
            character.toForaward = true;
            character.person.SetActive(true);
        }
        float predictCorrectionValue = 0.25f;
       /* public void UpdatePositions(List<Message> messages)
        {
            Debug.Log(messages);
            if(messages.Count == 0 ) return;
            //var charac = charactersList.Where(w => w.readyToUse == false);

            foreach (Characters character in charactersList.Where(w => w.readyToUse == false))
            {
                List<Message> nearPos = new List<Message>();
                if (character.currentPosition < 0 && character.toForaward)
                {
                    nearPos = messages.Where(w => w.current_position > character.currentPosition)
                        .OrderBy(o => o.current_position).ToList();
                }
                else if (character.currentPosition > 0 && character.toForaward)
                {
                    nearPos = messages.Where(w => w.current_position < character.currentPosition)
                        .OrderBy(o => o.current_position).ToList();
                }
                if (character.currentPosition < 0 && !character.toForaward)
                {
                    nearPos = messages.Where(w => w.current_position < character.currentPosition)
                        .OrderBy(o => o.current_position).ToList();
                }
                else if (character.currentPosition > 0 && !character.toForaward)
                {
                    nearPos = messages.Where(w => w.current_position > character.currentPosition)
                        .OrderBy(o => o.current_position).ToList();
                }

                if (nearPos.Count() > 0)
                {
                    Message message = nearPos.FirstOrDefault();
                    float predict = character.predictedPosition;
                    Debug.Log("predicted position" + predict);
                    float predAvg = Math.Abs(character.currentPosition - message.current_position);
                    Debug.Log("predicted Average" + predAvg);
                    if (character.toForaward)
                    {
                        if (message.current_position < 0)
                        {
                            predAvg = message.current_position + predAvg;
                        }
                        else
                        {
                            predAvg = message.current_position - predAvg;
                        }
                    }
                    else
                    {
                        if (message.current_position < 0)
                        {
                            predAvg = message.current_position - predAvg;
                        }
                        else
                        {
                            predAvg = message.current_position + predAvg;
                        }

                    }
                    Debug.Log("Character current Position:" + character.currentPosition);
                    character.lastPosition = character.currentPosition;
                    character.predictedPosition = predAvg;
                    character.currentPosition = message.current_position;
                    character.lastUpdate = DateTime.Now;
                    messages.Remove(message);
                    StartCoroutine(InterpolatePositionCoroutine("",
                        new Vector3(character.currentPosition, -0.12f, 0),
                        character.lastPosition,
                        1,
                        "Moving"));

                }
            }
            if(messages.Count > 0)
            {
                foreach(Message m in messages)
                {
                    var character = charactersList.Where(w => w.readyToUse == true).FirstOrDefault();
                    //Debug.Log("Character current Position when readyToUse but current_position is less than -8:" + character.currentPosition);
                    character.lastPosition = m.current_position +  1;
                    character.currentPosition = m.current_position;
                    character.predictedPosition = m.current_position + 0.75f;
                    character.readyToUse = false;
                    character.toForaward = (m.current_position < 0);
                    character.person.transform.position = new Vector3(character.currentPosition, -0.12f, 0);
                    //StartCoroutine(InterpolatePositionCoroutine("",
                    //    new Vector3(character.currentPosition, -0.12f, 0),
                    //    character.lastPosition,
                    //    1,
                    //    "Moving"));
                }
            }
            //    foreach (Characters character in charactersList.Where(w => (DateTime.Now - w.lastUpdate).TotalMilliseconds > 500 
            //    && !w.readyToUse))
            //    {

            //        character.person.SetActive(false);
            //        character.person.transform.position = new Vector3(-8.5f, 0, 0);
            //        character.readyToUse = true;
            //        character.toForaward = true;
            //        character.person.SetActive(true);
            //}

            *//*

            Debug.Log("Direction :" + character.toForaward);
            if (character.currentPosition >= 9.5f || character.currentPosition <= -9.5f)
            {
                Debug.Log("character current position");
                SetCharacterFree(character);
            }

            if (character.currentPosition > 8.5f && character.toForaward == true)
            {
                Debug.Log("Character current Position when readytoUse is true:" + character.currentPosition);
                character.lastPosition = character.currentPosition;
                character.predictedPosition = 0;
                character.currentPosition = 8.5f;
                character.lastUpdate = DateTime.Now;

                StartCoroutine(InterpolatePositionCoroutine("",
                    new Vector3(8.5f, -0.12f, 0),
                    character.lastPosition,
                    1,
                    "Moving"));
            }
            if (character.currentPosition < -8.0f && character.toForaward == false)
            {
                Debug.Log("Character current Position when toForward is false:" + character.currentPosition);
                character.lastPosition = character.currentPosition;
                character.predictedPosition = 0;
                character.currentPosition = -8.5f;
                character.lastUpdate = DateTime.Now;

                StartCoroutine(InterpolatePositionCoroutine("",
                    new Vector3(-8.5f, -0.12f, 0),
                    character.lastPosition,
                    1,
                    "Moving"));
            }

            var m = messages.Where(w => //8.93 9.9
               // w.status != "Kill" && 
                (character.lastPosition > w.current_position && 
                w.current_position < character.predictedPosition + predictCorrectionValue) ||
                (character.lastPosition < w.current_position && //-8.9
                w.current_position > character.predictedPosition - predictCorrectionValue)

            ).OrderBy(o=> o.current_position);

            if(m.Count() > 0)
            {
                Message message = m.FirstOrDefault();
                float predict = character.predictedPosition;
                Debug.Log("predicted position" + predict);
                float predAvg = Math.Abs(character.currentPosition - message.current_position);
                Debug.Log("predicted Average" + predAvg);
                if (character.toForaward)
                {
                    if (message.current_position < 0)
                    {
                        predAvg = message.current_position + predAvg;
                    }
                    else
                    {
                        predAvg = message.current_position - predAvg;
                    }
                }
                else
                {
                    if (message.current_position < 0)
                    {
                        predAvg = message.current_position - predAvg;
                    }
                    else
                    {
                        predAvg = message.current_position + predAvg;
                    }

                }
                Debug.Log("Character current Position:" + character.currentPosition);
                character.lastPosition = character.currentPosition;
                character.predictedPosition = predAvg;
                character.currentPosition = message.current_position;
                character.lastUpdate = DateTime.Now;
                messages.Remove(m.FirstOrDefault());
                StartCoroutine(InterpolatePositionCoroutine("",
                    new Vector3(character.currentPosition, -0.12f, 0),
                    character.lastPosition, 
                    1, 
                    "Moving"));

            }
            else
            { 
                character.person.SetActive(false);
                character.person.transform.position = new Vector3(-8.5f, 0, 0);
                character.readyToUse = true;
                character.toForaward = true;
                character.person.SetActive(true);



            }
            }
    var newPositions = messages.Where(w =>
               //w.status != "Kill" &&
               (w.current_position < -8.0f ||
               w.current_position > 8.0f)
           ).OrderBy(o => o.current_position);

    foreach (Message message in newPositions)
    {
        if (message.current_position < -8.5 && message.current_position > -9)
        {

            var character = charactersList.Where(w => w.readyToUse == true).FirstOrDefault();
            Debug.Log("Character current Position when readyToUse but current_position is less than -8:" + character.currentPosition);
            character.lastPosition = message.current_position + 1;
            character.currentPosition = message.current_position;
            character.predictedPosition = message.current_position + 0.75f;
            character.readyToUse = false;
            character.toForaward = true;
            StartCoroutine(InterpolatePositionCoroutine("",
                new Vector3(character.currentPosition, -0.12f, 0),
                character.lastPosition,
                1,
                "Moving"));
        }
        else if (message.current_position > 8.5)
        {
            var character = charactersList.Where(w => w.readyToUse == true).FirstOrDefault();
            Debug.Log("Character current Position when readyToUse but current_position is greater than 8:" + character.currentPosition);
            character.lastPosition = message.current_position - 1;
            character.currentPosition = message.current_position;
            character.predictedPosition = message.current_position - 0.75f;
            character.readyToUse = false;
            character.toForaward = false;

            StartCoroutine(InterpolatePositionCoroutine("",
                new Vector3(character.currentPosition, -0.12f, 0),
                character.lastPosition,
                1,
                "Moving"));
        }
    }
            *//*


        }*/
        // This method updates the data for a specific person
        public void UpdatePersonData(string personId, string direction, int personMovingLine, float currentPosition, string status)
        {
           
           // unityPersonID = personId;
            // If the person ID is null, do nothing
            if (personId == null)
            {
                return;
            }

            // If the person has stopped, remove their GameObject if it exists and do nothing else
            /*if (status.Equals("Stopped", System.StringComparison.InvariantCultureIgnoreCase))
            {
                if (personObjects.ContainsKey(personId))
                {
                    GameObject.Destroy(personObjects[personId]);
                    personObjects.Remove(personId);
                }

                return;
            }*/

            // If the person does not have a GameObject, instantiate one
            //var stoppedPersons = personObjects.Where(w => w.Value.IsDestroyed() == false &&
            //    (w.Value.transform.position.x - currentPosition > -0.09 &&
            //            w.Value.transform.position.x - currentPosition < 0.09)
            //            ).ToList();
            //foreach(var stoppedCharacter in stoppedPersons)

            //{
            //   // stoppedCharacter.Value.SetActive(false);
            //}
            //if(stoppedPersons.Count > 0) 
            //{
            //    return;
            //}
            float oldPosition = 0f;
            if (!personObjects.ContainsKey(personId))
            {
                var personsNear = personObjects.Where(w => w.Value.IsDestroyed() == false &&
                (w.Value.transform.position.x - currentPosition > -0.7 &&
                        w.Value.transform.position.x - currentPosition < 0.7
                        )).OrderBy(w=>w.Value.transform.position.x - currentPosition);
                if (personsNear.Count() > 1)
                {
                    Debug.Log("Clonned: " + personId);
                    personObjects[personId] = personsNear.FirstOrDefault().Value.CloneViaFakeSerialization();
                    personObjects[personId].name = personId;
                    DestroyObject(personsNear.FirstOrDefault().Key, personsNear.FirstOrDefault().Value);

                    if(personsNear.Count() > 1)
                    {
                        foreach(var person in personsNear)
                        {
                            if(person.Key != personId)
                            {
                                DestroyObject(person.Key, person.Value);
                            }
                        }
                    }

                }
                else
                {
                     
                    GameObject newPersonObject = Instantiate(personPrefab, new Vector3(currentPosition, -0.15f, 0), Quaternion.identity);
                    personObjects[personId] = newPersonObject;
                    personObjects[personId].name = personId;

                }



               
                /*if (currentPosition < minX || currentPosition > maxX)
                {
                   
                    unityPersonID = personId;
                    if(i == 0 && i < 3 )
                    {
                        
                        positions.Add(currentPosition);
                        i++;
                    }
                    else if(i >= 2)
                    {
                        i = 0;
                    }
                }
                else
                {
                   if(currentPosition < positions[2] &&  currentPosition > positions[0])
                    { 

                    }
                }*/
            }
            // Get the GameObject for the person
            if (!personObjects.ContainsKey(personId))
                return;

            GameObject personObject = personObjects[personId];
            

            //oldTime[personId] = DateTime.Now; 

            if (oldTime.ContainsKey(personId))
                oldTime[personId] = DateTime.Now;
            else
                oldTime.Add(personId, DateTime.Now);
            //try
            //{
            //    var oldTimes = oldTime.Where(w => (DateTime.Now - w.Value).TotalMilliseconds > 150).ToList();
            //    foreach (var time in oldTimes)
            //    {
            //        if (personObjects.ContainsKey(time.Key))
            //        {
            //            DestroyObject(time.Key, personObjects[time.Key].gameObject);
            //        }
            //        oldTime.Remove(time.Key);
            //    }

            //}
            //catch
            //{ }
            if (personObject == null)
            {
                return;
            }
            if(timeHistory.ContainsKey(personId))
            {
                timeHistory[personId] = DateTime.Now;
            }

            oldPosition = personObject.transform.position.x;

            Debug.Log("Person position"+personObject.transform.position);
           // testPosition = personObject.transform.position.x; 
            //var currentXPosition = personObject.transform.position.x;
            Debug.Log("Old positions " + oldPosition + "Current Positions" + personObject.transform);
            // If the person is outside the camera bounds, destroy it and do nothing else
            if (personObject.transform.position.x < minX || personObject.transform.position.x > maxX)
            {
                DestroyObject(personId, personObject);
                return;
            }
            //if (personObject.IsDestroyed() == false &&
            //    (oldPosition - currentPosition > -0.05 &&
            //            oldPosition - currentPosition < 0.05
            //            )
            //            )
            //{
            //    DestroyObject(personId, personObject);
            //    // personObject.GetComponent<AnimationSpeedController>().ChangeAnimationSpeed(0);
            //    return;
            //}

            //oldPosition.x > currentPosition
            /* if(direction == "Forward")
             {
                 personObject.transform.GetComponent<SpriteRenderer>().flipX = false;
                 personObject.transform.GetComponent<SpriteRenderer>().color = UnityEngine.Color.green;
             }
             else if(direction == "Backward")
             { }*/

            //-6  -5.2

            // 17 - (currentPosition < 
            //

            //Debug.Log((currentPosition < 0 ? currentPosition * (-1) : currentPosition + 8.5) + " - Old - " +
            //(oldPosition < 0 ? oldPosition * (-1) : oldPosition + 8.5));


            float currentPositionN = (float)(currentPosition < 0 ? 8.5 - (currentPosition * (-1)) : currentPosition + 8.5);
            float oldPositionN = (float)(oldPosition < 0 ? 8.5 - (oldPosition * (-1)) : oldPosition + 8.5);

            if (currentPositionN < oldPositionN && //-5.10 < -4.8 
                oldPositionN - currentPositionN > 0.15f)  
            {
                //Debug.Log(currentPosition + " - " + personObject.transform.position.x);
                // going to the right
                personObject.transform.GetComponent<SpriteRenderer>().flipX = false;
                //personObject.transform.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
                Debug.Log("Going right");
            }
            else if (currentPositionN > oldPositionN && //-5.10 < -4.8 
                currentPositionN - oldPositionN > 0.15f)
            {
                // going to the left
                personObject.transform.GetComponent<SpriteRenderer>().flipX = true;
                //personObject.transform.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
                Debug.Log("Going left");
            }
            else
            {
                // standing still, but note that because float variables have extreme precision, you will most
                // likely never hit this branch
                Debug.Log("Standing still");
                timer += Time.deltaTime;
                if (timer > timerOffset)
                {
                    timer = 0f;
                    personObject.SetActive(false);
                }
            }

            //testPosition = currentXPosition;
        /*
        Vector3 testData = new Vector3(-8.5f,0,0);
            Vector3 testDataa = new Vector3(8.5f,0,0);
                if (testDataa.x > personObject.transform.position.x &&
                testDataa.x - personObject.transform.position.x > 0.5)
            {
                Debug.Log("flipping false" + oldPosition.x);

            }
            else if (testData.x < personObject.transform.position.x && testData.x - personObject.transform.position.x < 0.5)
            {
                Debug.Log("flipping true" + oldPosition.x);
                personObject.transform.GetComponent<SpriteRenderer>().flipX = true;
                personObject.transform.GetComponent<SpriteRenderer>().color = UnityEngine.Color.red;
            }*/


            //if (Mathf.Round((oldPosition.x) * 10) > (Mathf.Round(currentPosition * 10)))
            //{Debug.Log("flipping false" + oldPosition.x);

            //    personObject.transform.GetComponent<SpriteRenderer>().flipX = false;
            //    personObject.transform.GetComponent<SpriteRenderer>().color = UnityEngine.Color.green;
            //}
            //else if (Mathf.Round((oldPosition.x) * 10) < (Mathf.Round(currentPosition * 10)))
            //{
            //    Debug.Log("flipping true" + oldPosition.x);
            //    personObject.transform.GetComponent<SpriteRenderer>().flipX = true;
            //    personObject.transform.GetComponent<SpriteRenderer>().color = UnityEngine.Color.red;
            //}
            SpriteRenderer spriteRenderer = personObject.GetComponent<SpriteRenderer>();

            // Smoothly interpolates to next position
            if (personObject != null)
            {
                /* Vector3 lastPos;
                 if (positionHistory[personId].Count > 0)
                 {
                     lastPos = positionHistory[personId][positionHistory[personId].Count - 1];
                 }
                 else
                 {
                     lastPos = new Vector3(currentPosition, 0f, 0);
                 }*/
                
                StartCoroutine(InterpolatePositionCoroutine(personId, new Vector3(currentPosition, -0.15f, 0), oldPosition, personMovingLine, status));


            //    foreach (var g in personObjects)
            //    {
            //        if (g.Value.transform.position.x == personObject.transform.position.x 
            //            && personId != g.Key)
            //        {
            //            DestroyObject(personId, g.Value);
            //        }
            //}

            }
            // Update the status
            if (status == "Moving")
            {
                personObject.SetActive(true);
            }
            else if(status == "Stopped")
            {

                //personObject.SetActive(false);

            }
            else if(status == "Kill")
            {
                DestroyObject(personId, personObject);
               // StartCoroutine(DestroyIfStationary(personObject));
            }
        }

        private void DestroyObject(string personId, GameObject personObject)
        {
            if (personObjects.ContainsKey(personId))
            {
                Destroy(personObject);
                personObjects.Remove(personId);
                                
            }
           /* if (testPosition - personObject.transform.position.x > -0.1f &&
               testPosition - personObject.transform.position.x < 0.1f)
            {
                //Set lehs stay 

                return;
            }*/
        }

        // --------------------------- NEW INTERPOLATION METHOD --------------------------------------------------------

        // This method interpolates between the last and current positions to smooth out movement
        Dictionary<string, DateTime> timeHistory = new Dictionary<string, DateTime>();
        private IEnumerator InterpolatePositionCoroutine(string personId, Vector3 currentPosition, float oldPosition, int personMovingLine, string status)
        {
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

            //if (!positionHistory.ContainsKey(personId))
            //{
            //    positionHistory[personId] = new List<Vector3>();
            //}
            //positionHistory[personId].Add(currentPosition);
            //if (positionHistory[personId].Count > 10)
            //{
            //    positionHistory[personId].RemoveAt(0);
            //}
            // Gets instance of the person

            GameObject personObject = personObjects[personId];
            // Gets current color
            UnityEngine.Color currentColor = personObject.GetComponent<SpriteRenderer>().color;

            // Cache the current position of the person
            Vector3 startPos = personObject.transform.position;

            // Figures out the distance between the current position and where they are going
            float factor = Mathf.Abs(currentPosition.x - oldPosition);

            // Changes speed of animator as a function of the speed the person is moving ing
            //personObject.GetComponent<AnimationSpeedController>().ChangeAnimationSpeed(0.75f);//
                //Mathf.Clamp((factor == 0?1:factor) * moveSpeed 
                //, 0.5f, 1.75f));

            // Interpolates as a function of the distance that needs to be covered
             float progress = 0;

            float timeDifff = 250;
            if (oldTime.ContainsKey(personId))
            {
                timeDifff = (float)(DateTime.Now - oldTime[personId]).TotalMilliseconds;
            }
            float k = factor / 1000;
            while (progress <= factor)
            {
                if (factor == 0)
                {
                    yield break;
                }

                personObject.transform.position = Vector3.Lerp(new Vector3(oldPosition, -0.15f, 0), currentPosition, progress / factor);
                progress += Time.deltaTime + k;

            }

            yield return null;
            //my comment
            /*float speed = factor / 1000;
           if (speed == 0)
               yield return null;


          while (progress <= factor)
           {
               personObject.transform.position = new Vector3(personObject.transform.position.x + speed, 0.12f, 0);
               progress += speed;
           }*/

            //magsud comment
            //while (progress <= factor)
            //{
            //    if (factor == 0)
            //    {
            //        yield break;
            //    }


            //    progress += Time.deltaTime;

            //    yield return null;
            //}

            // Makes sure the object goes to the final position
            // personObject.transform.position = Vector3.Lerp(new Vector3(testPosition, 0, 0), currentPosition, Time.deltaTime/factor);
            //
            //personObject.transform.position = currentPosition;
            yield return null;
        }

        private IEnumerator DestroyIfStationary(GameObject personObject)
        {
            yield return new WaitForSeconds(0f);

        }
    }
}