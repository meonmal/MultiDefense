using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject _spawn_Prefab;
    [SerializeField]
    private Monster _spawn_Monster_Prefab;

    public static List<Vector2> Player_move_List = new List<Vector2>();
    public static List<Vector2> Other_move_List = new List<Vector2>();

    private List<Vector2> Player_spawn_list = new List<Vector2>();
    private List<Vector2> Other_spawn_list = new List<Vector2>();
    private List<bool> Player_spawn_list_Array = new List<bool>();
    private List<bool> Other_spawn_list_Array = new List<bool>();

    private void Start()
    {
        SetGrid();
        StartCoroutine(Spawn_Monster_Coroutine());
    }

    private void SetGrid()
    {
        Grid_Start(transform.GetChild(0), true);
        Grid_Start(transform.GetChild(1), false);

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            Player_move_List.Add(transform.GetChild(0).GetChild(i).position);
        }

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            Other_move_List.Add(transform.GetChild(1).GetChild(i).position);
        }
    }

    #region 몬스터 소환
    private IEnumerator Spawn_Monster_Coroutine()
    {
        //var go = Instantiate(_spawn_Monster_Prefab, move_list[0], Quaternion.identity);

        //GameManager.Instance.AddMonster(go);
        yield return new WaitForSeconds(1.0f);

        if (IsClient)
        {
            ServerMonsterSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            MonsterSpawn(NetworkManager.Singleton.LocalClientId);
        }

        StartCoroutine(Spawn_Monster_Coroutine());
    }
    #endregion

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ServerMonsterSpawnServerRpc(ulong clientId)
    {
        MonsterSpawn(clientId);
    }

    private void MonsterSpawn(ulong clientId)
    {
        var go = Instantiate(_spawn_Monster_Prefab);

        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();

        ClientMonsterSetClientRpc(networkObject.NetworkObjectId, clientId);
    }

    [ClientRpc]
    private void ClientMonsterSetClientRpc(ulong networkObjectId, ulong clientId)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject monsterNetworkObject))
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                monsterNetworkObject.transform.position = new Vector3(0, -3.0f, 0);
            }
            else
            {
                monsterNetworkObject.transform.position = new Vector3(0, 3.0f, 0);
            }
        }
    }

    #region Make Grid
    private void Grid_Start(Transform tt, bool Player)
    {
        SpriteRenderer parentSprite = tt.GetComponent<SpriteRenderer>();

        float parentwidth = parentSprite.bounds.size.x;
        float parentheight = parentSprite.bounds.size.y;

        float xCount = tt.localScale.x / 6;
        float yCount = tt.localScale.y / 3;

        for(int row = 0; row < 3; row++)
        {
            for(int col = 0; col < 6; col++)
            {
                float xPos = (-parentwidth / 2) + (col * xCount) + (xCount / 2);
                float yPos = (parentheight / 2) - (row * yCount) + (yCount / 2);

                switch (Player)
                {
                    case true:
                        Player_spawn_list.Add(new Vector2(xPos, yPos + tt.localPosition.y - yCount));
                        Player_spawn_list_Array.Add(false);
                        break;
                    case false:
                        Other_spawn_list.Add(new Vector2(xPos, yPos + tt.localPosition.y - yCount));
                        Other_spawn_list_Array.Add(false);
                        break;
                }
            }
        }
    }
    #endregion

    #region 캐릭터 소환
    //public void Summon()
    //{
    //    if(GameManager.Instance.Money < GameManager.Instance.SummonCount)
    //    {
    //        return;
    //    }

    //    GameManager.Instance.Money -= GameManager.Instance.SummonCount;
    //    GameManager.Instance.SummonCount += 2;

    //    int position_value = -1;
    //    var go = Instantiate(_spawn_Prefab);
    //    for( int i = 0; i < spawn_list_Array.Count; i++)
    //    {
    //        if (spawn_list_Array[i] == false)
    //        {
    //            position_value = i;
    //            spawn_list_Array[i] = true;
    //            break;
    //        }
    //    }

    //    go.transform.position = spawn_list[position_value];
    //}
    #endregion
}
