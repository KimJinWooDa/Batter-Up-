// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Network
using Fusion;


public class Player : NetworkBehaviour
{
    [Networked] public Turn PlacedTurn { get; private set; } = Turn.None;

    // Conditions
    public bool IsMyTurn => GameManager.Instance.CurrentTurn == PlacedTurn;

    public override void Spawned()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (HasInputAuthority) PlaceTeamRpc(Runner.LocalPlayer.PlayerId);
    }

    private void Update()
    {
        // Turn 을 바꿀 수 있는 테스트 코드
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.Space))
        {
            RequestTurnFinished();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void PlaceTeamRpc(int playerId)
    {
        // Set the player turn via the player id (even number is blue, odd number is purple)
        PlacedTurn = (playerId % 2) == 0 ? Turn.Blue : Turn.Purple;
    }

    /// <summary>
    /// 현재 차례가 본인 차례인 경우에만 턴을 넘길 수 있는 함수
    /// </summary>
    public void RequestTurnFinished()
    {
        if(IsMyTurn) GameManager.Instance.RequestTurnFinishedRpc();
    }
}
