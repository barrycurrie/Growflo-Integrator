using Growflo.Integration.Core.Sage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Validation
{
    public class SageBatchInvoiceValidator : IValidator<SageBatchInvoice>
    {
        public List<BrokenRule> Validate(SageBatchInvoice entity)
        {
            var brokenRules = new List<BrokenRule>();

            if (string.IsNullOrWhiteSpace(entity.CustomerAccountNumber))
            {
                brokenRules.Add(new BrokenRule(nameof(entity.CustomerAccountNumber), "Missing customer account number"));
            }

            if (entity.Splits == null || entity.Splits.Count == 0)
            {
                brokenRules.Add(new BrokenRule(nameof(entity.Splits), "An invoice must have one or more splits"));
            }
            else
            {
                int count = 0;

                foreach (var split in entity.Splits)
                {
                    count++;

                    if (string.IsNullOrWhiteSpace(split.NominalCode))
                    {
                        brokenRules.Add(new BrokenRule(nameof(split.NominalCode), $"Item {count}: Nominal code is a required field."));
                    }

                    if (string.IsNullOrWhiteSpace(split.VatCode))
                    {
                        brokenRules.Add(new BrokenRule(nameof(split.VatCode), $"Item {count}: Vat code is a required field."));
                    }

                    if ((split.NetAmount) == 0)
                    {
                        brokenRules.Add(new BrokenRule(nameof(split.NetAmount), $"Item {count}: Net amount must be greater than zero."));
                    }
                }
            }
            return brokenRules;
        }
    }
}
