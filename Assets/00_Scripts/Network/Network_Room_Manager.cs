using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

public partial class Net_Mng : MonoBehaviour
{
    public async void JoinGameWithCode(string inputJoinCode)
    {
        if (string.IsNullOrEmpty(inputJoinCode))
        {
            Debug.Log("유효하지 않은 Join Code입니다.");
            return;
        }

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );

            StartClient();
            Debug.Log("Join Code로 게임에 접속 성공!");
        }
        catch (RelayServiceException e)
        {
            Debug.Log($"게임 접속 실패 : {e}");
        }
    }

    public async void StartMatchmaking()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("로그인되지 않았습니다.");
            return;
        }

        currentLobby = await FindAvailableLobby();

        if (currentLobby == null)
        {
            await CreateNewLobby();
        }
        else
        {
            await JoinLobby(currentLobby.Id);
        }
    }

    private async Task<Lobby> FindAvailableLobby()
    {
        // 예외 처리
        try
        {
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"로비 찾기 실패 {e}");
        }
        return null;
    }

    private async Task CreateNewLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤매칭방", maxPlayers);
            Debug.Log($"새로운 방 생성됨 {currentLobby.Id}");
            await AllocateRelayServerAndJoin(currentLobby);
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"로비 생성 실패 {e}");
        }
    }

    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log($"방에 접속되었습니다. {currentLobby.Id}");
            StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"로비 참가 실패 {e}");
        }
    }

    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            JoinCodeText.text = joinCode;
            Debug.Log($"Relay 서버 할당 완료. Join Code : {joinCode}");
        }
        catch (RelayServiceException e)
        {
            Debug.Log($"Relay 서버 할당 실패 {e}");
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("호스트가 시작되었습니다.");

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnHostDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        OnPlayerJoined();
    }

    private void OnHostDisconnected(ulong clientId)
    {
        if(clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnHostDisconnected;
        }
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("클라이언트가 연결되었습니다.");
    }
}
