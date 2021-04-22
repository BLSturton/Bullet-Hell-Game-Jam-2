using UnityEngine;
using Mirror;

public class ServerManager : NetworkManager
{
    [SerializeField] private GameObject _life;
    bool AlreadySpawned = false;
    private GameObject Life;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Vector3 Pos;
        if (AlreadySpawned)
        {
            Pos = new Vector3(Life.transform.position.x, Life.transform.position.y, Life.transform.position.z);
            Debug.Log(Pos);
        }
        else
        {
            Pos = new Vector3(0, 1f, 0);
            AlreadySpawned = true;
            SpawnLife();
        }
        GameObject player = Instantiate(playerPrefab, new Vector3(0,1f,0), Quaternion.identity);
        //Spawns on the server and gets connected to a client, meaning a client now owns this
        NetworkServer.AddPlayerForConnection(conn, player);//you cant test isLocalPlayer without this?
    }

    [Server]
    private void SpawnLife()
    {
        AlreadySpawned = true;
        Life = Instantiate(_life, Vector3.zero, Quaternion.Euler(0f, 0f, Random.Range(-360f, 360f)));
        NetworkServer.Spawn(Life);

    }
}
