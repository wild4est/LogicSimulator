using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using LogicSimulator.Models;
using System.Collections.Generic;

namespace LogicSimulator.Views.Shapes {
    public interface IGate {
        public int InputCount { get; }
        public int OutputCount { get; }
        public UserControl GetSelf();

        public Point GetPos();
        public Size GetSize();
        public Size GetBodySize();
        public void Move(Point pos);
        public void Resize(Size size, bool global);

        public Distantor GetPin(Ellipse finded, Visual? ref_point);
        public Point GetPinPos(int n, Visual? ref_point);

        public void AddJoin(JoinedItems join);
        public void RemoveJoin(JoinedItems join);
        public void ClearJoins();
        public void SetJoinColor(int o_num, bool value);

        public void InnerLogic(ref bool[] ins, ref bool[] outs);
        public void LogicUpdate(Dictionary<IGate, Meta> ids, Meta me);

        public int TypeId { get; }
        public object Export();
        public List<object[]> ExportJoins(Dictionary<IGate, int> to_num);
        public void Import(Dictionary<string, object> dict);
    }
}
