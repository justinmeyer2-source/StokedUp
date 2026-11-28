using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace StokedUp.Models
{
    [Table("Categories")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string CategoryType { get; set; } = string.Empty;
    }
}
