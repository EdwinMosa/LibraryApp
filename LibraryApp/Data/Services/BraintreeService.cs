using Braintree;
using Microsoft.Extensions.Configuration;

namespace LibraryApp.Data.Services
{
    public class BraintreeService : IBraintreeService
    {
        private readonly IConfiguration _configuration;

        public BraintreeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IBraintreeGateway CreateGateway()
        {
            var environment = _configuration["BraintreeGateway:Environment"];
            var merchantId = _configuration["BraintreeGateway:MerchantId"];
            var publicKey = _configuration["BraintreeGateway:PublicKey"];
            var privateKey = _configuration["BraintreeGateway:PrivateKey"];

            return new BraintreeGateway
            {
                Environment = environment == "SANDBOX" ? Braintree.Environment.SANDBOX : Braintree.Environment.PRODUCTION,
                MerchantId = merchantId,
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }

        public IBraintreeGateway GetGateway()
        {
            return CreateGateway();
        }
    }
}
