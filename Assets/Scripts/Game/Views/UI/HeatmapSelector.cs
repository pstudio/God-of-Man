using MarkLight;
using MarkLight.Views.UI;
using pstudio.GoM.Game.Views.Heatmap;

namespace pstudio.GoM.Game.Views.UI
{
    public class HeatmapSelector : UIView
    {
        public ObservableList<string> Heatmaps;

        internal HeatmapView _heatmapView;

        public override void Initialize()
        {
            Heatmaps = new ObservableList<string>();
        }

        private void Start()
        {
            Heatmaps.AddRange(_heatmapView.GetHeatmapNames());
        }

        public void MapSelected()
        {
            _heatmapView.ShowMap(Heatmaps.SelectedIndex);
        }
    }
}
