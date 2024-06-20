using System.ComponentModel.DataAnnotations;

namespace Papeleria_MVC.Models
{
    public class ItemsViewModel
    {
        /// <summary>
        /// Identificador del artículo.
        /// </summary>
        [Required(ErrorMessage = "El identificador del artículo es obligatorio.")]
        public int Id { get; set; }
        /// <summary>
        /// Nombre del artículo.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Descripción del artículo.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Código del artículo.
        /// </summary>
        public long ItemCode { get; set; }
    }
}
