﻿using Growflo.Integration.Core.Database;
using Growflo.Integration.Core.Entities.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Quantil.Database
{
    public class DatabaseController : DbControllerBase, IDatabaseController
    {
        public bool CheckConnection(ref string errorMessage)
        {
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString()))
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public void SaveInvoiceCredit(OnlineInvoice invoice)
        {
            string sql =
                " INSERT INTO [InvoiceCredit] " +
                " ([InvoiceCreditID], [OrderID],[Name],[AccountIdentifier],[CurrencyCode],[PurchaseOrderNo] " +
                " ,[IsCredit],[InvoiceNumber],[CreditNumber],[Imported],[DateTimeCreated]) " +
                "  VALUES (@InvoiceCreditID, @OrderID, @Name, @AccountIdentifier, @CurrencyCode, @PurchaseOrderNo, " +
                " @IsCredit, @InvoiceNumber, @CreditNumber, @Imported, @DateTimeCreated) ";

            int today = int.Parse(DateTime.Today.ToString("yyyyMMdd"));

            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InvoiceCreditID", invoice.InvoiceNumber);
                    command.Parameters.AddWithValue("@OrderId", invoice.OrderID);
                    command.Parameters.AddWithValue("@Name", invoice.Name);
                    command.Parameters.AddWithValue("@AccountIdentifier", invoice.AccountIdentifier);
                    command.Parameters.AddWithValue("@CurrencyCode", invoice.CurrencyCode);
                    command.Parameters.AddWithValue("@PurchaseOrderNo", invoice.PurchaseOrderNo);
                    command.Parameters.AddWithValue("@IsCredit", invoice.IsCredit ? 1 : 0);
                    command.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
                    command.Parameters.AddWithValue("@CreditNumber", DBNull.Value);
                    command.Parameters.AddWithValue("@Imported", 0);
                    command.Parameters.AddWithValue("@DateTimeCreated", today);


                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetInvoiceCreditAsImported(string invoiceCreditID)
        {
            string sql = $" UPDATE InvoiceCredit SET Imported = 1 WHERE InvoiceCreditID = {invoiceCreditID} ";
            ExecuteNonQuery(sql);
        }
        public void SaveInvoiceCredit(OnlineCredit credit)
        {
            string sql =
            " INSERT INTO [InvoiceCredit] " +
            " ([InvoiceCreditID], [OrderID],[Name],[AccountIdentifier],[CurrencyCode],[PurchaseOrderNo] " +
            " ,[IsCredit],[InvoiceNumber],[CreditNumber],[Imported],[DateTimeCreated]) " +
            "  VALUES (@InvoiceCreditID, @OrderID, @Name, @AccountIdentifier, @CurrencyCode, @PurchaseOrderNo, " +
            " @IsCredit, @InvoiceNumber, @CreditNumber, @Imported, @DateTimeCreated) ";

            int today = int.Parse(DateTime.Today.ToString("yyyyMMdd"));

            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InvoiceCreditID", credit.CreditNumber);
                    command.Parameters.AddWithValue("@OrderId", DBNull.Value);
                    command.Parameters.AddWithValue("@Name", credit.Name);
                    command.Parameters.AddWithValue("@AccountIdentifier", credit.AccountIdentifier);
                    command.Parameters.AddWithValue("@CurrencyCode", credit.CurrencyCode);
                    command.Parameters.AddWithValue("@PurchaseOrderNo", DBNull.Value);
                    command.Parameters.AddWithValue("@IsCredit", credit.IsCredit ? 1 : 0);
                    command.Parameters.AddWithValue("@InvoiceNumber", credit.InvoiceNumber);
                    command.Parameters.AddWithValue("@CreditNumber", DBNull.Value);
                    command.Parameters.AddWithValue("@Imported", 0);
                    command.Parameters.AddWithValue("@DateTimeCreated", today);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        internal DataSet GetImportByDate(DateTime date)
        {
            DataSet ds = new DataSet();

            string sql =
            " select InvoiceCreditId as InvoiceOrCreditNo, AccountIdentifier as AccountNo, OrderId as OrderNo, 'True' as Result, 'OK' as Message " +
            " from InvoiceCredit " +
            $" where DateTimeCreated >= {date.Date:yyyyMMdd} and DateTimeCreated < {date.Date.AddDays(1):yyyyMMdd} ";

            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        connection.Open();
                        adapter.Fill(ds);
                    }
                }
            }

            return ds;
        }

        public bool CheckForInvoice(OnlineInvoice invoice)
        {
            string sql = $"SELECT * FROM InvoiceCredit WHERE InvoiceCreditID = {invoice.InvoiceNumber}";

            var result = GetDataset(sql);

            return result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0;
        }

        public bool CheckForCredit(OnlineCredit credit)
        {
            string sql = $"SELECT * FROM InvoiceCredit WHERE InvoiceCreditID = {credit.CreditNumber}";

            var result = GetDataset(sql);

            return result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0;
        }
    }
}
