using Avalonia;
using Avalonia.Controls;
using System.ComponentModel;

namespace LogicSimulator.Views.Shapes {
    public partial class NOT: GateBase, IGate, INotifyPropertyChanged {
        public override int TypeId => 2;

        public override int InputCount => 1;
        public override int OutputCount => 1;
        public override UserControl GetSelf() => this;
        protected override IGate GetSelfI => this;

        protected override void Init() {
            height = 30 * 2.5;
            InitializeComponent();
            DataContext = this;
        }


        public override Point[][] PinPoints { get {
            double X = EllipseSize - EllipseStrokeSize / 2;
            double X2 = base_size + width - EllipseStrokeSize / 2;
            double Y = height / 2;
            double PinWidth = base_size - EllipseSize + PinStrokeSize;
            return new Point[][] {
                new Point[] { new(X, Y), new(X + PinWidth, Y) }, // Единственный вход
                new Point[] { new(X2, Y), new(X2 + PinWidth, Y) }, // Единственный выход
            };
        } }


        public void InnerLogic(ref bool[] ins, ref bool[] outs) => outs[0] = !ins[0];
    }
}
