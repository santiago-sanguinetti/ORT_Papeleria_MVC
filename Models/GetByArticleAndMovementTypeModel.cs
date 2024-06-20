using Microsoft.AspNetCore.Mvc.Rendering;

namespace Papeleria_MVC.Models
{
    public class GetByArticleAndMovementTypeModel
    {
        public int ItemId { get; set; }
        public int MovementTypeId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public SelectList Items { get; internal set; }
        public SelectList MovementTypes { get; internal set; }
    }
}
