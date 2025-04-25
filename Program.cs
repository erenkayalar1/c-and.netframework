using System;

// Main program class that serves as the entry point
class Program
{
    static void Main(string[] args)
    {
        // Create shipping service factory
        var factory = new ShippingServiceFactory();
        
        // Get required services from factory
        var inputService = factory.CreateInputService();
        var validationService = factory.CreateValidationService();
        var calculationService = factory.CreateCalculationService();
        
        // Process shipping quote
        ProcessShippingQuote(inputService, validationService, calculationService);
    }
    
    static void ProcessShippingQuote(
        IInputService inputService,
        IValidationService validationService,
        ICalculationService calculationService)
    {
        // Display welcome message
        Console.WriteLine("Welcome to Package Express. Please follow the instructions below.");
        
        // Get and validate weight
        var weight = inputService.GetInput("Please enter the package weight:");
        var (weightValid, weightError) = validationService.ValidateWeight(weight);
        if (!weightValid)
        {
            Console.WriteLine(weightError);
            return;
        }
        
        // Get dimensions
        var width = inputService.GetInput("Please enter the package width:");
        var height = inputService.GetInput("Please enter the package height:");
        var length = inputService.GetInput("Please enter the package length:");
        
        // Validate dimensions
        var (dimensionsValid, dimensionsError) = validationService.ValidateDimensions(width, height, length);
        if (!dimensionsValid)
        {
            Console.WriteLine(dimensionsError);
            return;
        }
        
        // Calculate and display quote
        var quote = calculationService.CalculateShippingCost(weight, width, height, length);
        Console.WriteLine($"Your estimated total for shipping this package is: ${quote:F2}");
        Console.WriteLine("Thank you!");
    }
}

// Abstract factory for creating shipping services
class ShippingServiceFactory
{
    public IInputService CreateInputService()
    {
        return new ConsoleInputService();
    }
    
    public IValidationService CreateValidationService()
    {
        return new StandardValidationService();
    }
    
    public ICalculationService CreateCalculationService()
    {
        return new StandardCalculationService();
    }
}

// Interface for input service
interface IInputService
{
    double GetInput(string prompt);
}

// Interface for validation service
interface IValidationService
{
    (bool isValid, string error) ValidateWeight(double weight);
    (bool isValid, string error) ValidateDimensions(double width, double height, double length);
}

// Interface for calculation service
interface ICalculationService
{
    double CalculateShippingCost(double weight, double width, double height, double length);
}

// Concrete implementation of input service
class ConsoleInputService : IInputService
{
    public double GetInput(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            if (double.TryParse(Console.ReadLine(), out double value))
            {
                return value;
            }
            Console.WriteLine("Invalid input. Please enter a numeric value.");
        }
    }
}

// Concrete implementation of validation service
class StandardValidationService : IValidationService
{
    private const double MaxWeight = 50;
    private const double MaxDimensions = 50;
    
    public (bool isValid, string error) ValidateWeight(double weight)
    {
        if (weight > MaxWeight)
        {
            return (false, "Package too heavy to be shipped via Package Express. Have a good day.");
        }
        return (true, string.Empty);
    }
    
    public (bool isValid, string error) ValidateDimensions(double width, double height, double length)
    {
        if (width + height + length > MaxDimensions)
        {
            return (false, "Package too big to be shipped via Package Express.");
        }
        return (true, string.Empty);
    }
}

// Concrete implementation of calculation service
class StandardCalculationService : ICalculationService
{
    public double CalculateShippingCost(double weight, double width, double height, double length)
    {
        return (width * height * length * weight) / 100;
    }
}