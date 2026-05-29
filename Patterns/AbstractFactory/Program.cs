using System;

// === Abstract Factory — Restaurant Menu Theme ===

public interface IMenuHeader { void Render(); }
public interface IFoodCard { void Render(); }
public interface IOrderButton { void Render(); }

public class PremiumMenuHeader : IMenuHeader { public void Render() => Console.WriteLine("═══════ ✦ Premium Header ✦ ═══════"); }
public class PremiumFoodCard : IFoodCard { public void Render() => Console.WriteLine("  ┌─ Butter Chicken ─── Rs.450 ─┐"); }
public class PremiumOrderButton : IOrderButton { public void Render() => Console.WriteLine("  [ ✦ Add to Cart ✦ ]"); }

public class CasualMenuHeader : IMenuHeader { public void Render() => Console.WriteLine("🍔 Casual Restaurant 🍕"); }
public class CasualFoodCard : IFoodCard { public void Render() => Console.WriteLine("  🍽️ Vada Pav — Rs.30"); }
public class CasualOrderButton : IOrderButton { public void Render() => Console.WriteLine("  [ 🛒 Add! ]"); }

public interface IMenuThemeFactory
{
    IMenuHeader CreateHeader();
    IFoodCard CreateFoodCard();
    IOrderButton CreateOrderButton();
}

public class PremiumThemeFactory : IMenuThemeFactory
{
    public IMenuHeader CreateHeader() => new PremiumMenuHeader();
    public IFoodCard CreateFoodCard() => new PremiumFoodCard();
    public IOrderButton CreateOrderButton() => new PremiumOrderButton();
}

public class CasualThemeFactory : IMenuThemeFactory
{
    public IMenuHeader CreateHeader() => new CasualMenuHeader();
    public IFoodCard CreateFoodCard() => new CasualFoodCard();
    public IOrderButton CreateOrderButton() => new CasualOrderButton();
}

public class MenuRenderer
{
    private readonly IMenuThemeFactory _factory;
    public MenuRenderer(IMenuThemeFactory factory) { _factory = factory; }

    public void RenderMenu()
    {
        _factory.CreateHeader().Render();
        _factory.CreateFoodCard().Render();
        _factory.CreateOrderButton().Render();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Abstract Factory ===\n");
        Console.WriteLine("--- Premium Theme ---");
        new MenuRenderer(new PremiumThemeFactory()).RenderMenu();
        Console.WriteLine("\n--- Casual Theme ---");
        new MenuRenderer(new CasualThemeFactory()).RenderMenu();
    }
}
