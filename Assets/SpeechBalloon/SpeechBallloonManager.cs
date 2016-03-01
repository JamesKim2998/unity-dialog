using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialog
{
    public static class SpeechBalloonManager
    {
        public static SpeechBalloonPlayer TryPlay(string key, GameObject target)
        {
            return TryPlay(SpeechBalloonDb.Inst.Get(key), target);
        }

        public static SpeechBalloonPlayer TryPlay(SentenceSequence sentences, GameObject target)
        {
            var player = target.AddComponent<SpeechBalloonPlayer>();
            player.Play(sentences);
            return player;
        }

        public static SpeechBalloonFlow TryPlay(List<string> sentencesKeyList, Func<string, GameObject> targetProvider)
        {
            var flowSentences = sentencesKeyList.Select(x => SpeechBalloonDb.Inst.Get(x)).ToList();
            var targetProviderByIndex = new Func<int, GameObject>(i => targetProvider(sentencesKeyList[i]));
            var flow = new SpeechBalloonFlow(flowSentences, targetProviderByIndex);
            flow.Play();
            return flow;
        }

        public static SpeechBalloonFlow TryPlayTalk(string key, Func<string, GameObject> targetProvider)
        {
            var talk = SpeechBalloonTalkDb.Inst.Get(key);
            var flowSentences = talk.Select(x => x.Sentences).ToList();
            var targetProviderByIndex = new Func<int, GameObject>(i => targetProvider(talk[i].Target));
            var flow = new SpeechBalloonFlow(flowSentences, targetProviderByIndex);
            flow.Play();
            return flow;
        }
    }
}