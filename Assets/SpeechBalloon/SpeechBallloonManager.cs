using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SpeechBalloonManager
{
    public static SpeechBalloonPlayer TryPlay(string key, GameObject target)
    {
        return TryPlay(SpeechBalloonDb.inst.Get(key), target);
    }

    public static SpeechBalloonPlayer TryPlay(DialogSentenceSequence sentences, GameObject target)
    {
        var player = target.AddComponent<SpeechBalloonPlayer>();
        player.Play(sentences);
        return player;
    }

    public static SpeechBalloonFlow TryPlay(List<string> sentencesKeyList, Func<string, GameObject> targetProvider)
    {
        var flowSentences = sentencesKeyList.Select<string, DialogSentenceSequence>(x => SpeechBalloonDb.inst.Get(x)).ToList();
        var targetProviderByIndex = new Func<int, GameObject>(i => targetProvider(sentencesKeyList[i]));
        var flow = new SpeechBalloonFlow(flowSentences, targetProviderByIndex);
        flow.Play();
        return flow;

    }

    public static SpeechBalloonFlow TryPlayTalk(string key, Func<string, GameObject> targetProvider)
    {
        var talk = SpeechBalloonTalkDb.inst.Get(key);
        var flowSentences = talk.Select<TargetedDialogSentenceSequence, DialogSentenceSequence>(x => x.sentences).ToList();
        var targetProviderByIndex = new Func<int, GameObject>(i => targetProvider(talk[i].target));
        var flow = new SpeechBalloonFlow(flowSentences, targetProviderByIndex);
        flow.Play();
        return flow;
    }
}
