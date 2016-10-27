namespace AnimeginationApi.Models
{
    public class ApiUserAccount
    {
        public string UserName { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public int StateId { get; set; }

        public string ZipCode { get; set; }

        public string CellPhone { get; set; }

        public string HomePhone { get; set; }

        public string Email { get; set; }

        public string CreditCardType { get; set; }

        public string CreditCardNumber { get; set; }

        public string CreditCardExpiration { get; set; }

        public string[] Roles { get; set; }
    }
}