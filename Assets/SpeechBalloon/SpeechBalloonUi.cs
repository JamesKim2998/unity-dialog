using UnityEngine;
using UnityEngine.UI;

public class SpeechBalloonUi : MonoBehaviour
{
    [SerializeField] private Text _text;

    public void SetText(string text)
    {
        _text.text = text;
    }
}