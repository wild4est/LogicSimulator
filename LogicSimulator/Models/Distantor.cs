using Avalonia;
using LogicSimulator.Views.Shapes;

namespace LogicSimulator.Models
{
    public class Distantor
    {
        public readonly int num;
        public IGate parent;
        public readonly string tag;

        readonly Visual? _ref_point;

        public Distantor(IGate gate, int n, Visual? r_p, string tag)
        {
            this.parent = gate;
            num = n; // Например, в AND_2-gate: 0 и 1 - входы, 2 - выход
            _ref_point = r_p;
            this.tag = tag;
        }

        public Point GetPos() => parent.GetPinPos(num, _ref_point);
    }
}
