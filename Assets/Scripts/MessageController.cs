using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InteractiveMovement.Models
{
    public class MessageController : MonoBehaviour
    {
        // Fields
        private SignalRConnector connector; // SignalRConnector object used to connect to the server
        PersonController personController; // PersonController object used to update person data

        // Methods
        private async void Start()
        {
            // Get reference to PersonController object
            personController = GameObject.FindObjectOfType<PersonController>();

            // Instantiate SignalRConnector object and set its OnMessageReceived event handler
            connector = new SignalRConnector();
            connector.OnMessageReceived += UpdateReceivedMessages;
            await connector.InitAsync(); // Initialize the connector asynchronously
        }

        private IEnumerator ExecuteUpdateMethod(Message newMessage)
        {
            // Log the contents of the received message
            Debug.Log($"Received new message: {JsonConvert.SerializeObject(newMessage)}");

            // Change the color of a material (not currently being used)
            // MaterialController.ColorChanger();

            yield return null;
        }

        private void UpdateReceivedMessages(Message newMessage)
        {
            try
            {
                // Enqueue an update method to be executed on the main Unity thread
                UnityMainThreadDispatcher.Instance().Enqueue(ExecuteUpdateMethod(newMessage));

                // Enqueue an update to the PersonController object to update person data
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    personController.UpdatePersonData(newMessage.person_id.ToString(), newMessage.direction, newMessage.person_moving_line, newMessage.current_position, newMessage.status);
                    Debug.Log($"Person {newMessage.person_id} mline {newMessage.person_moving_line}");

                    // Log that the TextMeshPro was updated with the speed property of the received message (not currently being used)
                    // Debug.Log($"Updated TextMeshPro with speed: {newMessage.speed}");
                });
            }
            catch (Exception ex)
            {
                // Log any errors that occur while updating the received message
                Debug.LogError($"Error updating received message: {ex}");
            }
        }
    }
}
