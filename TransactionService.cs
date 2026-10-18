namespace BankApp;

public record TransactionRecord(string TransactionId, string SourceAccount, string TargetAccount, decimal Amount, DateTime Timestamp);

public class TransactionService
{
    private readonly List<TransactionRecord> _history = [];

    public bool TransferFunds(BankAccount source, BankAccount target, decimal amount, out string failureReason)
    {
        failureReason = string.Empty;

        if (source == null || target == null)
        {
            failureReason = "Source or target account cannot be null.";
            return false;
        }

        if (string.Equals(source.AccountNumber, target.AccountNumber, StringComparison.Ordinal))
        {
            failureReason = "Cannot transfer to the same account.";
            return false;
        }

        if (!source.TryWithdraw(amount, out failureReason))
        {
            return false;
        }

        try
        {
            target.Deposit(amount);
            var record = new TransactionRecord(
                Guid.NewGuid().ToString("N"),
                source.AccountNumber,
                target.AccountNumber,
                amount,
                DateTime.UtcNow);
            _history.Add(record);
            return true;
        }
        catch (Exception ex)
        {
            // Rollback
            source.Deposit(amount);
            failureReason = $"Transfer failed: {ex.Message}";
            return false;
        }
    }

    public IReadOnlyList<TransactionRecord> GetHistory() => _history.AsReadOnly();
}
