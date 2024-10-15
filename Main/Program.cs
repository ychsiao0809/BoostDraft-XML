using SimpleXMLValidatorLibrary;

class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        // You can use here to test, feel free to modify/add the test cases here.
        // You can also use other ways to test if you want.

        List<(string testCase, bool expectedResult)> testCases = new()
        {
            ("<Design><Code>hello world</Code></Design>",  true),//normal case
            ("<Design><Code>hello world</Code></Design><People>", false),//no closing tag for "People" 
            ("<People><Design><Code>hello world</People></Code></Design>", false),// "/Code" should come before "/People" 
            ("<People age=”1”>hello world</People>", false),//there is no closing tag for "People age=”1”" and no opening tag for "/People"
        };
        int failedCount = 0;
        foreach ((string input, bool expected) in testCases)
        {
            bool result = SimpleXmlValidator.DetermineXml(input);
            string resultStr = result ? "Valid" : "Invalid";

            string mark;
            if (result == expected)
            {
                mark = "OK ";
            }
            else
            {
                mark = "NG ";
                failedCount++;
            }
            Console.WriteLine($"{mark} {input}: {resultStr}");
        }
        Console.WriteLine($"Result: {testCases.Count - failedCount}/{testCases.Count}");
#else
        string input = args.FirstOrDefault("");
        bool result = SimpleXmlValidator.DetermineXml(input);
        Console.WriteLine(result ? "Valid" : "Invalid");
#endif
    }
}