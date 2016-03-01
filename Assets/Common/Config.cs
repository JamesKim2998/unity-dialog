using UnityEngine;

namespace Dialog
{
    public class Config : MonoBehaviour
    {
        public static Config Inst;

        Config()
        {
            Inst = this;
        }

        public string DefaultSpeechBalloonDbDirectory;
        public string DefaultTalkDbDirectory;
        public string DefaultDbRootPath;
        public string DefaultSpeechBalloonDbFullDirectory { get { return DefaultDbRootPath + DefaultSpeechBalloonDbDirectory; } }
        public string DefaultTalkDbFullDirectory { get { return DefaultDbRootPath + DefaultTalkDbDirectory; } }
        public SpeechBalloonUi SpeechBalloonUi;
        public TalkPlayer TalkPlayer;
    }
}