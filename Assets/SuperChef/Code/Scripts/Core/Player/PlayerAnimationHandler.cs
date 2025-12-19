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
    [SerializeField] private HoldableProvider holdableProvider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackParamID = Animator.StringToHash(ATTACK_STRING);
    }

    [Inject]
    private void Init(GameInputReader inputReader)
    {
        this.inputReader = inputReader;

    }

    public override void OnNetworkSpawn()
    {
       if (!IsOwner) return;
         inputReader.AttackEvent += TriggerAttackAnimation;
    }
    private void TriggerAttackAnimation()
    {
        if(holdableProvider.CurrentHoldableItemSO == null) return;

        animator.SetTrigger(attackParamID);
    }


    public override void OnNetworkDespawn()
    {
       if (!IsOwner) return;
        inputReader.AttackEvent -= TriggerAttackAnimation;
    }

}
