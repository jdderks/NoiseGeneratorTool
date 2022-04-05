using MVVM.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MVVM.DataTemplateSelectors
{
    class ItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is ItemVM)
            {
                if (item is SubItem1VM)
                {
                    return element.FindResource("subItem1DataTemplate") as DataTemplate;
                }
                else if (item is SubItem2VM)
                {
                    return element.FindResource("subItem2DataTemplate") as DataTemplate;
                }
                else //if (item is ItemVM)
                {
                    return element.FindResource("itemDataTemplate") as DataTemplate;
                }
            }

            return null;
            //return base.SelectTemplate(item, container);
        }
    }
}
