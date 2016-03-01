using System;
using UnityEngine;

public class SpeechBalloonPlayer : MonoBehaviour
{
    private bool _isHavePlayed = false;
    private bool _isFinished = false;
    public bool isPlaying { get { return _dialogTextPlayer != null && !_dialogTextPlayer.isDone; } }

    private DialogTextPlayer _dialogTextPlayer;
    private SpeechBalloonUi _ui;

    public Action onFinish;

    void Start()
    {
        UpdatePosition();
    }

    private void OnDestroy()
    {
        Reset();
    }

    private void Finish()
    {
        if (_isFinished)
        {
            Debug.LogWarning("call finish again.");
            return;
        }

        _isFinished = true;
        onFinish.CheckAndCall();
        Destroy(this);
    }

    private void Update()
    {
        var dt = Time.deltaTime;

        UpdatePosition();
        UpdateLetter(dt);

        if (!isPlaying && _isHavePlayed && !_isFinished)
            Invoke("Finish", DialogTextPlayer.SentenceProceedDelay);
    }

    private void UpdatePosition()
    {
        _ui.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
    }

    private void UpdateLetter(float dt)
    {
        if (_dialogTextPlayer == null) return;
        _dialogTextPlayer.Update(dt);
        _ui.SetText(_dialogTextPlayer.text);
    }

    public void Play(DialogSentenceSequence dialog)
    {
        if (isPlaying)
        {
            Debug.LogWarning("already playing. reset.");
            Reset();
        }

        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        _dialogTextPlayer = new DialogTextPlayer(new DialogTextPlayerSource(dialog));

        _ui = PrefabDb.inst.speechBalloonUi.Instantiate();
        _ui.transform.SetParent(canvas.transform, false);
        _ui.SetText("");

        _isHavePlayed = true;
    }

    public void Reset()
    {
        if (_ui != null) Destroy(_ui.gameObject);
        _dialogTextPlayer = null;
    }
}
