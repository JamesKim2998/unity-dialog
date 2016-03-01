using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBalloonFlow
{
    private readonly List<DialogSentenceSequence> _flow;
    private readonly Func<int, GameObject> _targetProvider;
    public Action OnFinishAll;
    public Action<int> OnFinishOne;

    public SpeechBalloonFlow(List<DialogSentenceSequence> flow, Func<int, GameObject> targetProvider)
    {
        _flow = flow;
        _targetProvider = targetProvider;
    }

    public void Play()
    {
        Play(0);
    }

    private void Play(int idx)
    {
        if (idx >= _flow.Count) return;
        var sentences = _flow[idx];
        var target = _targetProvider(idx);
        if (target == null) return;
        var speechBalloonPlayer = SpeechBalloonManager.TryPlay(sentences, target);
        if (speechBalloonPlayer == null) return;
        speechBalloonPlayer.OnFinish += () => OnFinish(idx);
    }

    private void OnFinish(int idx)
    {
        OnFinishOne.CheckAndCall(idx);

        if (idx < _flow.Count - 1)
        {
            Play(idx + 1);
        }
        else if (idx == _flow.Count - 1)
        {
            OnFinishAll.CheckAndCall();
        }
        else
        {
            Debug.LogError("index out of range.");
        }
    }
}