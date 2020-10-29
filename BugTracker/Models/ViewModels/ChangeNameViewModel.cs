using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models.ViewModels
{
    public class ChangeNameViewModel
    {
        public string NameToBeChanged { get; set; }
        [Required(ErrorMessage = "Enter a name")]
        public string ChangedName { get; set; }
    }
}