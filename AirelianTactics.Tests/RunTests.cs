using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace AirelianTactics.Tests
{
    /// <summary>
    /// This class demonstrates how to programmatically run tests if needed.
    /// Normally, you would use the test runner in Visual Studio or 'dotnet test' command.
    /// </summary>
    public class RunTests
    {
        public static void RunAllTests(string[] args)
        {
            Console.WriteLine("Running tests for AirelianTactics State Management...");
            
            // Find all test classes in the assembly
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<TestClassAttribute>() != null)
                {
                    Console.WriteLine($"\nTest Class: {type.Name}");
                    RunTestsInClass(type);
                }
            }
            
            Console.WriteLine("\nAll tests completed!");
        }
        
        private static void RunTestsInClass(Type testClass)
        {
            var instance = Activator.CreateInstance(testClass);
            
            // Find setup method
            MethodInfo setupMethod = null;
            foreach (var method in testClass.GetMethods())
            {
                if (method.GetCustomAttribute<TestInitializeAttribute>() != null)
                {
                    setupMethod = method;
                    break;
                }
            }
            
            // Run each test method
            foreach (var method in testClass.GetMethods())
            {
                if (method.GetCustomAttribute<TestMethodAttribute>() != null)
                {
                    try
                    {
                        Console.WriteLine($"  Running test: {method.Name}");
                        
                        // Run setup if it exists
                        setupMethod?.Invoke(instance, null);
                        
                        // Run the test
                        method.Invoke(instance, null);
                        
                        Console.WriteLine($"  ✓ PASSED: {method.Name}");
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            var expectedExAttr = method.GetCustomAttribute<ExpectedExceptionAttribute>();
                            if (expectedExAttr != null && expectedExAttr.ExceptionType == ex.InnerException.GetType())
                            {
                                Console.WriteLine($"  ✓ PASSED (Expected exception): {method.Name}");
                            }
                            else
                            {
                                Console.WriteLine($"  ✗ FAILED: {method.Name} - {ex.InnerException.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"  ✗ FAILED: {method.Name} - {ex.Message}");
                        }
                    }
                }
            }
        }
    }
} 