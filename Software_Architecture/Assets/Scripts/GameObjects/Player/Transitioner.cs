using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitioner : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();

        EventBus<OnLevelUnloadedEvent>.OnEvent += TransitionOut;
    }

    private void OnDestroy()
    {
        EventBus<OnLevelUnloadedEvent>.OnEvent -= TransitionOut;
    }

    private void TransitionOut(OnLevelUnloadedEvent onLevelUnloadedEvent)
    {
        _anim.SetTrigger("TransitionOut");
    }

    public void SwitchToMenu()
    {
        GameManager.Instance.LoadSceneSpecific(0);
    }
}
