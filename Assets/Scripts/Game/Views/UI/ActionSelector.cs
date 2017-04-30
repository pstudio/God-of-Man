using System.Collections.Generic;
using MarkLight;
using MarkLight.Views.UI;
using pstudio.GoM.Game.Controllers;

namespace pstudio.GoM.Game.Views.UI
{
    public class ActionSelector : UIView
    {
        public ObservableList<ActionController.GodAction> Actions;
        private ActionController _actionController;

        public override void Initialize()
        {
            Actions = new ObservableList<ActionController.GodAction>();
        }

        public void RegisterActions(ActionController controller, IEnumerable<ActionController.GodAction> actions)
        {
            Actions.AddRange(actions);
            _actionController = controller;
        }

        public void UpdateView()
        {
            Actions.ItemsModified();
        }

        public void ActionSelected()
        {
           _actionController.ActionSelected(Actions.SelectedIndex);
        }

        public void Clicked()
        {
            _actionController.ActionSelected(0);
        }
    }

}
