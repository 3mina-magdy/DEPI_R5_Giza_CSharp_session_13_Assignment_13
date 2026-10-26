using ASSLINQ;

namespace Assignment02Linq;

internal class Program
{
    private static readonly int[] Numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

    private static void Main()
    {
        // The dictionary is loaded once because several questions use the same file.
        string[] dictionaryWords = File.ReadAllLines("dictionary_english.txt")
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .ToArray();

        RunElementOperators();
        RunAggregateOperators(dictionaryWords);
        RunSetOperators();
        RunPartitioningOperators();
        RunQuantifiers(dictionaryWords);
        RunGroupingOperators(dictionaryWords);
    }

    private static void RunElementOperators()
    {
        PrintSection("LINQ - Element Operators");

        // Q1: First returns the first item that matches the condition.
        Product firstOutOfStock = ListGenerators.ProductList
            .First(product => product.UnitsInStock == 0);
        PrintAnswer(1, firstOutOfStock);

        // Q2: FirstOrDefault returns null when no item matches.
        Product? firstAbove1000 = ListGenerators.ProductList
            .FirstOrDefault(product => product.UnitPrice > 1000);
        PrintAnswer(2, firstAbove1000?.ToString() ?? "null");

        // Q3: ElementAt(1) means the second item because indexing starts at zero.
        int secondGreaterThan5 = Numbers
            .Where(number => number > 5)
            .ElementAt(1);
        PrintAnswer(3, secondGreaterThan5);
    }

    private static void RunAggregateOperators(string[] dictionaryWords)
    {
        PrintSection("LINQ - Aggregate Operators");

        int oddNumbersCount = Numbers.Count(number => number % 2 != 0);
        PrintAnswer(1, oddNumbersCount);

        var customerOrderCounts = ListGenerators.CustomerList
            .Select(customer => new
            {
                customer.CustomerName,
                OrderCount = customer.Orders?.Length ?? 0
            });
        PrintSequence(2, customerOrderCounts, item => $"{item.CustomerName}: {item.OrderCount} order(s)");

        var categoryProductCounts = ListGenerators.ProductList
            .GroupBy(product => product.Category)
            .Select(group => new { Category = group.Key, ProductCount = group.Count() });
        PrintSequence(3, categoryProductCounts, item => $"{item.Category}: {item.ProductCount} product(s)");

        PrintAnswer(4, Numbers.Sum());
        PrintAnswer(5, dictionaryWords.Sum(word => word.Length));
        PrintAnswer(6, dictionaryWords.Min(word => word.Length));
        PrintAnswer(7, dictionaryWords.Max(word => word.Length));
        PrintAnswer(8, dictionaryWords.Average(word => word.Length).ToString("F2"));

        var stockPerCategory = ListGenerators.ProductList
            .GroupBy(product => product.Category)
            .Select(group => new { Category = group.Key, UnitsInStock = group.Sum(product => product.UnitsInStock) });
        PrintSequence(9, stockPerCategory, item => $"{item.Category}: {item.UnitsInStock} unit(s)");

        var cheapestPricePerCategory = ListGenerators.ProductList
            .GroupBy(product => product.Category)
            .Select(group => new { Category = group.Key, CheapestPrice = group.Min(product => product.UnitPrice) });
        PrintSequence(10, cheapestPricePerCategory, item => $"{item.Category}: {item.CheapestPrice:C}");

        // Q11: let stores the calculated minimum price for reuse inside the query.
        var cheapestProducts =
            from product in ListGenerators.ProductList
            group product by product.Category into categoryGroup
            let cheapestPrice = categoryGroup.Min(product => product.UnitPrice)
            from product in categoryGroup
            where product.UnitPrice == cheapestPrice
            select new { Category = categoryGroup.Key, product.ProductName, product.UnitPrice };
        PrintSequence(11, cheapestProducts, item => $"{item.Category}: {item.ProductName} ({item.UnitPrice:C})");

        var mostExpensivePricePerCategory = ListGenerators.ProductList
            .GroupBy(product => product.Category)
            .Select(group => new { Category = group.Key, MostExpensivePrice = group.Max(product => product.UnitPrice) });
        PrintSequence(12, mostExpensivePricePerCategory, item => $"{item.Category}: {item.MostExpensivePrice:C}");

        var mostExpensiveProducts =
            from product in ListGenerators.ProductList
            group product by product.Category into categoryGroup
            let mostExpensivePrice = categoryGroup.Max(product => product.UnitPrice)
            from product in categoryGroup
            where product.UnitPrice == mostExpensivePrice
            select new { Category = categoryGroup.Key, product.ProductName, product.UnitPrice };
        PrintSequence(13, mostExpensiveProducts, item => $"{item.Category}: {item.ProductName} ({item.UnitPrice:C})");

        var averagePricePerCategory = ListGenerators.ProductList
            .GroupBy(product => product.Category)
            .Select(group => new { Category = group.Key, AveragePrice = group.Average(product => product.UnitPrice) });
        PrintSequence(14, averagePricePerCategory, item => $"{item.Category}: {item.AveragePrice:C}");
    }

    private static void RunSetOperators()
    {
        PrintSection("LINQ - Set Operators");

        var uniqueCategories = ListGenerators.ProductList
            .Select(product => product.Category)
            .Distinct();
        PrintSequence(1, uniqueCategories);

        var productFirstLetters = ListGenerators.ProductList.Select(product => product.ProductName[0]);
        var customerFirstLetters = ListGenerators.CustomerList.Select(customer => customer.CustomerName[0]);

        // Union keeps only unique values from both sequences.
        PrintSequence(2, productFirstLetters.Union(customerFirstLetters));
        PrintSequence(3, productFirstLetters.Intersect(customerFirstLetters));
        PrintSequence(4, productFirstLetters.Except(customerFirstLetters));

        // Concat preserves duplicates, exactly as the question requests.
        var finalThreeCharacters = ListGenerators.ProductList
            .Select(product => product.ProductName[^3..])
            .Concat(ListGenerators.CustomerList.Select(customer => customer.CustomerName[^3..]));
        PrintSequence(5, finalThreeCharacters);
    }

    private static void RunPartitioningOperators()
    {
        PrintSection("LINQ - Partitioning Operators");

        // Region WA represents Washington in Customers.xml.
        var washingtonOrders = ListGenerators.CustomerList
            .Where(customer => customer.Region == "WA")
            .SelectMany(customer => customer.Orders!);

        PrintSequence(1, washingtonOrders.Take(3), order => order.ToString());
        PrintSequence(2, washingtonOrders.Skip(2), order => order.ToString());

        // TakeWhile stops as soon as its condition becomes false.
        PrintSequence(3, Numbers.TakeWhile((number, index) => number >= index));

        // SkipWhile discards items until the first number divisible by 3 is reached.
        PrintSequence(4, Numbers.SkipWhile(number => number % 3 != 0));

        PrintSequence(5, Numbers.SkipWhile((number, index) => number >= index));
    }

    private static void RunQuantifiers(string[] dictionaryWords)
    {
        PrintSection("LINQ - Quantifiers");

        bool containsEi = dictionaryWords.Any(word => word.Contains("ei", StringComparison.OrdinalIgnoreCase));
        PrintAnswer(1, containsEi);

        var categoriesWithOutOfStockProduct =
            from product in ListGenerators.ProductList
            group product by product.Category into categoryGroup
            where categoryGroup.Any(product => product.UnitsInStock == 0)
            select categoryGroup;
        PrintProductGroups(2, categoriesWithOutOfStockProduct);

        var categoriesWithAllProductsInStock =
            from product in ListGenerators.ProductList
            group product by product.Category into categoryGroup
            where categoryGroup.All(product => product.UnitsInStock > 0)
            select categoryGroup;
        PrintProductGroups(3, categoriesWithAllProductsInStock);
    }

    private static void RunGroupingOperators(string[] dictionaryWords)
    {
        PrintSection("LINQ - Grouping Operators");

        List<int> remainderNumbers = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        var groupsByRemainder = remainderNumbers.GroupBy(number => number % 5);

        Console.WriteLine("\n1) Numbers grouped by their remainder when divided by 5:");
        foreach (var group in groupsByRemainder)
        {
            Console.WriteLine($"Numbers with a remainder of {group.Key} when divided by 5: {string.Join(" ", group)}");
        }

        var wordsByFirstLetter = dictionaryWords
            .GroupBy(word => char.ToUpperInvariant(word[0]))
            .OrderBy(group => group.Key);

        Console.WriteLine("\n2) Dictionary words grouped by first letter:");
        foreach (var group in wordsByFirstLetter)
        {
            // Showing a preview keeps console output readable while grouping every word.
            Console.WriteLine($"{group.Key}: {group.Count()} word(s) | Preview: {string.Join(" ", group.Take(12))}");
        }

        // The leading space in " last" is handled by AnagramComparer.Trim().
        string[] words = { "from", "salt", "earn", " last", "near", "form" };
        var anagramGroups = words.GroupBy(word => word, new AnagramComparer());

        Console.WriteLine("\n3) Words grouped by the same characters:");
        foreach (var group in anagramGroups)
        {
            Console.WriteLine(string.Join(" ", group.Select(word => word.Trim())));
        }
    }

    private static void PrintProductGroups(int questionNumber, IEnumerable<IGrouping<string, Product>> groups)
    {
        Console.WriteLine($"\n{questionNumber})");
        foreach (var group in groups)
        {
            Console.WriteLine($"{group.Key}:");
            foreach (Product product in group)
            {
                Console.WriteLine($"  - {product.ProductName} | Stock: {product.UnitsInStock}");
            }
        }
    }

    private static void PrintSection(string sectionName)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 65));
        Console.WriteLine(sectionName);
        Console.WriteLine(new string('=', 65));
    }

    private static void PrintAnswer<T>(int questionNumber, T answer)
    {
        Console.WriteLine($"\n{questionNumber}) {answer}");
    }

    private static void PrintSequence<T>(int questionNumber, IEnumerable<T> answers, Func<T, string>? formatter = null)
    {
        Console.WriteLine($"\n{questionNumber})");
        foreach (T answer in answers)
        {
            Console.WriteLine(formatter?.Invoke(answer) ?? answer?.ToString());
        }
    }

    private sealed class AnagramComparer : IEqualityComparer<string>
    {
        public bool Equals(string? first, string? second) =>
            string.Equals(Normalize(first), Normalize(second), StringComparison.Ordinal);

        public int GetHashCode(string value) => Normalize(value).GetHashCode(StringComparison.Ordinal);

        private static string Normalize(string? value) => new string((value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .OrderBy(character => character)
            .ToArray());
    }
}

