using MarkLight.Views.UI;
using UnityEngine;

namespace pstudio.GoM.Menu.Views.UI
{
    public class MainMenu : UIView
    {
        public ViewSwitcher ContentViewSwitcher;

        public void StartGame()
        {
            ContentViewSwitcher.SwitchTo(1);
        }

        public void Options()
        {
            ContentViewSwitcher.SwitchTo(2);
        }

        public void Credits()
        {
            Debug.Log("Credits() called.");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }

}
