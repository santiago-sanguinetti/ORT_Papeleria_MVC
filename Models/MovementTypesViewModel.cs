namespace Papeleria_MVC.Models
{
    public class MovementTypesViewModel
    {
        /// <summary>
        /// Identificador del tipo de movimiento.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del tipo de movimiento.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Si el tipo de movimiento es de entrada.
        /// </summary>
        public bool IsStockAugment { get; set; }
    }
}
