using UnityEngine;

public class SharedWebData : MonoBehaviour
{
    public static SharedWebData Instance { get; private set; }

    public Profile playerProfile;
    public Profile lastEnemyProfile;
    public Lobby_participants lastLobbyParticipants;
    public Rock_paper_scissors_game lastGameData;

    private void Awake()
    {
        Instance = this;
    }
}
