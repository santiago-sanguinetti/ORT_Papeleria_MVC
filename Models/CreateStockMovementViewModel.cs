using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Papeleria_MVC.Models
{
    public class CreateStockMovementViewModel
    {
        /// <summary>
        /// Lista de artículos.
        /// </summary>
        public SelectList Items { get; set; }
        /// <summary>
        /// Lista de tipos de movimiento.
        /// </summary>
        public SelectList MovementTypes { get; set; }

        /// <summary>
        /// Identificador el artículo seleccionado.
        /// </summary>
        [Required(ErrorMessage = "El artículo es obligatorio.")]
        public int ItemId { get; set; }
        /// <summary>
        /// Identificador del tipo de movimiento seleccionado.
        /// </summary>
        [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
        public int MovementTypeId { get; set; }

        /// <summary>
        /// Cantidad de artículos.
        /// </summary>
        [Required(ErrorMessage = "La cantidad de artículos es obligatoria.")]
        public int ItemQuantity { get; set; }

        [Required(ErrorMessage = "El usuario responsable es obligatorio.")]
        public int ResponsibleUserId { get; set; }
    }
}
