﻿using System.Collections.ObjectModel;
using Fluent;

namespace RibbonProject.Models
{
    public class RibbonPageConfiguration
    {
        public Collection<RibbonGroupBox> HomeGroups { get; set; } = new Collection<RibbonGroupBox>();

        public Collection<RibbonTabItem> Tabs { get; set; } = new Collection<RibbonTabItem>();
    }
}
