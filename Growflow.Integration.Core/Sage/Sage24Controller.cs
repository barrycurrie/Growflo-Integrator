using Growflo.Integration.Core.Sage.Entities;
using NLog;
using SageDataObject240;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Data;
using System.Reflection;

namespace Growflo.Integration.Core.Sage
{
    public class Sage24Controller : BaseSageController, ISageController
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        protected SDOEngine _engine;
        protected WorkSpace _workSpace;

        public Sage24Controller(string sageDataPath) : base(sageDataPath)
        {

        }

        public Sage24Controller() : base ()
        {

        }

        public override bool Connect(string username, string password, ref string errorMessage)
        {
            try
            {
                if (!Directory.Exists(SageDataPath))
                {
                    throw new DirectoryNotFoundException(
                        $"The specified sage data path does not exist: {SageDataPath}");
                }

                _engine = new SDOEngine();
                _workSpace = (WorkSpace)_engine.Workspaces.Add("GROWFLO");

                IsConnected = _workSpace.Connect(SageDataPath, username, password);
            }
            catch (Exception)
            {
                IsConnected = false;
                errorMessage = _engine.LastError.Text;
            }

            return IsConnected;
        }

        public override void Disconnect()
        {
            if (IsConnected && _workSpace != null)
            {
                _workSpace.Disconnect();
            }
        }

        public override void Dispose()
        {
            Disconnect();

            if (_workSpace != null)
                Marshal.FinalReleaseComObject(_workSpace);

            if (_engine != null)
                Marshal.FinalReleaseComObject(_engine);
        }

        /// <summary>
        /// Writes to an SDO field
        /// </summary>
        /// <param name="item">The object to write to</param>
        /// <param name="fieldName">Name of the required field</param>
        /// <param name="value">Value you wish to write</param>
        public override void Write(object item, String fieldName, object value)
        {
            Fields fields = null;
            Field field = null;

            try
            {
                //Stores the required field name in an object
                fields = GetFields(item);
                field = GetField(fields, fieldName);
                field.Value = value;
            }
            finally
            {
                Marshal.FinalReleaseComObject(field);
                Marshal.FinalReleaseComObject(fields);
            }
        }

        /// <summary>
        /// Reads and SDO field and returns its value as an object
        /// </summary>
        /// <param name="item">The object to read from</param>
        /// <param name="fieldName">Name of the required field</param>
        /// <returns>Returns an object containing the value from the field</returns>
        public override object Read(object item, String fieldName)
        {
            Fields fields = null;
            Field field = null;
            object value = null;

            try
            {
                //Stores the required field name in an object
                fields = GetFields(item);
                field = GetField(fields, fieldName);
                value = field.Value;
            }
            finally
            {
                Marshal.FinalReleaseComObject(field);
                Marshal.FinalReleaseComObject(fields);
            }

            return value;
        }

        public Field GetField(Fields fields, string fieldName)
        {
            object fieldValue = fieldName;
            return fields.Item(ref fieldValue);
        }

        public Fields GetFields(object item)
        {
            return (Fields)item.GetType().InvokeMember("Fields", System.Reflection.BindingFlags.GetProperty, null, item, null);
        }

        /// <summary>
        /// Invokes the Add() method of an items collection
        /// </summary>
        /// <param name="item">The items collection you wish to invoke the Add() method on</param>
        /// <returns></returns>
        public override object Add(object item)

        {
            //Uses reflection to invoke the Add() Method on the required object
            return item.GetType().InvokeMember("Add", System.Reflection.BindingFlags.InvokeMethod, null, item, null);
        }

        public override void VerifyConnection()
        {
            if (_workSpace == null || !IsConnected)
                throw new SageConnectionFailedException();
        }

        public override SageActionResult<SageCustomer> CreateCustomer(SageCustomer customer)
        {
            VerifyConnection();

            SageActionResult<SageCustomer> result = null;
            SalesRecord salesRecord;

            try
            {
                salesRecord = (SalesRecord)_workSpace.CreateObject("SalesRecord");

                Write(salesRecord, "ACCOUNT_REF", customer.AccountNumber);

                if (salesRecord.Find(false))
                {
                    result = new SageActionResult<SageCustomer>()
                    {
                        Id = customer.AccountNumber,
                        Action = "Create Customer",
                        Data = customer,
                        Result = SageActionResultType.NoAction,
                        Message = $"{customer.AccountNumber} already exists.",
                    };

                    return result;
                }

                if (salesRecord.AddNew())
                {
                    Write(salesRecord, "ACCOUNT_REF", (string)customer.AccountNumber);
                    Write(salesRecord, "NAME", (string)customer.Name);
                    Write(salesRecord, "ADDRESS_1", (string)customer.InvoiceAddressLine1);
                    Write(salesRecord, "ADDRESS_2", (string)customer.InvoiceAddressLine2);
                    Write(salesRecord, "ADDRESS_3", (string)customer.InvoiceAddressLine3);
                    Write(salesRecord, "ADDRESS_4", (string)customer.InvoiceAddressLine4);
                    Write(salesRecord, "ADDRESS_5", (string)customer.InvoiceAddressLine5);
                    Write(salesRecord, "E_MAIL", (string)customer.Email);
                    Write(salesRecord, "CONTACT_NAME", (string)"");
                    Write(salesRecord, "TELEPHONE", (string)"");
                    Write(salesRecord, "FAX", (string)"");
                    Write(salesRecord, "ANALYSIS_1", (string)"");
                    Write(salesRecord, "ANALYSIS_2", (string)"");
                    Write(salesRecord, "ANALYSIS_3", (string)"");
                    Write(salesRecord, "TERMS", (string)"");
                    Write(salesRecord, "DEF_NOM_CODE", (string)"");
                    Write(salesRecord, "VAT_REG_NUMBER", (string)customer.VatNumber);
                    Write(salesRecord, "COUNTRY_CODE", (string)"");

                    if (salesRecord.Update())
                    {
                        result = new SageActionResult<SageCustomer>()
                        {
                            Action = "Create Customer",
                            Id = customer.AccountNumber,
                            Data = customer,
                            Result = SageActionResultType.Success,
                            Message = $"{customer.AccountNumber} created successfully."
                        };
                    }
                    else
                    {
                        result = new SageActionResult<SageCustomer>()
                        {
                            Action = "Create Customer",
                            Id = customer.AccountNumber,
                            Data = customer,
                            Result = SageActionResultType.Failure,
                            Message = $"{customer.AccountNumber} failed to create."
                        };
                    }


                }
            }
            catch (Exception ex)
            {
                result = new SageActionResult<SageCustomer>()
                {
                    Action = "Create Customer",
                    Id = customer.AccountNumber,
                    Data = customer,
                    Result = SageActionResultType.Failure,
                    Message = $"Could not create new customer record for {customer.AccountNumber}.\n\r{ex.Message}",
                };
            }

            return result;
        }

        public override IList<SageCustomer> GetCustomers()
        {
            VerifyConnection();

            SalesRecord salesRecord = null;
            List<SageCustomer> customers = new List<SageCustomer>();

            try
            {
                salesRecord = (SalesRecord) _workSpace.CreateObject("SalesRecord");

                if (salesRecord.MoveFirst())
                {
                    do
                    {
                        SageCustomer customer = new SageCustomer
                        {
                            AccountNumber = Convert.ToString(Read(salesRecord, "ACCOUNT_REF")),
                            Name = Convert.ToString(Read(salesRecord, "NAME")),
                            Balance = Convert.ToDecimal(Read(salesRecord, "BALANCE")),
                            Terms = Convert.ToString(Read(salesRecord, "TERMS")),
                            OnHold = Convert.ToBoolean(Read(salesRecord, "ACCOUNT_ON_HOLD")),
                            InvoiceAddressLine1 = Convert.ToString(Read(salesRecord, "ADDRESS_1")),
                            InvoiceAddressLine2 = Convert.ToString(Read(salesRecord, "ADDRESS_2")),
                            InvoiceAddressLine3 = Convert.ToString(Read(salesRecord, "ADDRESS_3")),
                            InvoiceAddressLine4 = Convert.ToString(Read(salesRecord, "ADDRESS_4")),
                            InvoiceAddressLine5 = Convert.ToString(Read(salesRecord, "ADDRESS_5")),
                            Email = Convert.ToString(Read(salesRecord, "E_MAIL")),
                            VatNumber = Convert.ToString(Read(salesRecord, "VAT_REG_NUMBER")),
                        };

                        customers.Add(customer);

                    } while (salesRecord.MoveNext());
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(salesRecord);
            }

            return customers;
        }

        public override IList<SageVatRate> GetTaxCodes()
        {
            VerifyConnection();


            ControlData controlData = null;
            List<SageVatRate> taxCodes = new List<SageVatRate>();

            try
            {
                controlData = (ControlData)_workSpace.CreateObject("CONTROLDATA");
                controlData.Read(1);

                //Loop through tax codes and add the details to a list view
                for (Int32 i = 0; i <= 99; i++)
                {
                    SageVatRate sageVatRate = new SageVatRate();
                    sageVatRate.Code = $"T{i}";

                    object fieldName = "T" + i + "_DESCRIPTION";
                    sageVatRate.Description = Convert.ToString(controlData.Fields.Item(ref fieldName).Value);

                    //Build the field name (RATE) object and add this field to the list view item
                    fieldName = "T" + i + "_RATE";
                    sageVatRate.Value = Convert.ToDecimal(controlData.Fields.Item(ref fieldName).Value);

                    taxCodes.Add(sageVatRate);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(controlData);
            }

            return taxCodes;
        }

        public override IList<SageNominalCode> GetNominalCodes()
        {
            VerifyConnection();

            var nominalAccounts = new List<SageNominalCode>();
            NominalRecord nominalRecord = null;

            try
            {
                nominalRecord = (NominalRecord) _workSpace.CreateObject("NominalRecord");

                if (nominalRecord.MoveFirst())
                {
                    do
                    {
                        var nominalCode = new SageNominalCode();
                        nominalCode.Code = (string)Read(nominalRecord, "ACCOUNT_REF");
                        nominalCode.Name = (string)Read(nominalRecord, "NAME");
                        nominalAccounts.Add(nominalCode);

                    } while (nominalRecord.MoveNext());
                }

            }
            finally
            {
                Marshal.FinalReleaseComObject(nominalRecord);
            }

            return nominalAccounts;
        }

        public override SageActionResult<SageBatchInvoice> CreateBatchInvoice(SageBatchInvoice invoice)
        {
            VerifyConnection();

            var result = new SageActionResult<SageBatchInvoice>()
            { Data = invoice };

            //Declare Variables
            SalesRecord salesRecord;
            TransactionPost transactionPost;
            SplitData splitData;

            try
            {
                //Instantiate Objects
                transactionPost = (TransactionPost)_workSpace.CreateObject("TRANSACTIONPOST");
                salesRecord = (SalesRecord)_workSpace.CreateObject("SALESRECORD");

                //Read the first customer
                Write(salesRecord, "ACCOUNT_REF", invoice.CustomerAccountNumber);

                if (!salesRecord.Find(false))
                {
                    result.Result = SageActionResultType.Failure;
                    result.Message = $"Unable to create invoice for order no: {invoice.InvoiceReference}.\n\r" +
                    $"Customer account not found: {invoice.CustomerAccountNumber}";
                }
                else
                {
                    //Populate Header Fields
                    Write(transactionPost.Header, "ACCOUNT_REF", (String)Read(salesRecord, "ACCOUNT_REF"));
                    Write(transactionPost.Header, "DATE", (DateTime)invoice.Date);
                    Write(transactionPost.Header, "POSTED_DATE", (DateTime)DateTime.Today);
                    Write(transactionPost.Header, "TYPE", (Byte)(invoice.IsCredit ? TransType.sdoSC : TransType.sdoSI));
                    Write(transactionPost.Header, "INV_REF", (String)invoice.InvoiceReference);

                    IEnumerable<SageBatchInvoice.Split> groupedSplits = GroupSplitsByNominalCodeAndVatRate(invoice);

                    foreach (var split in groupedSplits)
                    {
                        if (split.NetAmount + split.VatAmount == 0)
                        {
                            continue;
                        }

                        //Add a split to the headers item collection
                        splitData = (SplitData)Add(transactionPost.Items);

                        //Populate split fields
                        Write(splitData, "TYPE", Read(transactionPost.Header, "TYPE"));
                        Write(splitData, "NOMINAL_CODE", (String)split.NominalCode);
                        Write(splitData, "TAX_CODE", (Int16)ConvertVatCodeToShort(split.VatCode));
                        Write(splitData, "NET_AMOUNT", (Double) Math.Round(split.NetAmount, 2));
                        Write(splitData, "TAX_AMOUNT", (Double) Math.Round(split.VatAmount, 2));
                        Write(splitData, "DETAILS", (String)split.Details);
                        Write(splitData, "DATE", (DateTime)Read(transactionPost.Header, "DATE"));
                    }

                    int nextNumber = transactionPost.PostingNumber;

                    //Update the transaction post object
                    transactionPost.Update();
                    string message = $"{invoice.InvoiceType} created in {invoice.CustomerAccountNumber} for Growflo order: {invoice.InvoiceReference}";
                    _logger.Debug(message);

                    result.Id = invoice.InvoiceReference;
                    result.Action = "Create Invoice";
                    result.Data.SageInvoiceNo = nextNumber;
                    result.Result = SageActionResultType.Success;
                    result.Message = message;
                }
            }
            catch (Exception ex)
            {
                string error = $"An error occured creating a sage invoice for order {invoice.InvoiceReference}, A/C:{invoice.CustomerAccountNumber}.{ex.Message}";
                _logger.Error(ex, error);

                result.Result = SageActionResultType.Failure;
                result.Message = error;
            }

            return result; 
        }

        private static IEnumerable<SageBatchInvoice.Split> GroupSplitsByNominalCodeAndVatRate(SageBatchInvoice invoice)
        {
            Dictionary<string, SageBatchInvoice.Split> results = new Dictionary<string, SageBatchInvoice.Split>();

            foreach (SageBatchInvoice.Split split in invoice.Splits)
            {
                SageBatchInvoice.Split summarySplit;
                string key = $"{split.NominalCode}.{split.VatCode}";

                if (!results.ContainsKey(key))
                {
                    summarySplit = new SageBatchInvoice.Split();
                    summarySplit.VatCode = split.VatCode;
                    summarySplit.NominalCode = split.NominalCode;
                    summarySplit.VatAmount = split.VatAmount;
                    summarySplit.NetAmount = split.NetAmount;
                    summarySplit.Date = invoice.Date;
                    summarySplit.Details = "";
                    results.Add(key, summarySplit);
                }
                else
                {
                    summarySplit = results[key];
                    summarySplit.VatAmount += split.VatAmount;
                    summarySplit.NetAmount += split.NetAmount;
                }
            }

            return results.Values.Where(s => s.NetAmount + s.VatAmount > 0); 
        }

        private short ConvertVatCodeToShort(string vatCode)
        {
            vatCode = vatCode.Replace("T", string.Empty);
            return short.Parse(vatCode);
        }

        #region Stock
        public override DataSet GetStockData(string[] columns, bool includeStockCategories)
        {
            VerifyConnection();

            StockRecord stockRecord = null;
            var results = new DataSet();

            try
            {
                stockRecord = (StockRecord)_workSpace.CreateObject(nameof(StockRecord));

                if (stockRecord.MoveFirst())
                {
                    var stockItems = CreateDataTableFromSageObject(stockRecord, "StockItems", columns);
                    results.Tables.Add(stockItems);

                    do
                    {
                        var dataRow = stockItems.NewRow();
                        PopulateDataRowFromSageObject(dataRow, stockRecord);

                        stockItems.Rows.Add(dataRow);

                    } while (stockRecord.MoveNext());

                    if (includeStockCategories)
                    {
                        // Get Stock Category Data
                        var stockCategoriesDataSet = GetStockCategoryData();
                        results.Merge(stockCategoriesDataSet);
                    }
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(stockRecord);
            }

            return results;
        }

        public DataSet GetStockCategoryData()
        {
            VerifyConnection();

            StockCategory stockCategory = null;
            var results = new DataSet();

            try
            {
                stockCategory = (StockCategory)_workSpace.CreateObject(nameof(StockCategory));

                if (stockCategory.Open(OpenMode.sdoRead))
                {
                    var stockCategoryDataTable = new DataTable("StockCategories");
                    stockCategoryDataTable.Columns.Add(new DataColumn("ID", typeof(int)));
                    stockCategoryDataTable.Columns.Add(new DataColumn("NAME", typeof(string)));
                    results.Tables.Add(stockCategoryDataTable);

                    for (int i = 1; i <= stockCategory.Count(); i++)
                    {
                        stockCategory.Read(i);

                        var dataRow = stockCategoryDataTable.NewRow();
                        dataRow["ID"] = i;
                        dataRow["NAME"] = Read(stockCategory, "NAME");
                        stockCategoryDataTable.Rows.Add(dataRow);
                    }
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(stockCategory);
            }

            return results;
        }


        #endregion

        #region Customers

        public override DataSet GetCustomersData(string[] columns)
        {
            VerifyConnection();

            SalesRecord salesRecord = null;
            var results = new DataSet("Customers");

            try
            {
                salesRecord = (SalesRecord)_workSpace.CreateObject("SalesRecord");

                if (salesRecord.MoveFirst())
                {
                    var customers = CreateDataTableFromSageObject(salesRecord, "Customers", columns);
                    results.Tables.Add(customers);

                    do
                    {
                        var dataRow = customers.NewRow();
                        PopulateDataRowFromSageObject(dataRow, salesRecord);
                        customers.Rows.Add(dataRow);

                    } while (salesRecord.MoveNext());
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(salesRecord);
            }

            return results;
        }

        private DataTable CreateCustomerAddressDataTable()
        {
            var customerAddresses = new DataTable("CustomerAddresses");
            customerAddresses.Columns.Add(new DataColumn("ACCOUNT_REF", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("NAME", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("ADDRESS_TYPE", typeof(string)));

            customerAddresses.Columns.Add(new DataColumn("ADDRESS_LINE_1", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("ADDRESS_LINE_2", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("ADDRESS_LINE_3", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("ADDRESS_LINE_4", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("ADDRESS_LINE_5", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("ADDRESS_NUMBER", typeof(string)));

            customerAddresses.Columns.Add(new DataColumn("CONTACT", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("COUNTRY", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("DESCRIPTION", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("EMAIL", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("FAX", typeof(string)));

            customerAddresses.Columns.Add(new DataColumn("NOTES", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("ROLE", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("TAX_CODE", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("TELEPHONE", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("TELEPHONE2", typeof(string)));
            customerAddresses.Columns.Add(new DataColumn("VAT_NUMBER", typeof(string)));

            return customerAddresses;
        }


        public override DataSet GetCustomerAddressData(string[] columns)
        {
            VerifyConnection();

            return GetData<SalesDeliveryRecord, WorkSpace>(_workSpace, "DeliveryAddresses", columns);
        }

        #endregion

        #region Helper

        public void PopulateDataRowFromSageObject(DataRow dataRow, object sageDataObject)
        {
            var fields = GetFields(sageDataObject);

            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var field = GetField(fields, dataColumn.ColumnName);
                dataRow[dataColumn] = GetValueFromField(field);
            }
        }

        public object GetValueFromField(Field field)
        {
            object result;

            if(field.Value == null)
            {
                result = DBNull.Value;
            }
            else
            {
                result = field.Value;
            }

            return result;
        }

        private DataTable CreateDataTableFromSageObject(object sageDataObject, string tableName, string[] columns = null)
        {
            var result = new DataTable(tableName);

            var fields = GetFields(sageDataObject);

            foreach (Field field in fields)
            {
                string fieldName = field.Name;

                if (columns == null || columns.Contains(field.Name))
                {
                    var dataColumn = CreateDataColumnFromField(field);
                    result.Columns.Add(dataColumn);
                }
            }

            return result;
        }

        public DataColumn CreateDataColumnFromField(Field field)
        {
            var dataColumn = new DataColumn(field.Name);
            dataColumn.DataType = GetFieldType(field);
            dataColumn.ExtendedProperties.Add("VarType", field.Type);
            return dataColumn;
        }

        public Type GetFieldType(Field field)
        {
            Type type;

            switch (field.Type)
            {
                case VarType.sdoVarChar:
                    type = typeof(string);
                    break;
                case VarType.sdoLongVarChar:
                    type = typeof(string);
                    break;
                case VarType.sdoInteger:
                    type = typeof(int);
                    break;
                case VarType.sdoSmallInt:
                    type = typeof(short);
                    break;
                case VarType.sdoTinyInt:
                    type = typeof(short);
                    break;
                case VarType.sdoDouble:
                    type = typeof(double);
                    break;
                case VarType.sdoFloat:
                    type = typeof(float);
                    break;
                case VarType.sdoTimeStamp:
                    type = typeof(DateTime);
                    break;
                case VarType.sdoDate:
                    type = typeof(DateTime);
                    break;
                case VarType.sdoBit:
                    type = typeof(Boolean);
                    break;
                default:
                    throw new Exception($"Invalid field type: {field.Type}");
            }

            return type;
        }

        #endregion  

        public override DataSet GetTaxCodesData()
        {
            VerifyConnection();

            DataSet results = new DataSet();

            ControlData controlData = null;
            List<SageVatRate> taxCodes = new List<SageVatRate>();

            try
            {
                controlData = (ControlData)_workSpace.CreateObject("CONTROLDATA");
                controlData.Read(1);

                var dataTable = new DataTable("TaxCodes");
                dataTable.Columns.Add("ID", typeof(int));
                dataTable.Columns.Add("CODE", typeof(string));
                dataTable.Columns.Add("DESCRIPTION", typeof(string));
                dataTable.Columns.Add("RATE", typeof(decimal));

                //Loop through tax codes and add the details to a list view
                for (int i = 0; i <= 99; i++)
                {
                    var dataRow = dataTable.NewRow();
                    dataRow["ID"] = i;
                    dataRow["Code"] = $"T{i}";

                    object fieldName = "T" + i + "_DESCRIPTION";
                    dataRow["DESCRIPTION"] = Convert.ToString(controlData.Fields.Item(ref fieldName).Value);

                    //Build the field name (RATE) object and add this field to the list view item
                    fieldName = "T" + i + "_RATE";
                    dataRow["RATE"] = Convert.ToDecimal(controlData.Fields.Item(ref fieldName).Value);

                    dataTable.Rows.Add(dataRow);
                }

                results.Tables.Add(dataTable);
            }
            finally
            {
                Marshal.FinalReleaseComObject(controlData);
            }

            return results;
        }

        public override DataSet GetNominalCodesData(string[] columns)
        {
            VerifyConnection();

            var results = new DataSet();
            NominalRecord nominalRecord = null;

            try
            {
                nominalRecord = (NominalRecord)_workSpace.CreateObject(nameof(NominalRecord));
                var dataTable = CreateDataTableFromSageObject(nominalRecord, "NominalCodes", columns);
                results.Tables.Add(dataTable);

                if (nominalRecord.MoveFirst())
                {
                    do
                    {
                        var dataRow = dataTable.NewRow();
                        PopulateDataRowFromSageObject(dataRow, nominalRecord);

                        dataTable.Rows.Add(dataRow);

                    } while (nominalRecord.MoveNext());
                }

            }
            finally
            {
                Marshal.FinalReleaseComObject(nominalRecord);
            }

            return results;
        }

        public override DataSet GetPriceBandsData(string[] columns)
        {
            return GetData<PriceRecord, WorkSpace>(_workSpace, "PriceBands", null);
        }

        public override DataSet GetInvoicesCreditsData(DateTime? from, DateTime? to, string[] columns)
        {
            VerifyConnection();

            DataSet results = null;
            InvoiceRecord invoiceRecord = null;
            InvoiceItem invoiceItem = null;

            //TODO: Remove
            int count = 0;

            try
            {
                invoiceRecord = (InvoiceRecord)_workSpace.CreateObject("InvoiceRecord");
                invoiceItem = (InvoiceItem)_workSpace.CreateObject("InvoiceItem");

                results = CreateInvoicesDataSet(columns, invoiceRecord, invoiceItem);

                if (invoiceRecord.MoveFirst())
                {
                    do
                    {
                        count++;

                        int posted = Convert.ToInt32(Read(invoiceRecord, "POSTED_CODE"));
                        DateTime invoiceDate = Convert.ToDateTime(Read(invoiceRecord, "INVOICE_DATE"));

                        if (posted == 1 &&
                            (!from.HasValue || from <= invoiceDate) &&
                            (!to.HasValue || to >= invoiceDate))
                        {

                            DataTable parent = null;
                            DataTable children = null;

                            InvoiceType invoiceType = (InvoiceType) Convert.ToInt32(Read(invoiceRecord, "INVOICE_TYPE_CODE"));

                            bool? invoiceOrCredit = IsInvoiceOrCredit(invoiceType);

                            if (!invoiceOrCredit.HasValue)
                            {
                                continue;
                            }
                            else if (invoiceOrCredit == true)
                            {
                                parent = results.Tables["INVOICES"];
                                children = results.Tables["INVOICEITEMS"];
                            }
                            else
                            {
                                parent = results.Tables["CREDITS"];
                                children = results.Tables["CREDITITEMS"];
                            }
                            
                            var parentRow = parent.NewRow();
                            parent.Rows.Add(parentRow);

                            PopulateDataRowFromSageObject(parentRow, invoiceRecord);

                            invoiceItem = (InvoiceItem)invoiceRecord.Link;

                            if (invoiceItem.MoveFirst())
                            {
                                do
                                {
                                    var childRow = children.NewRow();
                                    PopulateDataRowFromSageObject(childRow, invoiceItem);
                                    children.Rows.Add(childRow);
                                }
                                while (invoiceItem.MoveNext());
                            }
                        }

                    } while (invoiceRecord.MoveNext());
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(invoiceRecord);
            }

            return results;
        }

        private bool? IsInvoiceOrCredit(InvoiceType invoiceType)
        {
            bool? result;

            switch (invoiceType)
            {
                case InvoiceType.sdoProductInvoice:
                case InvoiceType.sdoSopInvoice:
                case InvoiceType.sdoServiceInvoice:
                    result = true;
                    break;
                case InvoiceType.sdoProductCredit:
                case InvoiceType.sdoServiceCredit:
                    result = false;
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private DataSet CreateInvoicesDataSet(string[] columns, InvoiceRecord invoiceRecord, InvoiceItem invoiceItem)
        {
            DataSet invoicesDataSet = new DataSet("Invoices");

            var  invoices = CreateDataTableFromSageObject(invoiceRecord, "Invoices", columns);
            invoicesDataSet.Tables.Add(invoices);

            var invoiceItems = CreateDataTableFromSageObject(invoiceItem, "InvoiceItems", columns);
            invoicesDataSet.Tables.Add(invoiceItems);

            invoices.ChildRelations.Add(invoices.Columns["INVOICE_NUMBER"], invoiceItems.Columns["INVOICE_NUMBER"]);
            invoices.ChildRelations[0].Nested = true;


            var credits = CreateDataTableFromSageObject(invoiceRecord, "Credits", columns);
            invoicesDataSet.Tables.Add(credits);

            var creditItems = CreateDataTableFromSageObject(invoiceItem, "CreditItems", columns);
            invoicesDataSet.Tables.Add(creditItems);

            credits.ChildRelations.Add(credits.Columns["INVOICE_NUMBER"], creditItems.Columns["INVOICE_NUMBER"]);
            credits.ChildRelations[0].Nested = true;

            return invoicesDataSet;
        }

        public override DataSet GetCurrenciesData()
        {
            throw new NotImplementedException();
        }

        private DataSet GetData<T, W>(object workSpace, string tableName, string[] columns) where T:class where W:class
        {
            VerifyConnection();
            var results = new DataSet();
            T sageObject = null;

            try
            {
                string name = typeof(T).Name;
                sageObject = (T) (typeof(W)).InvokeMember("CreateObject", BindingFlags.InvokeMethod, null, workSpace, new object[] { name });

                //sageObject = (T)_workSpace.CreateObject(name);
                var dataTable = CreateDataTableFromSageObject(sageObject, tableName, columns);
                results.Tables.Add(dataTable);

                object moveFirst =  (typeof(T)).InvokeMember("MoveFirst", BindingFlags.InvokeMethod, null, sageObject, null);
                object moveNext;

                if (Convert.ToBoolean(moveFirst))
                {
                    do
                    {
                        var dataRow = dataTable.NewRow();
                        PopulateDataRowFromSageObject(dataRow, sageObject);

                        dataTable.Rows.Add(dataRow);

                        moveNext = (typeof(T)).InvokeMember("MoveNext", BindingFlags.InvokeMethod, null, sageObject, null);

                    } while (Convert.ToBoolean(moveNext));
                }

            }
            finally
            {
                Marshal.FinalReleaseComObject(sageObject);
            }

            return results;
        }

        public void PostSalesOrder(SageSalesOrderPost sageSalesOrder)
        {
            VerifyConnection();
            SalesRecord oSalesRecord;
            StockRecord oStockRecord;
            SopPost oSopPost;
            SopItem oSopItem;

            //Try a connection, will throw an exception if it fails
            try
            {

                //Instantiate objects
                oSalesRecord = (SageDataObject240.SalesRecord)_workSpace.CreateObject("SalesRecord");
                oStockRecord = (SageDataObject240.StockRecord)_workSpace.CreateObject("StockRecord");
                oSopPost = (SageDataObject240.SopPost)_workSpace.CreateObject("SopPost");

                //Read the first customer
                oSalesRecord.MoveFirst();

                //Populate the order Header Fields
                Write(oSopPost.Header, "ACCOUNT_REF", (String)sageSalesOrder.CustomerAccountNumber);
                Write(oSopPost.Header, "NAME", (String)sageSalesOrder.CustomerName);
                Write(oSopPost.Header, "ADDRESS_1", (String)Read(oSalesRecord, "ADDRESS_1"));
                Write(oSopPost.Header, "ADDRESS_2", (String)Read(oSalesRecord, "ADDRESS_2"));
                Write(oSopPost.Header, "ADDRESS_3", (String)Read(oSalesRecord, "ADDRESS_3"));
                Write(oSopPost.Header, "ADDRESS_4", (String)Read(oSalesRecord, "ADDRESS_4"));
                Write(oSopPost.Header, "ADDRESS_5", (String)Read(oSalesRecord, "ADDRESS_5"));
                Write(oSopPost.Header, "DEL_ADDRESS_1", (String)Read(oSalesRecord, "DEL_ADDRESS_1"));
                Write(oSopPost.Header, "DEL_ADDRESS_2", (String)Read(oSalesRecord, "DEL_ADDRESS_2"));
                Write(oSopPost.Header, "DEL_ADDRESS_3", (String)Read(oSalesRecord, "DEL_ADDRESS_3"));
                Write(oSopPost.Header, "DEL_ADDRESS_4", (String)Read(oSalesRecord, "DEL_ADDRESS_4"));
                Write(oSopPost.Header, "DEL_ADDRESS_5", (String)Read(oSalesRecord, "DEL_ADDRESS_5"));
                Write(oSopPost.Header, "CUST_TEL_NUMBER", (String)Read(oSalesRecord, "TELEPHONE"));
                Write(oSopPost.Header, "CONTACT_NAME", (String)"Chris Reed");
                Write(oSopPost.Header, "GLOBAL_TAX_CODE", (Int16)Read(oSalesRecord, "DEF_TAX_CODE"));

                //Populate other header information
                Write(oSopPost.Header, "ORDER_DATE", (DateTime)DateTime.Today);
                Write(oSopPost.Header, "NOTES_1", (String)"");
                Write(oSopPost.Header, "NOTES_2", (String)"");
                Write(oSopPost.Header, "NOTES_3", (String)"");
                Write(oSopPost.Header, "TAKEN_BY", (String)"Mark Steel");
                // If anything is entered in the GLOBAL_NOM_CODE, all of the updated invoice’s splits will have this 
                // nominal code and also this willforce anything entered in the GLOBAL_DETAILS field into the all
                // the splits details field. 
                Write(oSopPost.Header, "GLOBAL_NOM_CODE", (String)"");
                Write(oSopPost.Header, "GLOBAL_DETAILS", (String)"");

                //Create and order item
                oSopItem = (SageDataObject240.SopItem)Add(oSopPost.Items);

                //Read the first stock code
                oStockRecord.MoveFirst();
                Write(oSopItem, "STOCK_CODE", (String)Read(oStockRecord, "STOCK_CODE"));
                Write(oSopItem, "DESCRIPTION", (String)Read(oStockRecord, "DESCRIPTION"));
                Write(oSopItem, "NOMINAL_CODE", (String)Read(oStockRecord, "NOMINAL_CODE"));
                Write(oSopItem, "TAX_CODE", (Int16)Read(oStockRecord, "TAX_CODE"));

                //Populate other fields required for SOP Item
                //From 2015 the update method now wraps internal business logic 
                //that calculates the vat amount if a net amount is given.
                //If you wish to calculate your own Tax values you will need
                //to ensure that you set the TAX_FLAG to 1 and set the TAX_AMOUNT value on the item line
                //***Note if a NVD is set the item line values will be recalculated 
                //regardless of the Tax_Flag being set to 1***
                Write(oSopItem, "QTY_ORDER", (Double)2);
                Write(oSopItem, "UNIT_PRICE", (Double)100);
                Write(oSopItem, "NET_AMOUNT", (Double)200);
                Write(oSopItem, "FULL_NET_AMOUNT", (Double)200);
                Write(oSopItem, "COMMENT_1", (String)"");
                Write(oSopItem, "COMMENT_2", (String)"");
                Write(oSopItem, "UNIT_OF_SALE", (String)"");
                Write(oSopItem, "TAX_RATE", (Double)20);
                Write(oSopItem, "TAX_CODE", (Int16)1);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred importing a sales order.");
            }
        }
    }
}
