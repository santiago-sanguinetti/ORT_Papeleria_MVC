using System.ComponentModel.DataAnnotations;

namespace Papeleria_MVC.Models
{
    public class StockMovementViewModel
    {
        /// <summary>
        /// Identificador del movimiento de stock.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador del artículo.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Fecha del movimiento (fecha del sistema).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Cantidad de artículos.
        /// </summary>
        [Required(ErrorMessage = "La cantidad de artículos es obligatoria.")]
        public int ItemQuantity { get; set; }

        /// <summary>
        /// Identificador del tipo de movimiento.
        /// </summary>
        public int MovementTypeId { get; set; }

        /// <summary>
        /// Identificador del usuario que realiza el movimiento.
        /// </summary>
        public int ResponsibleUserId { get; set; }

        
    }
}
