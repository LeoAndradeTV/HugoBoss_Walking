using System;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SignalRConnector
{
    // This action will be invoked when a new message is received
    public Action<Message> OnMessageReceived;
    // HubConnection is used to establish a connection to a SignalR hub
    private HubConnection connection;

    // Initialize the connection to the SignalR hub
    public async Task InitAsync()
    {
        Debug.Log("Connecting to SignalR hub at https://localhost:7111/StudioHub");
        // Build the connection to the SignalR hub
        connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7111/StudioHub")
                .WithAutomaticReconnect()
                .Build();

        // Listen for "UnityMessage" event and deserialize the received messages
        connection.On<string>("UnityMessage", (message) =>
        {
            // Deserialize the message into a List of Message objects
            List<Message> receivedMessages = JsonConvert.DeserializeObject<List<Message>>(message);
            // Loop through the received messages and invoke the OnMessageReceived action for each message
            foreach (Message receivedMessage in receivedMessages)
            {
                Debug.Log($"Received message: {receivedMessage}");
                OnMessageReceived?.Invoke(receivedMessage);
            }
        });

        // Listen for "SendMessageAsync" event and deserialize the received message
        connection.On<string>("SendMessageAsync", (message) =>
        {
            // Deserialize the message into a Message object
            Message receivedMessage = JsonConvert.DeserializeObject<Message>(message);
            // Invoke the OnMessageReceived action with the received message
            OnMessageReceived?.Invoke(receivedMessage);
        });

        // Start the SignalR connection
        await StartConnectionAsync();
    }

    // Send a message to the SignalR hub
    public async Task SendMessageAsync(Message message)
    {
        try
        {
            // Serialize the message into JSON
            string json = JsonConvert.SerializeObject(message);
            // Invoke the "SendMessageAsync" method on the SignalR hub with the JSON message and a user identifier
            await connection.InvokeAsync("SendMessageAsync", "user", json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error {ex.Message}");
        }
    }

    // Start the SignalR connection
    private async Task StartConnectionAsync()
    {
        try
        {
            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error {ex.Message}");
        }
    }

}

// This is the Message class that represents the data of a received message
public class Message
{
    public int person_id { get; set; }
    public string status { get; set; } = "";
    public string direction { get; set; } = "";
    public int person_moving_line { get; set; }
    public float current_position { get; set; }

    // Override the ToString method to return a JSON string representation of the Message object
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
