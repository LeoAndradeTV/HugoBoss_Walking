using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InteractiveMovement.Models
{
    public class PersonController : MonoBehaviour
    {
        // Prefab for the person GameObject
        public GameObject personPrefab;

        // Dictionary to keep track of person GameObjects by ID
        private Dictionary<string, GameObject> personObjects = new Dictionary<string, GameObject>();

        // Interpolation variables
        private const float interpolationTime = 0.1f;
        private Dictionary<string, List<Vector3>> positionHistory = new Dictionary<string, List<Vector3>>();
        private List<float> positions = new List<float>();
        private float moveSpeed = 15f;

        // Camera Bounds
        private float minX = -7f, maxX = 7f;

        // This method updates the data for a specific person
        public void UpdatePersonData(string personId, string direction, int personMovingLine, float currentPosition, string status)
        {
            // If the person ID is null, do nothing
            if (personId == null)
            {
                return;
            }

            // If the person has stopped, remove their GameObject if it exists and do nothing else
            if (status.Equals("Stopped", System.StringComparison.InvariantCultureIgnoreCase))
            {
                if (personObjects.ContainsKey(personId))
                {
                    GameObject.Destroy(personObjects[personId]);
                    personObjects.Remove(personId);
                }

                return;
            }

            // If the person does not have a GameObject, instantiate one
            if (!personObjects.ContainsKey(personId))
            {
                GameObject newPersonObject = Instantiate(personPrefab, new Vector3(currentPosition, 1.7f, 0), Quaternion.identity);
                personObjects[personId] = newPersonObject;
            }

            // Get the GameObject for the person
            GameObject personObject = personObjects[personId];


            // If the person is outside the camera bounds, destroy it and do nothing else
            if (personObject.transform.position.x < minX || personObject.transform.position.x > maxX)
            {
                DestroyObject(personId, personObject);
                return;
            }

            // Set the sprite flip based on the direction
            bool flipX = (direction.Equals("Backward", System.StringComparison.InvariantCultureIgnoreCase));
            SpriteRenderer spriteRenderer = personObject.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = flipX;


            // Set the person moving line opacity
            UnityEngine.Color color = spriteRenderer.color;
            spriteRenderer.color = SetColor(personMovingLine, color);

            // Update the position history for interpolation
            if (!positionHistory.ContainsKey(personId))
            {
                positionHistory[personId] = new List<Vector3>();
            }
            positionHistory[personId].Add(new Vector3(currentPosition, 1.7f, 0));
            if (positionHistory[personId].Count > 10)
            {
                positionHistory[personId].RemoveAt(0);
            }

            // -------------------------------------- OLD CALL TO INTERPOLATION -----------------------------

            // Interpolate to the next position
            //Vector3 targetPosition = InterpolatePosition(personId, new Vector3(currentPosition, 0, 0));
            //personObject.transform.position = targetPosition;

            // ----------------------------------------------------------------------------------------------


            // Smoothly interpolates to next position
            StartCoroutine(InterpolatePositionCoroutine(personId, new Vector3(currentPosition,1.7f,0), personMovingLine, status));

            // Update the status
            if (status.Equals("Moving", System.StringComparison.InvariantCultureIgnoreCase))
            {
                personObject.SetActive(true);
                //if (spriteRenderer.color == UnityEngine.Color.clear) 
                //{
                    StartCoroutine(Fade(personObject, spriteRenderer.color, SetColor(personMovingLine, spriteRenderer.color)));
               // }
            }
            else
            {
                StartCoroutine(Fade(personObject, spriteRenderer.color, UnityEngine.Color.clear));
                personObject.SetActive(false);
            }
        }

        private void DestroyObject(string personId, GameObject personObject)
        {
            if (personObjects.ContainsKey(personId))
            {
                Destroy(personObject);
                personObjects.Remove(personId);
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
                    color.a = 0.6f;
                    break;
                case 3:
                    color.a = 0.3f;
                    break;
                case 4:
                    color.a = 0.1f;
                    break;
                default:
                    color.a = 1f;
                    break;
            }
            return color;
        }

        // -------------------------- LEFT THIS HERE AS YOUR OLD METHOD TO REFERENCE ------------------------------------

        // This method interpolates between the last and current positions to smooth out movement
        //private Vector3 InterpolatePosition(string personId, Vector3 currentPosition)
        //{
        //    Vector3 targetPosition = currentPosition;

        //    // If there is position history for the person and there are at least two positions in the history
        //    if (positionHistory.ContainsKey(personId) && positionHistory[personId].Count >= 2)
        //    {
        //        Vector3 lastPosition = positionHistory[personId][positionHistory[personId].Count - 2];
        //        float interpolationProgress = Mathf.Clamp01((Time.time - interpolationTime) / interpolationTime);
        //        targetPosition = Vector3.Lerp(lastPosition, currentPosition, interpolationProgress);
        //    }
        //    return targetPosition;
        //}


        // --------------------------- NEW INTERPOLATION METHOD --------------------------------------------------------

        // This method interpolates between the last and current positions to smooth out movement

        private IEnumerator InterpolatePositionCoroutine(string personId, Vector3 currentPosition, int personMovingLine, string status)
        {
            // Gets instance of the person
            GameObject personObject = personObjects[personId];

            // Gets current color
            UnityEngine.Color currentColor = personObject.GetComponent<SpriteRenderer>().color;

            // Cache the current position of the person
            Vector3 startPos = personObjects[personId].transform.position;

            // Figures out the distance between the current position and where they are going
            float factor = Mathf.Abs(currentPosition.x - startPos.x);

            // Changes speed of animator as a function of the speed the person is moving ing
            personObject.GetComponent<AnimationSpeedController>().ChangeAnimationSpeed(Mathf.Clamp(factor * moveSpeed, 0f, 1.5f));

            // Interpolates as a function of the distance that needs to be covered
            float progress = 0;

            while (progress <= factor)
            {
                personObject.transform.position = Vector3.Lerp(startPos, currentPosition, progress/factor);
                progress += Time.deltaTime;
                yield return null;
            }
            
            // Makes sure the object goes to the final position
            personObject.transform.position = currentPosition;
        }

        /// <summary>
        /// Fades color in and out
        /// </summary>
        /// <param name="personObject"></param>
        /// <param name="startingColor"></param>
        /// <param name="endColor"></param>
        /// <returns></returns>
        private IEnumerator Fade(GameObject personObject, UnityEngine.Color startingColor, UnityEngine.Color endColor)
        {
            float progress = 0;
            while (progress <= 1) 
            {
                personObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.Lerp(startingColor, endColor, progress);
                progress += Time.deltaTime;
                yield return null;
            }
            personObject.GetComponent<SpriteRenderer>().color = endColor;

        }
    }
}
