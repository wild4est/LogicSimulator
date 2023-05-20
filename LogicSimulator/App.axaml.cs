using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LogicSimulator.Views;
using System.IO;

namespace LogicSimulator {
    public partial class App: Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new LauncherWindow();

            base.OnFrameworkInitializationCompleted();
        }
    }
}