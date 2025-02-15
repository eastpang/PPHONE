﻿using UnityEngine;

public class BossRoom : MonoBehaviour {
    [SerializeField] private GameObject BossRoomWalls;
    private bool bossActive = false;
    private GameObject boss;
    [SerializeField] private GameObject NextStagePortal;
    private void Start() {
        BossRoomWalls.SetActive(false);
        boss = Instantiate(BossManager.Instance.GetBoss(), transform);
        boss.SetActive(false);
        NextStagePortal.SetActive(false);
    }

    private void Update() {
        //플레이어가 방의 중앙에서 3칸 이내의 거리에 오면
        if (!bossActive &&
            Mathf.Abs((Player.Position - transform.position).magnitude) <= 4.0f) {
            bossActive = true;
            //보스 소환, 방 문 닫힘
            BossRoomWalls.SetActive(true);
            boss.SetActive(true);
        }

        if (boss != null) {
            return;
        }
        //방 문 다시열림, 포탈 소환
        Destroy(BossRoomWalls);
        if (GameManager.Instance.StageNumber != GameManager.FinalStageNumber) {
            NextStagePortal.SetActive(true);
            NextStagePortal.transform.SetParent(null, true);
            Destroy(gameObject);
        } else {
            UIScript.GameClear = true;
        }
    }
}