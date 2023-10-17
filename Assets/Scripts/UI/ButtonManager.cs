﻿using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {
    [SerializeField]
    private Button UpButton, DownButton, LeftButton, RightButton, ModeButton, WaitButton;
    public static ButtonManager Instance;
    /// <summary>버튼이 공격모드인가? 이동모드인가?</summary>
    public bool AttackMode { get; private set; }
    private void Awake() {
        Instance = this;
        AttackMode = false;
        UpButton.onClick.AddListener(() => Player.Instance.TakeInput(TurnActor.Direction.Up));
        DownButton.onClick.AddListener(() => Player.Instance.TakeInput(TurnActor.Direction.Down));
        LeftButton.onClick.AddListener(() => Player.Instance.TakeInput(TurnActor.Direction.Left));
        RightButton.onClick.AddListener(() => Player.Instance.TakeInput(TurnActor.Direction.Right));
        ModeButton.onClick.AddListener(() => AttackMode = !AttackMode);
        WaitButton.onClick.AddListener(() => Player.Instance.TakeInput(null));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Player.Instance.TakeInput(TurnActor.Direction.Left);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Player.Instance.TakeInput(TurnActor.Direction.Right);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Player.Instance.TakeInput(TurnActor.Direction.Down);
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Player.Instance.TakeInput(TurnActor.Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            Player.Instance.TakeInput(null);
        } else if (Input.GetKeyDown(KeyCode.LeftShift)) {
            AttackMode = !AttackMode;
        }
    }
}