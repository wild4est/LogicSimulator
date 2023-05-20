﻿using Avalonia.Controls.Presenters;
using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;
using LogicSimulator.Views;
using LogicSimulator.Models;

namespace LogicSimulator.ViewModels
{
    public class LauncherWindowViewModel : ViewModelBase
    {
        Window? me;
        private static readonly MainWindow _main_window = new();

        public LauncherWindowViewModel()
        {
            Create = ReactiveCommand.Create<Unit, Unit>(_ => { FuncCreate(); return new Unit(); });
            Exit = ReactiveCommand.Create<Unit, Unit>(_ => { FuncExit(); return new Unit(); });
        }
        public void AddWindow(Window window) => me = window;

        void FuncCreate()
        {
            var newy = map.filer.CreateProject();
            current_proj = newy;
            current_scheme = current_proj.GetFirstCheme();
            _main_window.Show();
            _main_window.Update();
            me?.Close();
        }
        void FuncExit()
        {
            me?.Close();
        }

        public ReactiveCommand<Unit, Unit> Create { get; }
        public ReactiveCommand<Unit, Unit> Exit { get; }


        public static Project[] ProjectList { get => map.filer.GetSortedProjects(); }

        public void DTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Control? src = (Control?)e.Source;

            if (src is ContentPresenter cp && cp.Child is Border bord) src = bord;
            if (src is Border border && border.Child is TextBlock tb) src = tb;

            if (src is not TextBlock textBlock || textBlock.Tag is not Project proj) return;

            current_proj = proj;
            current_scheme = current_proj.GetFirstCheme();
            _main_window.Show();
            _main_window.Update();
            me?.Close();
        }
    }
}
