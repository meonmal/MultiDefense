using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class Network_Manager : MonoBehaviour
{
    // Lobby -> 플레이어가 원하는 게임을 찾거나, 새 게임을 만들고 대기할 수 있다.
    // Relay -> 매칭된 플레이어들의 Relay의 Join Code로 연결되어, 호스트-클라이언트 방식으로 실시간 멀티플레이 환경을 유지
    private Lobby currentLobby;

    public Button StartMatchButton, JoinMatchButton;
    public TMP_InputField fieldText;
    public Text JoinCodeText;

    /// <summary>
    /// async(비동기) -> 동시에 일어나지 않는다.
    /// 즉, 요청이 일어날 때 까지 결과값이 나오지 않는다.
    /// </summary>
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        StartMatchButton.onClick.AddListener(() => StartMatchmaking());
        JoinMatchButton.onClick.AddListener(() => JoinGameWithCode(fieldText.text));
    }

    public async void JoinGameWithCode(string inputJoinCode)
    {
        if(string.IsNullOrEmpty(inputJoinCode))
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
        catch(RelayServiceException e)
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

        if(currentLobby == null)
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
            if(queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log($"로비 찾기 실패 {e}");
        }
        return null;
    }

    private async Task CreateNewLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤매칭방", 2);
            Debug.Log($"새로운 방 생성됨 {currentLobby.Id}");
            await AllocateRelayServerAndJoin(currentLobby);
            StartHost();
        }
        catch(LobbyServiceException e)
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
        catch(LobbyServiceException e)
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
        catch(RelayServiceException e)
        {
            Debug.Log($"Relay 서버 할당 실패 {e}");
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("호스트가 시작되었습니다.");
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("클라이언트가 연결되었습니다.");
    }
}
