﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Service.Activities;
using Restbucks.Service.Domain;
using Restbucks.Service.Infrastructure;
using Restbucks.Service.Mappers;
using Restbucks.Service.Representations;
using Restbucks.Service.Resources;

namespace Restbucks.Service.Tests
{
    [TestFixture]
    public class ReceiptResourceTests
    {
        private ReceiptResource _sut;
        private InMemoryOrderRepository _repository;

        [Test]
        public void Get_should_return_receipt_with_amount_paid()
        {
            var order = CreateOrder();
            order.Pay(new PaymentInformation(1, "", "", 12, 12));
            var id = _repository.Store(order);

            var responseMessage = new HttpResponseMessage();

            var responseBody = _sut.Get(id.ToString(),
                     new HttpRequestMessage(HttpMethod.Get, "http://restbucks.net/receipt/" + id),
                     responseMessage);

            Assert.AreEqual(HttpStatusCode.OK, responseMessage.StatusCode);
            Assert.AreEqual(1m, responseBody.AmountPaid);
        }

        private static Order CreateOrder()
        {
            return new Order(Location.InStore, new[] { new Item(Drink.Espresso, Size.Medium, Milk.Semi) });
        }

        [SetUp]
        public void Initialize()
        {
            _repository = new InMemoryOrderRepository();
            var mapper = new ReceiptRepresentationMapper();
            _sut = new ReceiptResource(new ReadReceiptActivity(_repository, mapper));
        }
    }
}
