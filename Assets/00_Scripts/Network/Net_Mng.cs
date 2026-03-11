using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public partial class Net_Mng : MonoBehaviour
{
    // Lobby -> 플레이어가 원하는 게임을 찾거나, 새 게임을 만들고 대기할 수 있다.
    // Relay -> 매칭된 플레이어들의 Relay의 Join Code로 연결되어, 호스트-클라이언트 방식으로 실시간 멀티플레이 환경을 유지
    private Lobby currentLobby;

    private const int maxPlayers = 2;
    private string gamePlaySceneName = "GamePlayScene";
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

    
}
