// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;

/// <summary>
/// Turn 을 정의하는 열거형.<br></br>
/// None : 아직 팀이 배치 되지 않은 상태, Blue : 파란 팀, Purple : 보라 팀
/// </summary>
public enum Turn
{
    None,
    Blue,
    Purple,
}

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    [Networked] public Turn CurrentTurn { get; set; } = Turn.None;

    /// <summary>
    /// 오브젝트가 스폰 될 때 호출 되는 함수
    /// </summary>
    public override void Spawned()
    {
        Initialize();
    }

    /// <summary>
    /// 게임이 시작 될 때 마다 호출 되는 함수
    /// </summary>
    private void Initialize()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(this);

        // Set the current turn
        CurrentTurn = Turn.Blue;
    }

    /// <summary>
    /// 차례를 넘길 때 호출하는 함수.<br></br>
    /// 이 함수를 호출하게 되면 다음 턴으로 넘어가게 됩니다.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestTurnFinishedRpc()
    {
        // Set the current turn
        CurrentTurn = CurrentTurn == Turn.Blue ? Turn.Purple : Turn.Blue;

        print($"Turn Changed. Now {CurrentTurn}");
    }
}