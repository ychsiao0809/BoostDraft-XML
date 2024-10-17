using SimpleXMLValidatorLibrary;

class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        bool quiet = false;
        if (args.Length != 0) {
            foreach(string arg in args) {
                if (arg == "--no-attr") { // if needs to compare attributes between start/end block
                    SimpleXmlValidator.SetNoAttribute();
                } else if (arg == "--quiet") { // Display the result as example number instead of the entire xml string.
                    quiet = true;
                }
            }
        }
        // You can use here to test, feel free to modify/add the test cases here.
        // You can also use other ways to test if you want.

        List<(string testCase, bool expectedResult, bool expectedResult_noAttr)> testCases = new()
        {
            ("<Design><Code>hello world</Code></Design>",  true, true),// correct case
            ("<Design><Code>hello world</Code></Design><People>", false, false),// no closing tag for "People"      
            ("<Design><Code>hello world</Code></Design></People>", false, false),// no opening tag for "People"
            ("<People><Design><Code>hello world</People></Code></Design>", false, false),// "/Code" should come before "/People" 
            ("<People age=”1”>hello world</People>", false, false),// no attribute "age=”1”" for closing tag "</People>" and the colons of quoted value should be halfwidth colons            
            (@"<People age=""1"">hello world</People>", false, true),// no attribute "age="1"" for closing tag "</People>" / correct
            (@"<People age=""1"">hello world</People age=""1"">", true, true), // correct case
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
                </books>", false, true), // no attributes at every closing tag / correct
            
        };
        int failedCount = 0;
        int lineID = 1; // Line number of testCases
        foreach ((string input, bool expected, bool expected_noAttr) in testCases)
        {
            // Set noAttribute to check whether should compare attributes
            bool result = SimpleXmlValidator.DetermineXml(input);
            string resultStr = result ? "Valid" : "Invalid";

            string mark;
            if ((result == expected && !SimpleXmlValidator.GetNoAttribute()) || (result == expected_noAttr && SimpleXmlValidator.GetNoAttribute()))
            {
                mark = "OK ";
            }
            else
            {
                mark = "NG ";
                failedCount++;
            }
            if (quiet) {
                Console.WriteLine($"{mark} Example {lineID++}: {resultStr}");
            } else {
                Console.WriteLine($"{mark} {input}: {resultStr}");
            }
        }
        Console.WriteLine($"Result: {testCases.Count - failedCount}/{testCases.Count}");
#else
        string input = args.FirstOrDefault("");
        bool result = SimpleXmlValidator.DetermineXml(input);
        Console.WriteLine(result ? "Valid" : "Invalid");
#endif
    }
}