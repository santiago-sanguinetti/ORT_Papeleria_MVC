namespace Papeleria_MVC.Models
{
    public class ItemByMovementTypeModel
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public int itemId { get; set; }
        public int movementTypeId { get; set; }
        public int itemQuantity { get; set; }
        public int responsibleUserId { get; set; }
    }
}
