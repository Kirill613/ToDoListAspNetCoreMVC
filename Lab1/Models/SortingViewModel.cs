using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1.Models
{
    public class SortingViewModel
    {
        public IEnumerable<TaskEx> Tasks { get; set; }
        public SelectList SortOrder { get; set; }
        public Guid Id { get; set; }
    }
}
