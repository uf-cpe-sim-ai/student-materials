using System.Windows.Forms;

namespace AI.SteeringBehaviors.ApplicationPresentation
{
    public partial class RenderControl : UserControl
    {
        public RenderControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }
    }
}
