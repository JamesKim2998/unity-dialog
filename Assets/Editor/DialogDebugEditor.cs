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