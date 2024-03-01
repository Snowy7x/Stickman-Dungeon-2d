using Map;
using Map.Challenges;
using UnityEngine;

public enum Challenges
{
    EnemySpawner,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] GameObject player;
    [SerializeField] RoomChallenge[] challenges;
    [SerializeField] GameObject loadingCam;
    [SerializeField] GameObject loadingScreen;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        loadingScreen.SetActive(true);
        loadingCam.SetActive(true);
        player.SetActive(false);
    }
    
    public void StartGame()
    {
        loadingCam.SetActive(false);
        loadingScreen.SetActive(false);
        player.SetActive(true);
    }
    
    public RoomChallenge StartChallenge(Room room, Challenges challenge)
    {
        switch (challenge)
        {
            case Challenges.EnemySpawner:
                // Get the challenge from the list of challenges and cast it to the EnemySpawner class
                EnemySpawner enemySpawner = null;
                foreach (RoomChallenge roomChallenge in challenges)
                {
                    if (roomChallenge is EnemySpawner spawner)
                    {
                        enemySpawner = spawner;
                        break;
                    }
                }
                if (enemySpawner == null) return null;
                // Initialize the challenge
                enemySpawner.Initialize(room, 0);
                return enemySpawner;
            
        }

        return null;
    }
}
