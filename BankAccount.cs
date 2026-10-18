namespace BankApp;

public enum AccountStatus
{
    Active,
    Suspended,
    Closed
}

public class BankAccount
{
    public string AccountNumber { get; }
    public string OwnerTaxId { get; }
    public decimal CurrentBalance { get; private set; }
    public AccountStatus Status { get; private set; }

    public BankAccount(string accountNumber, string ownerTaxId, decimal initialDeposit)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required.");
        if (initialDeposit < 0)
            throw new ArgumentOutOfRangeException(nameof(initialDeposit), "Initial deposit cannot be negative.");

        AccountNumber = accountNumber;
        OwnerTaxId = ownerTaxId;
        CurrentBalance = initialDeposit;
        Status = AccountStatus.Active;
    }

    public void Deposit(decimal amount)
    {
        if (Status != AccountStatus.Active)
            throw new InvalidOperationException("Account is not active.");
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");

        CurrentBalance += amount;
    }

    public bool TryWithdraw(decimal amount, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (Status != AccountStatus.Active)
        {
            errorMessage = "Account is not active.";
            return false;
        }

        if (amount <= 0)
        {
            errorMessage = "Withdrawal amount must be positive.";
            return false;
        }

        if (amount > CurrentBalance)
        {
            errorMessage = "Insufficient funds.";
            return false;
        }

        CurrentBalance -= amount;
        return true;
    }

    public void Freeze() => Status = AccountStatus.Suspended;
    public void Unfreeze() => Status = AccountStatus.Active;
}
