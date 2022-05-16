// To learn more about WinUI3, see: https://docs.microsoft.com/windows/apps/winui/winui3/.
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

namespace GPS.SplitBrowser
{
    public partial class App : CancelableApplication
    {
        private const string COULD_NOT_ENQUEUE_ACTION = "Could not enqueue action.";
        private IHost? _appHost;
        private static App? _instance;
        private static Window? _mainWindow;

        public IHost AppHost
        {
            get => _appHost ?? throw new ArgumentNullException(nameof(_appHost));
            set
            {
                if (value is null || (_appHost?.Equals(value) ?? false))
                {
                    return;
                }

                _appHost = value;
            }
        }

        public T GetRequiredService<T>()
            where T : class
        {
            return AppHost.Services.GetRequiredService<T>() ?? throw new NullReferenceException(typeof(T).Name);
        }

        public T? GetService<T>()
            where T : class
        {
            return AppHost.Services.GetService<T>() ?? default;
        }

        public Window? MainWindow { get => _mainWindow; set => _mainWindow = value; }
        public static App? Instance { get => _instance; internal set => _instance = value; }

        public App()
        {
            try
            {
                InitializeComponent();
                UnhandledException += App_UnhandledException;
            }
            catch(Exception ex)
            {

            }
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // For more details, see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs.
        }

        public void Dispatch(Action? a)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            DispatcherQueue? dqueue = DispatcherQueue.GetForCurrentThread();
            if (dqueue is null)
            {
                throw new NullReferenceException(nameof(dqueue));
            }

            if (dqueue!.HasThreadAccess)
            {
                a();
            }
            else
            {
                if (dqueue.TryEnqueue(DispatcherQueuePriority.Normal,
                    new DispatcherQueueHandler(a)))
                {
                    return;
                }

                throw new ApplicationException(COULD_NOT_ENQUEUE_ACTION);
            }
        }

        public TReturn? Dispatch<TReturn>(
            Func<TReturn>? a)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            DispatcherQueue? dqueue = DispatcherQueue.GetForCurrentThread();
            if (dqueue is null)
            {
                throw new NullReferenceException(nameof(dqueue));
            }

            if (dqueue!.HasThreadAccess)
            {
                return a();
            }
            else
            {
                ManualResetEventSlim mres = new();
                TReturn? result = default;

                DispatcherQueueHandler? del = new(() =>
                {
                    result = a();
                    mres.Set();
                });

                if (dqueue.TryEnqueue(DispatcherQueuePriority.Normal, del))
                {
                    CancellationToken token = _instance?.Token ?? CancellationToken.None;
                    mres.Wait(token);

                    return result;
                }

                throw new ApplicationException(COULD_NOT_ENQUEUE_ACTION);
            }
        }

        public Task DispatchAsync(
            Func<Task>? a,
            CancellationToken token = default)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            DispatcherQueue? dqueue = DispatcherQueue.GetForCurrentThread();
            if (dqueue is null)
            {
                throw new NullReferenceException(nameof(dqueue));
            }

            if (dqueue!.HasThreadAccess)
            {
                return a();
            }
            else
            {
                ManualResetEventSlim mres = new();
                Task? task = null;

                DispatcherQueueHandler? del = new(() =>
                {
                    task = Task.Run(a, token);
                    mres.Set();
                });

                if (dqueue.TryEnqueue(DispatcherQueuePriority.Normal, del))
                {
                    mres.Wait(token);

                    return task!;
                }

                return Task.FromException(new ApplicationException(COULD_NOT_ENQUEUE_ACTION));
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            _mainWindow = AppHost.Services.GetRequiredService<Window>();

            await DispatchAsync(InnerAsync, _instance?.Token ?? CancellationToken.None);

            Task InnerAsync()
            {
                IActivationService? activationService = GetRequiredService<IActivationService>();
                return activationService?.ActivateAsync(args) ?? Task.FromException(new NullReferenceException(nameof(activationService)));
            }
        }
    }
}
