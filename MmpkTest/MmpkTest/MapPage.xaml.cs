using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MmpkTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {

        public static readonly BindableProperty MapProperty = BindableProperty.Create("Map", typeof(Map), typeof(MapPage), null);

        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public MapPage()
        {
            InitializeComponent();
            SetMap();
        }

        private async void SetMap()
        {
            try
            {
                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var path = Directory.EnumerateFiles(localFolder, "*.mmpk").FirstOrDefault();
                if (path == null)
                {
                    using (var fos = File.Create(Path.Combine(localFolder, "map.mmpk")))
                    using (var fis = this.GetType().Assembly.GetManifestResourceStream("MmpkTest.Yellowstone.mmpk"))
                    {
                        fis.CopyTo(fos);
                    }
                    path = Directory.EnumerateFiles(localFolder, "*.mmpk").FirstOrDefault();
                }
                if (path != null && File.Exists(path))
                {
                    var mmpk = await MobileMapPackage.OpenAsync(path);
                    TheMap.Map = mmpk.Maps.First();
                    Map = TheMap.Map;
                }
                else
                {
                    var stack = new StackLayout() { Margin = 40 };
                    stack.Children.Add(new Label() { Text = "Mobile Map Package not found. Please place an mmpk (any file name that ends in .mmpk) and restart this app." });
                    this.Content = stack;
                }
            }
            catch (Exception e)
            {
                var stack = new StackLayout() { Margin = 40 };
                stack.Children.Add(new Label() { Text = e.ToString() });
                this.Content = stack;
            }
        }
    }
}