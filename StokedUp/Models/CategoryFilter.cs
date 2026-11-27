using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StokedUp.Models;

    public class CategoryFilter
    {
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }