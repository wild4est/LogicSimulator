using Avalonia;
using Avalonia.Controls;
using System.ComponentModel;

namespace LogicSimulator.Views.Shapes
{
    public partial class Demul : GateBase, IGate, INotifyPropertyChanged
    {
        public override int TypeId => 4;

        public override int InputCount => 3;
        public override int OutputCount => 4;
        public override UserControl GetSelf() => this;
        protected override IGate GetSelfI => this;

        protected override void Init()
        {
            height = 30 * 4;
            InitializeComponent();
            DataContext = this;
        }


        public override Point[][] PinPoints
        {
            get
            {
                double X = EllipseSize - EllipseStrokeSize / 2;
                double X2 = base_size + width - EllipseStrokeSize / 2;
                double R = BodyRadius.TopLeft;
                double Y_s = R, Y_m = height / 2, Y_e = height - Y_s;
                double min = EllipseSize + BaseFraction * 2;

                double Y = Y_s + (Y_e - Y_s) / 8;
                double Y2 = Y_s + (Y_e - Y_s) / 8 * 3;
                double Y3 = Y_s + (Y_e - Y_s) / 8 * 5;
                double Y4 = Y_s + (Y_e - Y_s) / 8 * 7;
                if (Y2 - Y < min) { Y = Y_m - min / 2 * 3; Y2 = Y_m - min / 2; Y3 = Y_m + min / 2; Y4 = Y_m + min / 2 * 3; }
                double PinWidth = base_size - EllipseSize + PinStrokeSize;
                return new Point[][] {
                new Point[] { new(X, Y), new(X + PinWidth, Y) }, // Первый вход
                new Point[] { new(X, Y3), new(X + PinWidth, Y3) }, // Второй вход
                new Point[] { new(X, Y4), new(X + PinWidth, Y4) }, // Третий вход
                new Point[] { new(X2, Y), new(X2 + PinWidth, Y) }, // Первый выход
                new Point[] { new(X2, Y2), new(X2 + PinWidth, Y2) }, // Второй выход
                new Point[] { new(X2, Y3), new(X2 + PinWidth, Y3) }, // Третий выход
                new Point[] { new(X2, Y4), new(X2 + PinWidth, Y4) }, // Четвёртый выход
            };
            }
        }



        public void InnerLogic(ref bool[] ins, ref bool[] outs)
        {
            bool a = ins[0], b = ins[1], c = ins[2];
            int num = (b ? 1 : 0) + (c ? 2 : 0);
            outs[0] = num == 0 && a;
            outs[1] = num == 1 && a;
            outs[2] = num == 2 && a;
            outs[3] = num == 3 && a;
        }
    }
}
