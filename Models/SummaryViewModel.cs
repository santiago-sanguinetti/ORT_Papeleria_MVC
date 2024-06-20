namespace Papeleria_MVC.Models
{
    public class SummaryViewModel
    {
        public int year { get; set; }
        public List<SummaryItemModel> movements { get; set; }
        public int yearTotal { get; set; }
    }
}
