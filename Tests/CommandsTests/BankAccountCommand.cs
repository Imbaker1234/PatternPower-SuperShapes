using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Commands;

namespace CommandsTests
{
    public class BankAccountCommand : Command
    {
        internal BankAccountCommand(Action callAction, Action undoAction, Action<Exception> failureAction) : base(callAction, undoAction, failureAction)
        {
        }
    }

    public class BankAccount
    {
        public long AccountNumber { get; set; }
        public double Balance { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOpened { get; set; }
        public CompositeCommand Transactions { get; set; }

        public BankAccount(long accountNumber, double balance, string firstName, string lastName, DateTime dateOpened, CompositeCommand transactions)
        {
            AccountNumber = accountNumber;
            Balance = balance;
            FirstName = firstName;
            LastName = lastName;
            DateOpened = dateOpened;
            Transactions = transactions;
        }
    }

    public static class BankAccountCommandFactory
    {
        public static BankAccountCommand Deposit(long accountNumber, double amount)
        {
            var account = BankAccounts.Accounts.Find(acc => acc.AccountNumber == accountNumber);

            var product = new BankAccountCommand(
                () => account.Balance += amount, 
                () => account.Balance -= amount,
                e => Console.WriteLine(e.Message));

            account.Transactions.Add(product);

            return product;
        }

        public static BankAccountCommand Withdraw(long accountNumber, double amount)
        {
            var account = BankAccounts.Accounts.Find(acc => acc.AccountNumber == accountNumber);

            var product = new BankAccountCommand(() =>
                {
                    if (account.Balance <= amount) account.Balance -= amount;
                    else throw new Exception($"Amount would overdraft account. Current balance = {account.Balance}");
                }, () => account.Balance += amount,
                e => Console.WriteLine(e.Message));

            account.Transactions.Add(product);

            return product;
        }

        public static CompositeCommand Transfer(long sendingAccountNumber, double withdrawAmount, long receivingAccountNumber, double depositAmount)
        {
            var cmdWithdraw = Withdraw(sendingAccountNumber, withdrawAmount);
            var cmdDeposit = Deposit(receivingAccountNumber, depositAmount);
            return new CompositeCommand(cmdWithdraw, cmdDeposit);
        }
    }

    public static class BankAccounts
    {
        public static List<BankAccount> Accounts = new List<BankAccount>()
        {
            new BankAccount(456987321, 500.21, "Sam", "Smith", DateTime.Now.AddYears(-3), new CompositeCommand()),
            new BankAccount(456987321, 33.22, "George", "Graham", DateTime.Now.AddYears(-3), new CompositeCommand()),
            new BankAccount(456987321, 67.81, "Keith", "Sullivan", DateTime.Now.AddYears(-3), new CompositeCommand()),
        };
    }
}