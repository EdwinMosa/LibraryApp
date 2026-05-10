using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApp.Data.Services;
using LibraryApp.Data.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Braintree;
using System.Globalization;

namespace LibraryApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IBraintreeService _braintreeService;

        public CheckoutController(IBookService bookService, IBraintreeService braintreeService)
        {
            _bookService = bookService;
            _braintreeService = braintreeService;
        }

        public IActionResult Purchase(Guid id)
        {

            var book = _bookService.GetById(id);
            if (book == null) return NotFound();

            var gateway = _braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;

            var data = new BookPurchaseVM
            {
                Id = book.Id,
                Description = book.Description,
                Author = book.Author,
                Thumbnail = book.Thumbnail,
                Title = book.Title,
                Price = book.Price,
                Nonce = ""
            };

            return View(data);
        }

        public IActionResult Create(BookPurchaseVM model)
        {
            var gateway = _braintreeService.GetGateway();
            var book = _bookService.GetById(model.Id);

            var request = new Braintree.TransactionRequest
            {
                Amount = decimal.Parse(book.Price, CultureInfo.InvariantCulture),
                PaymentMethodNonce = model.Nonce,
                Options = new Braintree.TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };
            var result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                return View("Success");
            }
            else
            {
                return View("Failure");
            }
            
        }
    }
}