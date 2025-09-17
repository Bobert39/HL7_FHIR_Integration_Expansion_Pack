using FhirIntegrationService.ValidationSuite.Cli;

namespace FhirIntegrationService.ValidationSuite;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await ValidationCliProgram.Main(args);
    }
}