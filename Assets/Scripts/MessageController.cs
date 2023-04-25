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

        private void UpdateReceivedMessages(List<Message> newMessage)
        {
            try
            {
                // Enqueue an update method to be executed on the main Unity thread
                //UnityMainThreadDispatcher.Instance().Enqueue(ExecuteUpdateMethod(newMessage));

                // Enqueue an update to the PersonController object to update person data
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    foreach (Message m in newMessage)  
                    personController.UpdatePersonData(m.person_id.ToString(), m.direction, m.person_moving_line,
                        m.current_position, m.status);
                });
            }
            catch (Exception ex)
            {
                // Log any errors that occur while updating the received message
               // Debug.LogError($"Error updating received message: {ex}");
            }
        }
    }
}
