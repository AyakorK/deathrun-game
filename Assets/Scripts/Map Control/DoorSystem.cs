using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject newRoomPrefab;
    private Transform player;
    private GameObject currentRoom;

    private GameTracker gameTracker;
    public static bool isInCorridorX7 = false;

    private void Start()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        gameTracker = FindObjectOfType<GameTracker>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (isInCorridorX7)
            {
                gameTracker.ClearCorridorEvents();
                isInCorridorX7 = false;
            }

            currentRoom = GameObject.FindGameObjectWithTag("Map");
            DeleteCurrentRoom();
            Vector3 newRoomPosition = new Vector3(4.1f, 2.8f, 0.0f);
            GameObject newRoom = Instantiate(newRoomPrefab, newRoomPosition, Quaternion.identity);
            PlacePlayerInNewRoom(newRoom);
            currentRoom = newRoom;

            GameObject[] traps = GameObject.FindGameObjectsWithTag("trap");
            if (traps == null)
            {
                UnityEngine.Debug.Log("No traps found");
            }
            else
            {
                foreach (GameObject trap in traps)
                {
                    Destroy(trap);
                }
            }


            if (newRoom.name.Contains("CorridorX7"))
            {
                gameTracker.SpawnCorridorEvents();
                isInCorridorX7 = true;
                UnityEngine.Debug.Log("Coucou");
            }
            else
            {
                if (gameTracker != null)
                {
                    gameTracker.NextStage();
                }
            }


        }
    }

    void PlacePlayerInNewRoom(GameObject newRoom)
    {
        Transform spawnPoint = newRoom.transform.Find("SpawnPoint");
        Vector3 spawnPosition = spawnPoint.position;
        player.position = spawnPosition;
    }

    void DeleteCurrentRoom()
    {
        Destroy(currentRoom);
    }
}
