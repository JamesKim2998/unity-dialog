using System;
using UnityEngine;

namespace Dialog
{
    public class SpeechBalloonPlayer : MonoBehaviour
    {
        private TextPlayer _textPlayer;
        private bool _isFinished;
        private bool _isHavePlayed;
        private SpeechBalloonUi _ui;
        public Action OnFinish;

        public bool IsPlaying
        {
            get { return _textPlayer != null && !_textPlayer.IsDone; }
        }

        private void Start()
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
            OnFinish.CheckAndCall();
            Destroy(this);
        }

        private void Update()
        {
            var dt = Time.deltaTime;

            UpdatePosition();
            UpdateLetter(dt);

            if (!IsPlaying && _isHavePlayed && !_isFinished)
                Invoke("Finish", TextPlayer.SentenceProceedDelay);
        }

        private void UpdatePosition()
        {
            _ui.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        }

        private void UpdateLetter(float dt)
        {
            if (_textPlayer == null) return;
            _textPlayer.Update(dt);
            _ui.SetText(_textPlayer.Text);
        }

        public void Play(SentenceSequence dialog)
        {
            if (IsPlaying)
            {
                Debug.LogWarning("already playing. reset.");
                Reset();
            }

            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;

            _textPlayer = new TextPlayer(new TextPlayerSource(dialog));

            _ui = Config.Inst.SpeechBalloonUi.Instantiate();
            _ui.transform.SetParent(canvas.transform, false);
            _ui.SetText("");

            _isHavePlayed = true;
        }

        public void Reset()
        {
            if (_ui != null) Destroy(_ui.gameObject);
            _textPlayer = null;
        }
    }
}