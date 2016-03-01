using System;
using UnityEngine;

namespace Dialog
{
    public interface IDialogTextPlayerSource
    {
        int Count { get; }
        DialogSentence this[int i] { get; }
    }

    public class DialogTextPlayerSource : IDialogTextPlayerSource
    {
        private readonly DialogSentenceSequence _sentenceSequence;

        public DialogTextPlayerSource(DialogSentenceSequence sentenceSequence)
        {
            _sentenceSequence = sentenceSequence;
        }

        int IDialogTextPlayerSource.Count
        {
            get { return _sentenceSequence.Sentences.Count; }
        }

        DialogSentence IDialogTextPlayerSource.this[int i]
        {
            get { return _sentenceSequence.Sentences[i]; }
        }
    }

    public class DialogTextPlayer
    {
        public const float SentenceProceedDelay = 1;
        private const float LettersPerSecond = 6;
        private readonly RichTextStroller _richTextStroller = new RichTextStroller();
        private readonly IDialogTextPlayerSource _source;
        private float _numberOfLettersToShow;
        private string _oldText;
        private float _timeLeftToProceed = SentenceProceedDelay;
        public Action<int> OnSentenceProceed;

        public DialogTextPlayer(IDialogTextPlayerSource source)
        {
            Text = "";
            _source = source;
            _richTextStroller.Start(_source[0].Text);
        }

        public bool IsPlaying
        {
            get { return CurrentSentenceIndex < _source.Count; }
        }

        public bool IsDone
        {
            get { return !IsPlaying; }
        }

        public string Text { get; private set; }
        public int CurrentSentenceIndex { get; private set; }

        public void Update(float dt)
        {
            if (IsDone) return;
            var canAppendText = UpdateNumberOfCharactersToBeAppended(dt);
            if (canAppendText)
            {
                AppendSentence();
            }
            else
            {
                ProceedSentenceWithTimer(dt);
            }
        }

        private bool UpdateNumberOfCharactersToBeAppended(float dt)
        {
            var sentence = _source[CurrentSentenceIndex];
            _numberOfLettersToShow += (int)sentence.Speed * LettersPerSecond * dt;
            var newLetterIndex = _richTextStroller.Text.Length + (int)_numberOfLettersToShow;
            return newLetterIndex <= sentence.Text.Length;
        }

        private void AppendSentence()
        {
            Debug.Assert(!IsDone, "already done.");
            for (var i = 0; i < (int)_numberOfLettersToShow; ++i)
                _richTextStroller.Next();
            Text = _oldText + _richTextStroller.Text;
            _numberOfLettersToShow -= (int)_numberOfLettersToShow;
        }

        private void ProceedSentenceWithTimer(float dt)
        {
            if (_timeLeftToProceed <= 0)
            {
                ProceedSentence();
                _timeLeftToProceed = SentenceProceedDelay;
            }

            _timeLeftToProceed -= dt;
        }

        private void ProceedSentence()
        {
            var oldSentence = _source[CurrentSentenceIndex];
            ++CurrentSentenceIndex;
            _numberOfLettersToShow = 0;
            _oldText = oldSentence.Clear ? "" : Text;
            _richTextStroller.StopAndReset();
            if (!IsDone) _richTextStroller.Start(_source[CurrentSentenceIndex].Text);
            OnSentenceProceed.CheckAndCall(CurrentSentenceIndex);
        }
    }
}