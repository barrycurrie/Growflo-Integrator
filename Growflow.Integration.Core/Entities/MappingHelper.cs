﻿using Growflo.Integration.Core.Sage.Entities;
using Growflo.Integration.Core.Entities.Web;
using System;
using System.Linq;

namespace Growflo.Integration.Core.Entities
{
    public class MappingHelper
    {
        public SageBatchInvoice CreateSageBatchInvoice(OnlineInvoice onlineOrder)
        {
            SageBatchInvoice sageBatchInvoice = new SageBatchInvoice()
            {
                InvoiceReference = onlineOrder.InvoiceNumber,
                CustomerAccountNumber = onlineOrder.AccountIdentifier,
                Date = onlineOrder.Dates == null ? DateTime.Today : onlineOrder.Dates.DeliveryDate.GetValueOrDefault(DateTime.Today),
                IsCredit = onlineOrder.IsCredit,
                Currency = onlineOrder.CurrencyCode,
                Details= onlineOrder.PurchaseOrderNo,
            };



            var splits = onlineOrder.Items.data.Select(
                x => new SageBatchInvoice.Split()
                {
                    NominalCode = x.NominalCode,
                    VatCode = x.VatCode,
                    VatAmount = (double)x.VatTotal,
                    NetAmount = (double)x.SubTotal,
                    Details = sageBatchInvoice.Details,
                    Date = sageBatchInvoice.Date
                });

            sageBatchInvoice.Splits.AddRange(splits);

            return sageBatchInvoice;
        }

        public SageBatchInvoice CreateSageBatchInvoice(OnlineCredit onlineOrder)
        {
            SageBatchInvoice sageBatchInvoice = new SageBatchInvoice()
            {
                InvoiceReference = onlineOrder.CreditNumber,
                CustomerAccountNumber = onlineOrder.AccountIdentifier,
                Date = DateTime.Today,
                IsCredit = onlineOrder.IsCredit,
                Currency = onlineOrder.CurrencyCode
            };



            var splits = onlineOrder.Items.data.Select(
                x => new SageBatchInvoice.Split()
                {
                    NominalCode = x.NominalCode,
                    VatCode = x.VatCode,
                    VatAmount = (double)x.VatTotal,
                    NetAmount = (double)x.SubTotal,
                    Details = x.Name,
                    Date = sageBatchInvoice.Date
                });

            sageBatchInvoice.Splits.AddRange(splits);

            return sageBatchInvoice;
        }


        public SageCustomer CreateSageCustomer(OnlineCustomer onlineCustomer)
        {
            var sageCustomer = new SageCustomer()
            {
                Name = onlineCustomer.Name,
                AccountNumber = onlineCustomer.Identifier,
                Email = onlineCustomer.EmailAddress,
                VatNumber = onlineCustomer.VatNumber,
            };

            if (onlineCustomer.InvoiceAddress != null)
            {
                sageCustomer.InvoiceAddressLine1 = onlineCustomer.InvoiceAddress.Line1;
                sageCustomer.InvoiceAddressLine2 = onlineCustomer.InvoiceAddress.Line2;
                sageCustomer.InvoiceAddressLine3 = onlineCustomer.InvoiceAddress.Town;
                sageCustomer.InvoiceAddressLine4 = onlineCustomer.InvoiceAddress.County;
                sageCustomer.InvoiceAddressLine5 = onlineCustomer.InvoiceAddress.Postcode;
            }

            return sageCustomer;
        }
    }
}
