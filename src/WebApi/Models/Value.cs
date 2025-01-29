using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.WebApi.Models {
    public class Value {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
