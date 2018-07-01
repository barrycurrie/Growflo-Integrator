using Growflo.Integration.Core.Entities.Web;
using Growflo.Integration.Core.Sage.Entities;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;

namespace Growflo.Integration.Core.Controllers
{
    public class WebController : IWebController
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private RestClient _client;

        public WebController(string baseUri, string authorizationKey)
        {
            try
            {
                _client = new RestClient();
                _client.BaseUrl = new Uri(baseUri);
                _client.AddDefaultHeader("Authorization", authorizationKey);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public WebController() : 
            this(ConfigurationManager.AppSettings["BaseUri"], ConfigurationManager.AppSettings["AuthorizationKey"])
        {
           
        }

        private IList<T> ExecuteRestRequest<T>(string resource, bool multipleResults = true)
        {
            List<T> items = new List<T>();

            try
            {
                var request = new RestRequest();
                request.Resource = resource;
                IRestResponse response = _client.Execute(request);

                if (response.ErrorException != null)
                    throw response.ErrorException;

                if (multipleResults)
                {
                    RootList<T> root = JsonConvert.DeserializeObject<RootList<T>>(response.Content);
                    items.AddRange(root.data);
                }
                else
                {
                    Root<T> root = JsonConvert.DeserializeObject<Root<T>>(response.Content);
                    items.Add(root.data);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "An error occured executing a rest request");
                throw;
            }


            return items;
        }

        private void ExecuteRequest(Method method, string resource, object item)
        {
            try
            {
                string json = JsonConvert.SerializeObject(item);

                var request = new RestRequest(resource, method);
                request.RequestFormat = DataFormat.Json;
                request.Resource = resource;
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                IRestResponse response = _client.Execute(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(
                        $"Status Code:{response.StatusCode}\n\r" +
                        $"Status Description:{response.StatusDescription}\n\r" +
                        $"Response:{response.Content}\n\r");
                }

                if (response.ErrorException != null)
                    throw response.ErrorException;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occured executing a {method} request: {ex.Message}");
                throw;
            }
        }

        public IList<OnlineCustomer> DownloadCustomers()
        {
            List<OnlineCustomer> customers = new List<OnlineCustomer>();
            customers.AddRange(ExecuteRestRequest<OnlineCustomer>("customer"));

            return customers;
        }

        public IList<OnlineInvoice> DownloadInvoices()
        {
            List<OnlineInvoice> results = new List<OnlineInvoice>();
            try
            {
                _logger.Debug($"Downloading orders from {_client.BaseUrl}/sage/order");

                var orders = ExecuteRestRequest<OnlineInvoice>("sage/order");
                results.AddRange(orders);

                _logger.Debug($"Downloaded {orders.Count} orders from the web.");
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred retrieving orders from the web: {0}", ex.Message);
                throw;
            }

            return results;
        }



        public OnlineInvoice DownloadOrder(string orderNumber)
        {
            OnlineInvoice onlineOrder = null;

            try
            {
                _logger.Debug($"Downloading orders from {_client.BaseUrl}/sage/order");

                onlineOrder = ExecuteRestRequest<OnlineInvoice>($"sage/order/{orderNumber}", false).FirstOrDefault();

                _logger.Debug($"Downloaded single order {onlineOrder.OrderID} from the web.");
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred retrieving orders from the web: {0}", ex.Message);
                throw;
            }

            return onlineOrder;
        }

        public void UploadTaxCodes(IEnumerable<SageVatRate> vatRates)
        {
            try
            {
                var data = vatRates.
                    Where(vr => !string.IsNullOrWhiteSpace(vr.Description))
                    .Select(x => new OnlineVatRate()
                {
                    Code = x.Code,
                    Rate = x.Value,
                    Description = x.Description,
                }).ToList();

                dynamic root = new { rates = data };

                ExecuteRequest(Method.POST, "vat_rates/insertUpdateDelete", root);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred uploading vat rates: {0}", ex.Message);
                throw;
            }
        }

        public void UploadNominalCodes(IEnumerable<SageNominalCode> nominalCodes)
        {
            try
            {
                var data = nominalCodes.Select(x => new OnlineNominalCode()
                {
                    Code = x.Code,
                    Title = x.Name,
                }).ToList();

                dynamic root = new { codes = data };

                ExecuteRequest(Method.POST, "nominal_codes/insertUpdateDelete", root);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred uploading nominal codes: {0}", ex.Message);
                throw;
            }
        }

        public void UploadCustomerDetails(IEnumerable<SageCustomer> customers)
        {
            try
            {
                var data = customers.Select(c => new
                {
                    identifier = c.AccountNumber,
                    sage_code = c.AccountNumber,
                    balance = c.Balance,
                    balance_as_of = DateTime.Today.ToString("dd/MM/yyyy"),
                    status = c.OnHold ? "On Hold" : "OK",
                    payment_terms = string.IsNullOrWhiteSpace(c.Terms) ? "TERMS NOT AGREED" : c.Terms
                });

                dynamic root = new { customers = data };

                ExecuteRequest(Method.POST, "sage/customer", root);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred uploading nominal codes: {0}", ex.Message);
                throw;
            }
        }

        public void ConfirmOrders(int[] data)
        {
            dynamic root = new { orders = data };

            ExecuteRequest(Method.PUT, "order/bulkAction/sage/sent", root);
        }

        public void ConfirmCredits(int[] data)
        {
            dynamic root = new { credits = data };

            ExecuteRequest(Method.PUT, "credit/bulkAction/sage/sent", root);
        }

        public IList<OnlineCredit> DownloadCredits()
        {
            List<OnlineCredit> results = new List<OnlineCredit>();
            try
            {
                results.AddRange(ExecuteRestRequest<OnlineCredit>("sage/credit"));
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred retrieving orders from the web: {0}", ex.Message);
                throw;
            }

            return results;
        }


        public OnlineCredit DownloadCredit(string creditID)
        {
            OnlineCredit onlineDredit = null;

            try
            {
                onlineDredit = ExecuteRestRequest<OnlineCredit>($"sage/credit/{creditID}", false).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred retrieving orders from the web: {0}", ex.Message);
                throw;
            }

            return onlineDredit;
        }



        public class RootList<T>
        {
            public List<T> data { get; set; }
        }

        public class Root<T>
        {
            public T data { get; set; }
        }
    }
}
