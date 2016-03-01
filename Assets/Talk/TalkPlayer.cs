using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class TalkPlayer : MonoBehaviour
    {
        private TalkSentenceSequence _dialog;
        private DialogTextPlayer _dialogTextPlayer;
        [SerializeField]
        private Text _text;
        private GameObject _view;

        public bool IsPlaying
        {
            get { return !IsCurrentSentenceDone || !IsLastSentence; }
        }

        public bool IsLastSentence
        {
            get { return _dialog == null || string.IsNullOrEmpty(_dialog.NextTalk); }
        }

        public bool IsCurrentSentenceDone
        {
            get { return _dialogTextPlayer == null || _dialogTextPlayer.IsDone; }
        }

        private void Update()
        {
            UpdateText(Time.deltaTime);
            ContinueIfPossible();
        }

        private void UpdateText(float dt)
        {
            if (_dialogTextPlayer == null) return;
            _dialogTextPlayer.Update(dt);
            _text.text = _dialogTextPlayer.Text;
        }

        private void ContinueIfPossible()
        {
            if (!IsCurrentSentenceDone) return;
            if (!_dialog.ContinueAuto) return;
            Next();
        }

        public void Play(TalkSentenceSequence dialog)
        {
            if (IsPlaying) Debug.LogWarning("already playing.");
            ForcePlay(dialog);
        }

        public void Next()
        {
            if (IsLastSentence)
            {
                Destroy(gameObject);
            }
            else
            {
                if (!IsCurrentSentenceDone)
                    Debug.LogWarning("not yet done previous dialog.");
                ForcePlay(TalkDb.Inst.Get(_dialog.NextTalk));
            }
        }

        private void ForcePlay(TalkSentenceSequence dialog)
        {
            _dialog = dialog;
            _dialogTextPlayer = new DialogTextPlayer(new DialogTextPlayerSource(_dialog.Sentences));
            if (_view != null) Destroy(_view);
            if (dialog.View != null) _view = MakeView(dialog.View);
            _text.text = "";
        }

        private GameObject MakeView(TalkView view)
        {
            GameObject ret;
            if (view.Type == TalkViewType.Sprite)
            {
                ret = new GameObject("view");
                var image = ret.AddComponent<Image>();
                image.sprite = Resources.Load<Sprite>(view.Path);
            }
            else if (view.Type == TalkViewType.Prefab)
            {
                ret = Resources.Load<GameObject>(view.Path).Instantiate();
                if (!string.IsNullOrEmpty(view.AnimationTrigger))
                {
                    var animator = ret.GetComponent<Animator>();
                    animator.SetTrigger(view.AnimationTrigger);
                }
            }
            else
            {
                Debug.LogError("undefined: " + view.Type);
                return null;
            }

            var pivot = transform.FindChild("Pivots/" + view.Pivot);
            ret.transform.SetParent(pivot, false);
            ret.transform.localPosition = view.Position;
            return ret;
        }
    }
}