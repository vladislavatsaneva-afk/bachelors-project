using MetricConverter.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MetricConverter.Data
{ 
    public class AppUser : IdentityUser
    {
        public ICollection<ConversionHistoryEntry> ConversionHistoryEntries { get; set; }
    }
}

