using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MmpkTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LayersPage : ContentPage
    {
        public static readonly BindableProperty MapProperty = BindableProperty.Create("Map", typeof(Map), typeof(MapPage), null, propertyChanged: MapChanged);

        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public LayersPage()
        {
            InitializeComponent();
        }

        private static void MapChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var page = (LayersPage)bindable;
            page.UpdateMap();
        }

        private void UpdateMap()
        {
            Collection.ItemsSource = Map.AllLayers.SelectMany(x => CreateLayerModels(x, 0)).ToArray();
            this.ForceLayout();
        }

        protected override void OnAppearing()
        {
            this.ForceLayout();
        }

        private static IEnumerable<LayerModel> CreateLayerModels(ILayerContent layer, int depth)
        {
            yield return new LayerModel(layer, depth);
            depth++;
            foreach (var sublayerContent in layer.SublayerContents)
            {
                foreach (var flattened in CreateLayerModels(sublayerContent, depth))
                {
                    yield return flattened;
                }
            }
        }

        private void Collection_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var context = (LayerModel)Collection.SelectedItem;
            if (context != null)
            {
                var layer = context.Layer;
                layer.IsVisible = !layer.IsVisible;
                Collection.SelectedItem = null;
            }
        }
    }

    public class LayerModel
    {
        public int Ancestors { get; set; }

        public ILayerContent Layer { get; set; }

        public LayerModel(ILayerContent layer, int ancestors)
        {
            this.Ancestors = ancestors;
            this.Layer = layer;
        }
    }
}