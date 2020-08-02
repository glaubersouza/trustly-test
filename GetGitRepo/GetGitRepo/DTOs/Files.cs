using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetGitRepo.Models
{
    public class Files
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public decimal FileSize { get; set; }
        public int FileLineCode { get; set; }
    }
}
