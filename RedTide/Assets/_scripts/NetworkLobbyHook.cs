using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

namespace GameDuel
{
    public class NetworkLobbyHook : LobbyHook 
    {
        public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
        {
            LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
            NetworkPlayer spaceship = gamePlayer.GetComponent<NetworkPlayer>();

            spaceship.name = lobby.name;
            spaceship.color = lobby.playerColor;
            spaceship.score = 0;
            spaceship.lifeCount = 3;
        }
    }
}