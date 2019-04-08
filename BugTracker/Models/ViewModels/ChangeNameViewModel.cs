using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class ChangeNameViewModel
    {
        public string NameToBeChanged { get; set; }
        [Required(ErrorMessage = "Enter a name")]
        public string ChangedName { get; set; }
    }
}