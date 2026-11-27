using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StokedUp.Models;
    public class StokpileItem
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public string Quantity { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsNotesExpanded { get; set; }
    }