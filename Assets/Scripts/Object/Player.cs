﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player클래스가 턴의 흐름을 제어한다.
/// </summary>
public class Player : MovingTurnActor, TurnActor.IDamagable {
    public static Player Instance;
    public Armor equippedArmor = null;
    private readonly List<PartComponents> playerPartComponents = new();
    private Animator animator;
    private int defaultAttackDamage = 1;
    private Direction facing = Direction.Right;
    [SerializeField]
    private int hp = 100, maxHP = 100, shield = 20, maxShield = 20;
    private int moveCount = 0;
    [SerializeField]
    private List<GameObject> playerParts;
    public int HP => hp;
    public int MaxHP => maxHP;
    public int MaxShield => maxShield;
    public int Shield => shield;

    /// <summary>
    /// 모든 TurnActor들이 이 이벤트에 TurnUpdate()를 구독시키고,
    /// 플레이어 행동이 정해지면 이 이벤트를 호출해 이 이벤트에 구독된
    /// 수많은 TurnActor들의 TurnUpdate()가 실행된다.
    /// </summary>
    public static event Action OnTurnUpdate;

    /// <summary>
    /// 플레이어의 파츠의 필요한 컴포넌트만 담는 클래스
    /// GetComponent를 최대한 적게 쓰려고 만들었다.
    /// </summary>
    private class PartComponents {
        public Animator animator;
        public SpriteRenderer sprite;
        public Transform transform;
        public PartComponents(Animator animator, SpriteRenderer sprite, Transform transform) {
            this.animator = animator != null ? animator : throw new ArgumentNullException(nameof(animator));
            this.sprite = sprite != null ? sprite : throw new ArgumentNullException(nameof(sprite));
            this.transform = transform;
        }
    }

    public void AddMaxHP(int value) {
        maxHP += value;
    }

    /// <summary>
    /// 최대 쉴드 감소는 여기에 음수를 넣어서 표현한다.("음수를 더한다")
    /// </summary>
    public void AddMaxShield(int value) {
        maxShield += value;
    }

    public void ArmorEquip(Armor armor) {
        if (equippedArmor != null) {
            equippedArmor.OnUnequip();
        }
        equippedArmor = armor;
        equippedArmor.OnEquip(this);
    }

    public void HideOrShowArm(int showArm) {
        foreach (var part in playerPartComponents) {
            part.sprite.enabled = showArm == 1 ? true : false;
        }
    }

    public void TakeDamage(int damage) {
        if (shield < damage) {
            hp -= damage - shield;
            shield = 0;
        } else {
            shield -= damage;
        }
        equippedArmor?.OnHit();
        animator.SetTrigger("isHit");
    }

    /// <summary>
    /// 입력은 여기에서 받는다.
    /// </summary>
    public void TakeInput(Direction? inputDir) {
        if (inputDir == null) {
            nextAction = () => { };
            return;
        }
        if (ButtonManager.Instance.AttackMode) {
            nextAction = () => PlayerAttack((Direction)inputDir, defaultAttackDamage);
        } else {
            nextAction = () => Move((Direction)inputDir);
        }
    }

    protected override void Awake() {
        base.Awake();
        Instance = this;
        animator = GetComponent<Animator>();
        foreach (GameObject obj in playerParts) {
            playerPartComponents.Add(
                new PartComponents(obj.GetComponent<Animator>(), obj.GetComponent<SpriteRenderer>(), obj.transform));
        }
        TurnReady = true;
        StartsFacingRight = true;
    }

    protected override void DecideNextAction() {
    }

    /// <summary>
    /// 자신 스프라이트를 좌우로 뒤집는다.
    /// </summary>
    /// <param name="toRight">true면 오른쪽, false면 왼쪽을 보게 된다.</param>
    protected override void FlipSprite(bool toRight) {
        //먼저 자신을 뒤집는다
        base.FlipSprite(toRight);
        //자신의 모든 파츠를 플레이어와 같은 방향으로 뒤집는다
        foreach (var part in playerPartComponents) {
            part.sprite.flipX = spriteRenderer.flipX;
        }
    }

    protected override void TurnUpdate() {
        base.TurnUpdate();
        moveCount += 1;
        if (moveCount == 3) {
            moveCount = 0;
            hp -= 1;
        }
        equippedArmor?.OnTurnUpdate();
    }

    private new void Move(Direction dir) {
        base.Move(dir);
        facing = dir;
        animator.SetTrigger("isWalk");
    }

    private void PlayerAttack(Direction dir, int damage) {
        Vector3 direction = Vector3.zero;
        switch (dir) {
            case Direction.Left:
                direction = Vector3.left;
                FlipSprite(false);
                break;

            case Direction.Right:
                direction = Vector3.right;
                FlipSprite(true);
                break;

            case Direction.Up:
                direction = Vector3.up;
                break;

            case Direction.Down:
                direction = Vector3.down;
                break;
        }
        AttackPreTurn(transform.position + direction, damage);

        foreach (var obj in playerPartComponents) {
            obj.animator.SetTrigger("Attack");
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            nextAction = () => Move(Direction.Left);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            nextAction = () => Move(Direction.Right);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            nextAction = () => Move(Direction.Down);
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            nextAction = () => Move(Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.Z)) {
            nextAction = () => PlayerAttack(facing, 1);
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            nextAction = () => { };
        }

        if (nextAction != null && TurnReady) {
            //if (UIManager.Instance.UIActive) {
            //    UIManager.Instance.SetUIActive(false);
            //    nextAction = null;
            //} else {
            OnTurnUpdate();
            //}
        }
    }
}