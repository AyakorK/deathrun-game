using System;
using UnityEngine;
using UnityEngine.UI;
using SocketIOClient;
using CandyCoded.env;
using Newtonsoft.Json.Linq;
using System.Collections;

public class WaitingTrapper : MonoBehaviour
{
    private SocketIOUnity clientSocket;
    private bool gamePaused = true;

    private GameObject overlay;
    private Text messageText;
    private Text codeText;

    public Font messageFont;
    public PlayerMovement playerMovement;

    private string roomCode;
    private bool trapperJoined = false;
    private bool codeChanged = false;
    private bool coroutineStarted = false;

    void Start()
    {
        clientSocket = SocketManager.Instance.ClientSocket;
        CreateOverlay();
        BlockPlayerMovement();
        RegisterSocketEvents();
    }

    private void RegisterSocketEvents()
    {
        clientSocket.On("trapper:join", _ => OnTrapperJoin());
        clientSocket.On("rooms:create", response => OnRoomCreate(response));
    }

    private void OnTrapperJoin()
    {
        Debug.Log("Trapper joined the room!");
        trapperJoined = true;
    }

    private void OnRoomCreate(SocketIOResponse response)
    {
        Debug.Log("Room created!");
        JArray roomDataArray = JArray.Parse(response.ToString());
        roomCode = roomDataArray[0]["code"].Value<string>();

        if (!string.IsNullOrEmpty(roomCode))
        {
            codeChanged = true;
        }
    }

    private void CopyRoomCodeToClipboard()
    {
        try
        {
            GUIUtility.systemCopyBuffer = roomCode;
            Debug.Log("Room code copied to clipboard: " + roomCode);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to copy to clipboard: " + e.Message);
        }
    }

    private void CreateOverlay()
    {
        var canvasObject = CreateCanvas();
        overlay = CreateOverlayImage(canvasObject);
        messageText = CreateTextObject(canvasObject, "Waiting for trapper to join...", 50, Color.red, new Vector2(0.5f, 0.5f));
        codeText = CreateTextObject(canvasObject, "Waiting for the room code...", 30, Color.white, new Vector2(0.5f, 0.1f));
    }

    private GameObject CreateCanvas()
    {
        var canvasObject = new GameObject("OverlayCanvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvasObject;
    }

    private GameObject CreateOverlayImage(GameObject parent)
    {
        var overlay = new GameObject("OverlayImage");
        overlay.transform.parent = parent.transform;

        var overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.8f);

        var overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        return overlay;
    }

    private Text CreateTextObject(GameObject parent, string text, int fontSize, Color color, Vector2 anchor)
    {
        var textObject = new GameObject(text + "Text");
        textObject.transform.parent = parent.transform;
        var textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = messageFont;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAnchor.MiddleCenter;

        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = anchor;
        textRect.anchorMax = anchor;
        textRect.sizeDelta = new Vector2(800, 200);
        textRect.anchoredPosition = Vector2.zero;

        return textComponent;
    }

    private void BlockPlayerMovement()
    {
        gamePaused = true;
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void ResumePlayerMovement()
    {
        gamePaused = false;
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        Destroy(overlay.transform.parent.gameObject);
        Destroy(this);
    }

    private IEnumerator StartCountdown(int countdownTime)
    {
        while (countdownTime > 0)
        {
            Debug.Log("Countdown: " + countdownTime);
            messageText.text = $"Trapper joined! Time to place traps: {countdownTime}s";
            yield return new WaitForSecondsRealtime(1);
            countdownTime--;
        }
        ResumePlayerMovement();
    }

    void Update()
    {
        if (trapperJoined)
        {
            trapperJoined = false; // Reset the flag
            coroutineStarted = true;
            StartCoroutine(StartCountdown(5)); // Start countdown for traps
        }

        if (gamePaused && !coroutineStarted)
        {
            messageText.text = "Waiting for trapper to join...";
        }

        if (codeChanged)
        {
            codeChanged = false;
            codeText.text = $"Room Code: {roomCode} (copied to clipboard)";
            CopyRoomCodeToClipboard();
        }
    }
}
