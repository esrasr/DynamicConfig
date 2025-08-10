using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfig.Models
{
    public class Config
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Value { get; set; }
        public bool IsActive { get; set; } = true;
        public required string ApplicationName { get; set; }

    }

    public class ConfigDto
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Value { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
