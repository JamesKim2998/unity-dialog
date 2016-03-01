using UnityEngine;
using UnityEngine.UI;

public class TalkPlayer : MonoBehaviour
{
    public bool isPlaying { get { return !isCurrentSentenceDone || !isLastSentence; } }
    public bool isLastSentence { get { return _dialog == null || string.IsNullOrEmpty(_dialog.nextTalk); } }
    public bool isCurrentSentenceDone { get { return _dialogTextPlayer == null || _dialogTextPlayer.isDone; } }

    private TalkSentenceSequence _dialog;
    private DialogTextPlayer _dialogTextPlayer;
    private GameObject _view;
    [SerializeField]
    private Text _text;

    private void Update()
    {
        UpdateText(Time.deltaTime);
        ContinueIfPossible();
    }

    private void UpdateText(float dt)
    {
        if (_dialogTextPlayer == null) return;
        _dialogTextPlayer.Update(dt);
        _text.text = _dialogTextPlayer.text;
    }

    private void ContinueIfPossible()
    {
        if (!isCurrentSentenceDone) return;
        if (!_dialog.continueAuto) return;
        Next();
    }

    public void Play(TalkSentenceSequence dialog)
    {
        if (isPlaying) Debug.LogWarning("already playing.");
        ForcePlay(dialog);
    }

    public void Next()
    {
        if (isLastSentence)
        {
            Destroy(gameObject);
        }
        else
        {
            if (!isCurrentSentenceDone)
                Debug.LogWarning("not yet done previous dialog.");
            ForcePlay(TalkDb.inst.Get(_dialog.nextTalk));
        }
    }

    private void ForcePlay(TalkSentenceSequence dialog)
    {
        _dialog = dialog;
        _dialogTextPlayer = new DialogTextPlayer(new DialogTextPlayerSource(_dialog.sentences));
        if (_view != null) Destroy(_view);
        if (dialog.view != null) _view = MakeView(dialog.view);
        _text.text = "";
    }

    private GameObject MakeView(TalkView view)
    {
        GameObject ret = null;
        if (view.type == TalkViewType.Sprite)
        {
            ret = new GameObject("view");
            var image = ret.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>(view.path);
        }
        else if (view.type == TalkViewType.Prefab)
        {
            ret = Resources.Load<GameObject>(view.path).Instantiate();
            if (!string.IsNullOrEmpty(view.animationTrigger))
            {
                var animator = ret.GetComponent<Animator>();
                Debug.Assert(animator != null);
                animator.SetTrigger(view.animationTrigger);
            }
        }
        else
        {
            Debug.LogError("undefined: " + view.type);
            return null;
        }

        var pivot = transform.FindChild("Pivots/" + view.pivot);
        ret.transform.SetParent(pivot, false);
        ret.transform.localPosition = view.position;
        return ret;
    }
}
