using MarkLight.Views.UI;

namespace pstudio.GoM.Menu.Views.UI
{
    public class Options : UIView
    {
        public float Volume;
        public bool EasyMode;
        public string PlayerName;

        public void ResetDefaults()
        {
            SetValue(() => Volume, 75f);
            SetValue(() => EasyMode, true);
            SetValue(() => PlayerName, "Player");
        }
    }

}
