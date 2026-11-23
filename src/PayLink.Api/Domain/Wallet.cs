namespace PayLink.Api.Domain;

/// <summary>
/// A customer wallet.
///
/// The two balances are deliberately separate, because the project's hardest cases
/// live exactly here: money that is promised to a biller (held) is no longer
/// available to spend, but it has not left the wallet yet either.
///
///   AvailableBalance  money the customer can spend right now
///   HeldBalance       money reserved by an in-flight payment
///
/// An invariant of this class: neither balance can ever go negative.
/// </summary>
public sealed class Wallet
{
    private Wallet(Guid id, string currency, decimal initialBalance)
    {
        Id = id;
        Currency = currency;
        AvailableBalance = initialBalance;
    }

    public Guid Id { get; }

    public string Currency { get; }

    public decimal AvailableBalance { get; private set; }

    public decimal HeldBalance { get; private set; }

    public decimal TotalBalance => AvailableBalance + HeldBalance;

    public static Wallet Open(string currency, decimal initialBalance = 0m)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        if (initialBalance < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(initialBalance), "Opening balance cannot be negative.");
        }

        return new Wallet(Guid.NewGuid(), currency.ToUpperInvariant(), initialBalance);
    }

    /// <summary>Adds money to the available balance.</summary>
    public void TopUp(decimal amount)
    {
        EnsurePositive(amount);
        AvailableBalance += amount;
    }

    /// <summary>
    /// Reserves money for an in-flight payment. The amount leaves the available
    /// balance but stays in the wallet until it is captured or released.
    /// </summary>
    public void Hold(decimal amount)
    {
        EnsurePositive(amount);

        if (AvailableBalance < amount)
        {
            throw new InsufficientFundsException(amount, AvailableBalance);
        }

        AvailableBalance -= amount;
        HeldBalance += amount;
    }

    /// <summary>Gives a hold back to the customer — used when a payment fails.</summary>
    public void Release(decimal amount)
    {
        EnsurePositive(amount);

        if (HeldBalance < amount)
        {
            throw new InvalidOperationException($"Cannot release {amount} — only {HeldBalance} is held.");
        }

        HeldBalance -= amount;
        AvailableBalance += amount;
    }

    /// <summary>
    /// Completes a hold: the money leaves the wallet for good because the biller
    /// confirmed the payment.
    /// </summary>
    public void Capture(decimal amount)
    {
        EnsurePositive(amount);

        if (HeldBalance < amount)
        {
            throw new InvalidOperationException($"Cannot capture {amount} — only {HeldBalance} is held.");
        }

        HeldBalance -= amount;
    }

    private static void EnsurePositive(decimal amount)
    {
        if (amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }
    }
}

public sealed class InsufficientFundsException : InvalidOperationException
{
    public InsufficientFundsException(decimal requested, decimal available)
        : base($"Cannot hold {requested} — only {available} is available.")
    {
        Requested = requested;
        Available = available;
    }

    public decimal Requested { get; }

    public decimal Available { get; }
}
