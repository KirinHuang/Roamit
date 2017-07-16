﻿using QuickShare.ViewModels.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QuickShare.HelperClasses
{
    public class HistoryItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ClipboardTextTemplate { get; set; }
        public DataTemplate SingleFileTemplate { get; set; }
        public DataTemplate MultipleFileTemplate { get; set; }
        public DataTemplate WebLinkTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is HistoryClipboardTextItem)
                return ClipboardTextTemplate;
            else if (item is HistorySingleFileItem)
                return SingleFileTemplate;
            else if (item is HistoryMultipleFileItem)
                return MultipleFileTemplate;
            else if (item is HistoryWebLinkItem)
                return WebLinkTemplate;

            Debug.WriteLine($"HistoryItemTemplateSelector couldn't choose template for {item.ToString()}");
            throw new InvalidCastException($"HistoryItemTemplateSelector couldn't choose template for {item.ToString()}");
        }
    }
}