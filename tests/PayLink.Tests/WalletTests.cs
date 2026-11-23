using PayLink.Api.Domain;
using Xunit;

namespace PayLink.Tests;

/// <summary>
/// The wallet is where the project's money rules live, so these tests guard the
/// invariants from day one: no negative balances, and holds that always resolve.
/// </summary>
public sealed class WalletTests
{
    [Fact]
    public void Open_starts_with_the_given_balance()
    {
        var wallet = Wallet.Open("egp", 1000m);

        Assert.Equal(1000m, wallet.AvailableBalance);
        Assert.Equal(0m, wallet.HeldBalance);
        Assert.Equal("EGP", wallet.Currency);
    }

    [Fact]
    public void Hold_moves_money_from_available_to_held()
    {
        var wallet = Wallet.Open("EGP", 1000m);

        wallet.Hold(352.50m);

        Assert.Equal(647.50m, wallet.AvailableBalance);
        Assert.Equal(352.50m, wallet.HeldBalance);
        Assert.Equal(1000m, wallet.TotalBalance);
    }

    [Fact]
    public void Hold_beyond_the_available_balance_is_rejected()
    {
        var wallet = Wallet.Open("EGP", 500m);

        var error = Assert.Throws<InsufficientFundsException>(() => wallet.Hold(400m + 400m));

        Assert.Equal(800m, error.Requested);
        Assert.Equal(500m, error.Available);
        Assert.Equal(500m, wallet.AvailableBalance);
        Assert.Equal(0m, wallet.HeldBalance);
    }

    [Fact]
    public void Two_holds_cannot_overdraw_the_wallet()
    {
        // 500 available, two payments of 400: exactly one may succeed.
        var wallet = Wallet.Open("EGP", 500m);

        wallet.Hold(400m);

        Assert.Throws<InsufficientFundsException>(() => wallet.Hold(400m));
        Assert.Equal(100m, wallet.AvailableBalance);
    }

    [Fact]
    public void Release_returns_held_money_to_the_customer()
    {
        var wallet = Wallet.Open("EGP", 1000m);
        wallet.Hold(352.50m);

        wallet.Release(352.50m);

        Assert.Equal(1000m, wallet.AvailableBalance);
        Assert.Equal(0m, wallet.HeldBalance);
    }

    [Fact]
    public void Capture_removes_held_money_for_good()
    {
        var wallet = Wallet.Open("EGP", 1000m);
        wallet.Hold(352.50m);

        wallet.Capture(352.50m);

        Assert.Equal(647.50m, wallet.AvailableBalance);
        Assert.Equal(0m, wallet.HeldBalance);
        Assert.Equal(647.50m, wallet.TotalBalance);
    }

    [Fact]
    public void Capturing_more_than_is_held_is_rejected()
    {
        var wallet = Wallet.Open("EGP", 1000m);
        wallet.Hold(100m);

        Assert.Throws<InvalidOperationException>(() => wallet.Capture(200m));
        Assert.Equal(100m, wallet.HeldBalance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-352.50)]
    public void Non_positive_amounts_are_rejected(decimal amount)
    {
        var wallet = Wallet.Open("EGP", 1000m);

        Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Hold(amount));
        Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Release(amount));
        Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Capture(amount));
        Assert.Throws<ArgumentOutOfRangeException>(() => wallet.TopUp(amount));
    }

    [Fact]
    public void Opening_a_wallet_with_a_negative_balance_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Wallet.Open("EGP", -1m));
    }
}
