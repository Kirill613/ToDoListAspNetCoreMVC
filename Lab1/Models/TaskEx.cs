using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1.Models
{
    public class TaskEx
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImageName { get; set; }
        public DateTime Date { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
