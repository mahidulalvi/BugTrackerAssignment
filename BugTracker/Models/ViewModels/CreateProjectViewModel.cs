using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models.ViewModels
{
    public class CreateEditProjectViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Name required")]
        public string ProjectName { get; set; }
        [Required(ErrorMessage = "Details required")]
        public string ProjectDetails { get; set; }
    }
}