﻿using LogicSimulator.ViewModels;
using LogicSimulator.Views.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicSimulator.Models
{
    public class Meta
    {
        public IGate? item;
        public int[] ins;
        public int[] outs;
        public bool[] i_buf;
        public bool[] o_buf;

        public Meta(IGate item, int out_id)
        {
            this.item = item;
            ins = Enumerable.Repeat(0, item.InputCount).ToArray();
            outs = Enumerable.Range(out_id, item.OutputCount).ToArray();
            i_buf = Enumerable.Repeat(false, item.InputCount).ToArray();
            o_buf = Enumerable.Repeat(false, item.OutputCount).ToArray();
        }

        public void Print()
        {
            Log.Write("Элемент: " + item + " | Ins: " + Utils.Obj2json(ins) + " | Outs: " + Utils.Obj2json(outs));
        }
    }


    public class Simulator
    {
        public bool lock_sim = false;
        public Simulator()
        {
            var task = Task.Run(async () =>
            {
                for (; ; )
                {
                    await Task.Delay(1000 / 60);
                    if (lock_sim) continue;
                    try { Tick(); }
                    catch (Exception e) { Log.Write("Logical crush: " + e); continue; }
                }
            });
        }



        List<bool> outs = new() { false };
        List<bool> outs2 = new() { false };
        readonly List<Meta> items = new();
        readonly Dictionary<IGate, Meta> ids = new();

        public void AddItem(IGate item)
        {
            lock_sim = true;

            int out_id = outs.Count;
            for (int i = 0; i < item.OutputCount; i++)
            {
                outs.Add(false);
                outs2.Add(false);
            }

            Meta meta = new(item, out_id);
            items.Add(meta);
            ids.Add(item, meta);

            lock_sim = false;
        }

        public void RemoveItem(IGate item)
        {
            lock_sim = true;

            Meta meta = ids[item];
            meta.item = null;
            foreach (var i in Enumerable.Range(0, meta.outs.Length)) meta.outs[i] = 0;

            lock_sim = false;
        }

        private void Tick()
        {
            foreach (var meta in items)
            {
                var item = meta.item;
                if (item == null) continue;

                item.LogicUpdate(ids, meta);

                int[] i_n = meta.ins, o_n = meta.outs;
                bool[] ib = meta.i_buf, ob = meta.o_buf;

                for (int i = 0; i < ib.Length; i++) ib[i] = outs[i_n[i]];
                item.InnerLogic(ref ib, ref ob);
                for (int i = 0; i < ob.Length; i++)
                {
                    bool res = ob[i];
                    outs2[o_n[i]] = res;
                    item.SetJoinColor(i, res);
                }
            }

            (outs2, outs) = (outs, outs2);
        }

        public bool[] Export() => outs.ToArray();
        public void Import(bool[] state)
        {
            if (state.Length == 0) state = new bool[] { false };
            outs = state.ToList();
            outs2 = Enumerable.Repeat(false, state.Length).ToList();
        }
    }
}
