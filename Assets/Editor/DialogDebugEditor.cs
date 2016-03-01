using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (SpeechBalloonDebug))]
public class SpeechBalloonDebugEditor : ComponentEditor<SpeechBalloonDebug>
{
    private static GameObject _target;
    private List<string> _fileList = new List<string>();

    protected override void OnEnable()
    {
        base.OnEnable();
        _fileList = FileSystemHelper.GetListOfFilesWithOutExtension("Assets/Resources/Dialog/SpeechBalloon", "*.json");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
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
        GUILayout.Label("dialog");
        EditorUtil.DrawButtonList(SpeechBalloonDb.Inst, x => x.Key,
            kv => SpeechBalloonManager.TryPlay(kv.Key, Target.Target));
    }
}

[CustomEditor(typeof (TalkDebug))]
public class TalkDebugEditor : ComponentEditor<TalkDebug>
{
    private List<string> _fileList = new List<string>();

    protected override void OnEnable()
    {
        base.OnEnable();
        _fileList = FileSystemHelper.GetListOfFilesWithOutExtension("Assets/Resources/Dialog/Talk", "*.json");
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
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