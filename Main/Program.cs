﻿using SimpleXMLValidatorLibrary;

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
            (@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <books>
                    <book id=""1"">
                        <title>Hello C#</title>
                        <author>David Hsiao</author>
                        <price>199.9</price>
                    </book>
                    <book id=""2"">
                        <title>Hello XML</title>
                        <author>Hsiao David</author>
                        <price>299.9</price>
                    </book>
                </books>", false),// My test case
            
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