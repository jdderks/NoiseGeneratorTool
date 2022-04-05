using MVVM.MainView;
using MVVM.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MVVM.DataTemplateSelectors
{
    class LayerDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is LayerVM)
            {
                if (item is SmoothNoiseLayerVM)
                {
                    return element.FindResource("smoothLayerDataTemplate") as DataTemplate;
                }
                else //(item is LayerVM)
                {
                    return element.FindResource("layerDataTemplate") as DataTemplate;
                }
            }
            return null;
            //return base.SelectTemplate(item, container);
        }

    }
}
