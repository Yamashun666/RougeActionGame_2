using System;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }
    public SkillOrbDragController skillOrbDragController;

    [Header("UIスロット（Q=0, W=1, E=2, R=3 の想定）")]
    public SkillSlotUI[] slots;
    public SkillExecutor executor;
    [Header("プレイヤーのSkillExecutor")]
    public SkillExecutor PlayerExecutor;
    [Header("プレイヤーのParameterBase（発動元）")]
    public ParameterBase playerParameter;
    private float coolTimeQ;
    private float coolTimeW;
    private float coolTimeE;
    private float coolTimeR;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("[SkillUIManager] slots が設定されていません。Inspectorで4つの SkillSlotUI を割り当ててください。");
        }
            if (PlayerExecutor == null)
        PlayerExecutor = FindFirstObjectByType<PlayerController>()?.GetComponent<SkillExecutor>();

        if (executor == null)
            executor = PlayerExecutor; // executor は PlayerExecutor を使う

        if (playerParameter == null)
            playerParameter = FindFirstObjectByType<PlayerController>()?.GetComponent<ParameterBase>();

        Debug.Log($"[SkillUIManager] executor={(executor ? executor.name : "null")}, playerParameter={(playerParameter ? playerParameter.name : "null")}");
    }
        void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) TryUseSkill(0);
        if (Input.GetKeyDown(KeyCode.W)) TryUseSkill(1);
        if (Input.GetKeyDown(KeyCode.E)) TryUseSkill(2);
        if (Input.GetKeyDown(KeyCode.R)) TryUseSkill(3);
        Debug.Log(coolTimeW);
        CTCounter();
    }

    /// <summary>
    /// 最初に空いているスロットへ登録
    /// </summary>
    public void RegisterSkillToNextSlot(SkillData skill, DroppedItem dropItem)
    {
        if (skill == null)
        {
            Debug.LogError("[SkillUIManager] skill が null です。");
            return;
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("[SkillUIManager] slots が未設定です。");
            return;
        }

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            if (slot.assignedSkill == null)
            {
                slot.SetSkill(skill, dropItem, skillOrbDragController.cachedIcon);
                Debug.Log($"[SkillUIManager] {skill.SkillName} をスロット {slot.slotIndex} に登録しました。");
                return;
            }
        }


        Debug.LogWarning("[SkillUIManager] 空きスロットがありません。");
    }

    public void TryUseSkill(int index)
    {
        Debug.Log("[SkillUIManager.TryUseSkill]TryUseSkillCalled");
        if (index < 0 || index >= slots.Length) return;
        SkillSlotUI slot = slots[index];

        if (slot.assignedSkill == null)
        {
            Debug.Log($"[{index}] スキル未登録");
            return;
        }
        if (slot.assignedSkill == null)
        {
            Debug.Log("[SkillUIManager.TryUseSkill]ExecuteSkill()を使用しました");
        }

        // 実際に発動
        if(index == 0)
        {
            if(coolTimeQ <= 0)
            {
                executor.ExecuteSkill(slot.assignedSkill, playerParameter,null);
            }
        }
        if(index == 1)
        {
            if(coolTimeW <= 0)
            {
                executor.ExecuteSkill(slot.assignedSkill, playerParameter,null);
            }
        }
        if(index == 2)
        {
            if(coolTimeE <= 0)
            {
                executor.ExecuteSkill(slot.assignedSkill, playerParameter,null);
            }
        }
        if(index == 3)
        {
            if(coolTimeR <= 0)
            {
                executor.ExecuteSkill(slot.assignedSkill, playerParameter,null);
            }
        }
        CTSetter(index,slot);
        Debug.Log($"[TryUseSkill] executor={executor?.name}, caster={playerParameter?.name}, skill={slot.assignedSkill?.SkillName}");
    }
    public void CTSetter(int index,SkillSlotUI slot)
    {
        if(index == 0)
        {
            coolTimeQ = slot.assignedSkill.CoolTime;
        }
        if(index == 1)
        {
            coolTimeW = slot.assignedSkill.CoolTime;
        }
        if(index == 2)
        {
            coolTimeE = slot.assignedSkill.CoolTime;
        }
        if(index == 3)
        {
            coolTimeR = slot.assignedSkill.CoolTime;
        }
    }
    private void CTCounter()
    {
        coolTimeQ -= Time.deltaTime;
        coolTimeW -= Time.deltaTime;
        coolTimeE -= Time.deltaTime;
        coolTimeR -= Time.deltaTime;
    }
    /// <summary>
    /// スロット番号を指定して登録（必要なら使用）
    /// </summary>
    public void RegisterSkillToSlot(int index, SkillData skill, DroppedItem dropItem)
    {
        if (slots == null || index < 0 || index >= slots.Length)
        {
            Debug.LogError($"[SkillUIManager] 不正なスロット index={index}");
            return;
        }
        slots[index].SetSkill(skill, dropItem, skillOrbDragController.cachedIcon);
    }
}
