using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace BooksApplication.Models
{
    [Table("tblBooks")]
    public class BooksModel
    {
        [Key]
        public int BookModelId { get; set; }

        public string BookName { get; set; }
        public string AuthorName { get; set; }
    }
}
