using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PlayerAnimationHandler : NetworkBehaviour
{
    private Animator animator;
    private int attackParamID;
    private const string ATTACK_STRING = "Attack";
    private GameInputReader inputReader;
    private PickUp pickUp;
    [SerializeField] private HoldableProvider holdableProvider;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackParamID = Animator.StringToHash(ATTACK_STRING);
    }

    [Inject]
    private void Init(GameInputReader inputReader, PickUp pickUp)
    {
        this.inputReader = inputReader;
        this.pickUp = pickUp;
    }

    public void TriggerAttackAnimation()
    {
        if (holdableProvider.CurrentHoldableItemSO == null || pickUp.CurrentPickableObject != null) return;

        animator.SetTrigger(attackParamID);
    }


}
