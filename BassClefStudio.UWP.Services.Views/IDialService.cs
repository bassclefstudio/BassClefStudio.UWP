using Autofac;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.DialProtocol;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Windows.UI.Xaml;

namespace BassClefStudio.UWP.Services.Views
{
    /// <summary>
    /// Represents a service used for controlling and responding to wheel input, such as from a Surface Dial.
    /// </summary>
    public interface IDialService
    {
        /// <summary>
        /// When on-screen (see <see cref="MenuActive"/>), the bounds of the dial, relative to the application window.
        /// </summary>
        Rect DialBounds { get; }

        /// <summary>
        /// When on-screen (see <see cref="MenuActive"/>), the position of the dial, relative to the application window.
        /// </summary>
        Point DialLocation { get; }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the dial has been placed on the screen while using an app-specific tool.
        /// </summary>
        bool MenuActive { get; }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether haptic feedback should be enabled on the wheel input device.
        /// </summary>
        bool HapticsEnabled { get; set; }

        /// <summary>
        /// A <see cref="double"/> number of degrees the dial should be turned to register a change.
        /// </summary>
        double DegreeChange { get; set; }

        /// <summary>
        /// Registers a tool on the dial that, when activated, will pass input to the <see cref="IDialService"/>.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        /// <param name="iconFile">An image <see cref="IStorageFile"/> pointing to the icon for the tool.</param>
        /// <param name="reRegister">A <see cref="bool"/> indicating whether the tool should be unregisrered and re-registered if it is already present. Defaults to false (ignored if present).</param>
        void RegisterTool(string name, IStorageFile iconFile, bool reRegister = false);

        /// <summary>
        /// An event fired whenever the rotation of the dial device changes while the app's tool is selected.
        /// </summary>
        event EventHandler<RotationChangedEventArgs> RotationChanged;

        /// <summary>
        /// An event fired whenever the dial device button is clicked.
        /// </summary>
        event EventHandler ButtonClicked;
    }

    public class RotationChangedEventArgs : EventArgs
    {
        public double RotationChangedAmount { get; }
        public RotationChangedEventArgs(double amount) => RotationChangedAmount = amount;
    }

    /// <summary>
    /// An <see cref="IDialService"/> using the UWP Surface Dial APIs.
    /// </summary>
    public class SurfaceDialService : Observable, IDialService
    {
        private Rect dialBounds;
        /// <inheritdoc/>
        public Rect DialBounds { get => dialBounds; private set => Set(ref dialBounds, value); }

        private Point dialLocation;
        /// <inheritdoc/>
        public Point DialLocation { get => dialLocation; private set => Set(ref dialLocation, value); }

        private bool menuActive;
        /// <inheritdoc/>
        public bool MenuActive { get => menuActive; private set => Set(ref menuActive, value); }

        private bool hapticsEnabled;
        /// <inheritdoc/>
        public bool HapticsEnabled { get => hapticsEnabled; set { Set(ref hapticsEnabled, value); HapticsChanged(); } }

        private void HapticsChanged()
        {
            Controller.UseAutomaticHapticFeedback = HapticsEnabled;
        }

        private double degreeChange;
        /// <inheritdoc/>
        public double DegreeChange { get => degreeChange; set { Set(ref degreeChange, value); DegreeChanged(); } }

        private void DegreeChanged()
        {
            Controller.RotationResolutionInDegrees = DegreeChange;
        }

        /// <inheritdoc/>
        public event EventHandler<RotationChangedEventArgs> RotationChanged;
        /// <inheritdoc/>
        public event EventHandler ButtonClicked;

        /// <summary>
        /// The associated <see cref="RadialController"/> for the Surface Dial.
        /// </summary>
        public RadialController Controller { get; }

        public SurfaceDialService()
        {
            if (IsSupported())
            {
                // Create a reference to the RadialController.
                Controller = RadialController.CreateForCurrentView();
                Controller.ButtonClicked += Controller_ButtonClicked;
                Controller.RotationChanged += Controller_RotationChanged;
                Controller.ScreenContactStarted += Controller_ScreenContactStarted;
                Controller.ScreenContactEnded += Controller_ScreenContactEnded;
            }
            else
            {
                throw new NotSupportedException("RadialController and the native Surface Dial APIs are not supported on this device.");
            }
        }

        public static bool IsSupported() => RadialController.IsSupported();

        /// <inheritdoc/>
        public void RegisterTool(string name, IStorageFile iconFile, bool reRegister = false)
        {
            var existingItem = Controller.Menu.Items.FirstOrDefault(i => i.Tag.ToString() == name);
            if (existingItem != null)
            {
                if(reRegister)
                {
                    Controller.Menu.Items.Remove(existingItem);
                }
                else
                {
                    return;
                }
            }

            RandomAccessStreamReference icon = RandomAccessStreamReference.CreateFromFile(iconFile);
            RadialControllerMenuItem myItem = RadialControllerMenuItem.CreateFromIcon(name, icon);
            myItem.Tag = name;
            // Add the custom tool to the RadialController menu.
            Controller.Menu.Items.Add(myItem);
        }

        private void Controller_ScreenContactStarted(RadialController sender, RadialControllerScreenContactStartedEventArgs args)
        {
            MenuActive = true;
            DialBounds = args.Contact.Bounds;
            DialLocation = args.Contact.Position;
        }

        private void Controller_ScreenContactEnded(RadialController sender, object args)
        {
            MenuActive = false;
        }

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
            => RotationChanged?.Invoke(this, new RotationChangedEventArgs(args.RotationDeltaInDegrees));

        private void Controller_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
            => ButtonClicked?.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Represents extension methods for adding <see cref="IDialService"/>s to an <see cref="Autofac"/> container.
    /// </summary>
    public static class DialExtensions
    {
        /// <summary>
        /// Adds surface dial / wheel input support through the <see cref="IDialService"/> to the <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The relevant DI <see cref="ContainerBuilder"/>.</param>
        public static void AddSurfaceDial(this ContainerBuilder builder)
        {
            if (SurfaceDialService.IsSupported())
            {
                builder.RegisterType<SurfaceDialService>().SingleInstance().AsImplementedInterfaces();
            }
        }
    }
}
