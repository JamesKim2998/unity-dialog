using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dialog
{
    [CustomEditor(typeof(SpeechBalloonDebug))]
    public class SpeechBalloonDebugEditor : ComponentEditor<SpeechBalloonDebug>
    {
        private List<string> _fileList = new List<string>();

        protected override void OnEnable()
        {
            base.OnEnable();
            _fileList = EditorUtil.GetListOfFilesWithOutExtension(Config.Inst.DefaultSpeechBalloonDbFullDirectory, "*.json");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!Application.isPlaying) return;
            DrawLoadButton();
            DrawPlay();
        }

        private void DrawLoadButton()
        {
            GUILayout.Label("db");
            EditorUtil.DrawButtonList(_fileList, x => x,
                fileName => SpeechBalloonDb.Inst.TryAppendWithDefaultDirectory(fileName, true));
        }

        private void DrawPlay()
        {
            if (Target.Target == null)
            {
                GUILayout.Label("please set target.", EditorUtil.MakeTextColor(Color.red));
                return;
            }

            GUILayout.Label("dialog");
            EditorUtil.DrawButtonList(SpeechBalloonDb.Inst, x => x.Key,
                kv => SpeechBalloonManager.TryPlay(kv.Key, Target.Target));
        }
    }

    [CustomEditor(typeof(SpeechBalloonTalkDebug))]
    public class SpeechBalloonTalkDebugEditor : ComponentEditor<SpeechBalloonTalkDebug>
    {
        private List<string> _fileList = new List<string>();

        protected override void OnEnable()
        {
            base.OnEnable();
            _fileList = EditorUtil.GetListOfFilesWithOutExtension(Config.Inst.DefaultSpeechBalloonTalkDbFullDirectory, "*.json");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!Application.isPlaying) return;
            DrawLoadButton();
            DrawPlay();
        }

        private void DrawLoadButton()
        {
            GUILayout.Label("db");
            EditorUtil.DrawButtonList(_fileList, x => x,
                fileName => SpeechBalloonTalkDb.Inst.TryAppendWithDefaultDirectory(fileName, true));
        }

        private void DrawPlay()
        {
            GUILayout.Label("dialog");
            EditorUtil.DrawButtonList(SpeechBalloonTalkDb.Inst, x => x.Key,
                kv => SpeechBalloonManager.TryPlayTalk(kv.Key, Map));
        }

        private GameObject Map(string targetKey)
        {
            for (var i = 0; i != Target.Targets.Count; ++i)
            {
                if (targetKey == Target.Targets[i])
                    return Target.Objects[i];
            }

            Debug.LogError("no target for key: " + targetKey);
            return null;
        }
    }


    [CustomEditor(typeof(TalkDebug))]
    public class TalkDebugEditor : ComponentEditor<TalkDebug>
    {
        private List<string> _fileList = new List<string>();

        protected override void OnEnable()
        {
            base.OnEnable();
            _fileList = EditorUtil.GetListOfFilesWithOutExtension(Config.Inst.DefaultTalkDbFullDirectory, "*.json");
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            if (!Application.isPlaying) return;
            DrawLoadButton();
            DrawPlay();
            DrawControl();
        }

        private void DrawLoadButton()
        {
            GUILayout.Label("db");
            EditorUtil.DrawButtonList(_fileList, x => x,
                fileName => TalkDb.Inst.TryAppendWithDefaultDirectory(fileName, true));
        }

        private static void DrawPlay()
        {
            GUILayout.Label("dialog");
            EditorUtil.DrawButtonList(TalkDb.Inst, x => x.Key, kv => TalkManager.TryPlay(kv.Key));
        }

        private static void DrawControl()
        {
            var talkPlayer = FindObjectOfType<TalkPlayer>();
            if (talkPlayer == null) return;

            GUILayout.Label("control");
            if (GUILayout.Button("next"))
                talkPlayer.Next();
        }
    }
}