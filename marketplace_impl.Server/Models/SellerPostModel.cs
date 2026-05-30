using marketplace_impl.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace marketplace_impl.Server.Models
{
    public class SellerPostModel
    {
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public bool ContractConditionsAccepted { get; set; }

        public Seller ToEntity(int userId, string contractNumber)
        {
            return new Seller
            {
                UserId = userId,
                Email = Email,
                ContractNumber = contractNumber,
            };
        }
    }
}
