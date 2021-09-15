using System.ComponentModel.DataAnnotations;

namespace JWT_Controllers.Models {
    public class Store {

        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}