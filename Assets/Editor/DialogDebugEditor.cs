using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpeechBalloonDebug))]
public class SpeechBalloonDebugEditor : ComponentEditor<SpeechBalloonDebug>
{
    private List<string> _fileList = new List<string>();

    protected override void OnEnable()
    {
        base.OnEnable();
        _fileList = FileSystemHelper.GetListOfFilesWithOutExtension("Assets/Resources/Dialog/SpeechBalloon", "*.json");
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        DrawLoadButton();
        DrawPlay();
    }

    private void DrawLoadButton()
    {
        GUILayout.Label("db");
        EditorUtil.DrawButtonList(_fileList, x => x, fileName => SpeechBalloonDb.inst.TryAppendWithDefaultDirectory(fileName, true));
    }

    private void DrawPlay()
    {
        GUILayout.Label("dialog");
        EditorUtil.DrawButtonList(SpeechBalloonDb.inst, x => x.Key, kv => SpeechBalloonManager.TryPlay(kv.Key, HouseSceneObjectType.Character));
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
        EditorUtil.DrawButtonList(_fileList, x => x, fileName => TalkDb.inst.TryAppendWithDefaultDirectory(fileName, true));
    }

    private void DrawPlay()
    {
        GUILayout.Label("dialog");
        EditorUtil.DrawButtonList(TalkDb.inst, x => x.Key, kv => TalkManager.TryPlay(kv.Key));
    }

    private void DrawControl()
    {
        var talkPlayer = FindObjectOfType<TalkPlayer>();
        if (talkPlayer == null) return;

        GUILayout.Label("control");
        if (GUILayout.Button("next"))
            talkPlayer.Next();
    }
}
