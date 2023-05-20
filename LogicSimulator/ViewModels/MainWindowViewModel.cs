﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using LogicSimulator.Models;
using LogicSimulator.Views;
using LogicSimulator.Views.Shapes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive;

namespace LogicSimulator.ViewModels {
    public class Log {
        static readonly List<string> logs = new();
        static readonly string path = "../../../Log.txt";
        static bool first = true;

        static readonly bool use_file = false;

        public static MainWindowViewModel? Mwvm { private get; set; }
        public static void Write(string message, bool without_update = true) {
            if (!without_update) {
                foreach (var mess in message.Split(Environment.NewLine)) logs.Add(mess);
                while (logs.Count > 50) logs.RemoveAt(0);

                if (Mwvm != null) Mwvm.Logg = string.Join(Environment.NewLine, logs);
            }

            if (use_file) {
                if (first) File.WriteAllText(path, message + Environment.NewLine);
                else File.AppendAllText(path, message + Environment.NewLine);
                first = false;
            }
        }
    }

    public class MainWindowViewModel: ViewModelBase, INotifyPropertyChanged {
        private string log = "";
        public string Logg { get => log; set {
            if (log == value) return;
            log = value;
            PropertyChanged?.Invoke(this, new(nameof(Logg)));
        } }

        public MainWindowViewModel() {
            Log.Mwvm = this;
            Comm = ReactiveCommand.Create<string, Unit>(n => { FuncComm(n); return new Unit(); });
        }

        private Window? mw;
        private Canvas? canv;
        public void AddWindow(Window window) {
            Canvas canv = window.Find<Canvas>("Canvas");

            mw = window;
            this.canv = canv;
            if (canv == null) return;

            canv.Children.Add(map.Marker);

            Panel? panel = (Panel?) canv.Parent;
            if (panel == null) return;

           
            panel.PointerPressed += (object? sender, PointerPressedEventArgs e) => {
                if (e.Source != null && e.Source is Control @control) map.Press(@control, e.GetCurrentPoint(canv).Position, e.MouseButton);
            };
            panel.PointerMoved += (object? sender, PointerEventArgs e) => {
                if (e.Source != null && e.Source is Control @control) map.Move(@control, e.GetCurrentPoint(canv).Position);
            };
            panel.PointerReleased += (object? sender, PointerReleasedEventArgs e) => {
                if (e.Source != null && e.Source is Control @control) {
                    int mode = map.Release(@control, e.GetCurrentPoint(canv).Position);
                    bool tap = map.tapped;
                    if (tap && mode == 1) {
                        var pos = map.tap_pos;
                        if (canv == null) return;

                        var newy = map.GenSelectedItem();
                        var size = newy.GetSize() / 2;
                        newy.Move(pos - new Point(size.Width, size.Height));
                        canv.Children.Add(newy.GetSelf());
                        map.AddItem(newy);
                    }

                    if (map.new_join != null) {
                        canv.Children.Add(map.new_join);
                        map.new_join = null;
                    }
                }
            };
        }

        public static IGate[] ItemTypes { get => map.item_types; }
        public static int SelectedItem { get => map.SelectedItem; set => map.SelectedItem = value; }



        Border? cur_border;
        TextBlock? old_b_child;
        object? old_b_child_tag;
        string? prev_scheme_name;

        public static string ProjName { get => current_proj == null ? "???" : current_proj.Name; }

        public static List<Scheme> Schemes { get => current_proj == null ? new() : current_proj.schemes; }



        public void DTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            var src = (Control?) e.Source;

            if (src is ContentPresenter cp && cp.Child is Border bord) src = bord;
            if (src is Border bord2 && bord2.Child is TextBlock tb2) src = tb2;

            if (src is not TextBlock tb) return;

            var p = tb.Parent;
            if (p == null || p is not Border b) return;

            if (cur_border != null && old_b_child != null) cur_border.Child = old_b_child;
            cur_border = b;
            old_b_child = tb;
            old_b_child_tag = tb.Tag;
            prev_scheme_name = tb.Text;

            var newy = new TextBox { Text = tb.Text };
            
            b.Child = newy;
            
            newy.KeyUp += (object? sender, KeyEventArgs e) => {
                if (e.Key != Key.Return) return;

                if (newy.Text != prev_scheme_name) {
                    if ((string?) tb.Tag == "p_name") current_proj?.ChangeName(newy.Text);
                    else if (old_b_child_tag is Scheme scheme) scheme.ChangeName(newy.Text);
                }

                b.Child = tb;
                cur_border = null; old_b_child = null;
            };
        }

#pragma warning disable CS0108
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0108
        public void Update() {
            Log.Write("Текущий проект:" + Environment.NewLine + current_proj);

            if (current_scheme == null || canv == null) throw new Exception();
            map.ImportScheme(current_scheme, canv);

            PropertyChanged?.Invoke(this, new(nameof(ProjName)));
            PropertyChanged?.Invoke(this, new(nameof(Schemes)));
        }



        public void FuncComm(string Comm) {
            Log.Write("Comm: " + Comm);
            switch (Comm) {
            case "Create":
                break;
            case "Open":
                new LauncherWindow().Show();
                mw?.Hide();
                break;
            case "Save":
                if (current_scheme != null) map.Export(current_scheme);
                break;
            case "Exit":
                mw?.Close();
                break;
            }
        }

        public ReactiveCommand<string, Unit> Comm { get; }
    }
}