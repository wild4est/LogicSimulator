using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System.Collections.Generic;

namespace LogicSimulator.Models
{
    public class JoinedItems //соединение между логическими элементами
    {
        public static readonly Dictionary<Line, JoinedItems> ArrowToJoin = new();

        public JoinedItems(Distantor a, Distantor b)
        {
            A = a;
            B = b;
            Update();

            a.parent.AddJoin(this);
            b.parent.AddJoin(this);
            ArrowToJoin[line] = this;
        }
        public Distantor A { get; set; }
        public Distantor B { get; set; }
        public Line line = new() { Tag = "Join", ZIndex = 2, Stroke = Brushes.DarkGray, StrokeThickness = 3 };

        public void Update()
        {
            line.StartPoint = A.GetPos();
            line.EndPoint = B.GetPos();
        }
        public void Delete()
        {
            ArrowToJoin.Remove(line);
            line.Remove();
            A.parent.RemoveJoin(this);
            B.parent.RemoveJoin(this);
        }
    }
}
