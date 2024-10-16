namespace SimpleXMLValidatorLibrary
{
	//feel free to add other classes/methods if you want
    public class SimpleXmlValidator
    {
		public static void XmlParser(string xml)
        {
            // Current string, whether block elements or content.
            string currentString = string.Empty;
            // Stack the start tags.
            Stack<string> tagStack = new Stack<string>();

            // Read XML string by character
            foreach (char currentChar in xml)
            {
                if (currentChar == '<') // open block
                {
                    if (!string.IsNullOrWhiteSpace(currentString))
                    {
                        // Print if there has content before new block starts.
                        Console.WriteLine(string.Format($"Content: {currentString}"));
                        currentString = string.Empty;
                    }
                }
                else if (currentChar == '>') // close block
                {
                    // Get elements in block, including tag and attributes.
                    string[] elements = currentString.Split(' ');
                    // Tag name of block.
                    string tagName = elements[0];

                    if (tagName == "?xml") // XML block
                    {
                        Console.WriteLine("XML header");
                    }
                    else if (tagName.StartsWith("/")) // end block
                    {
                        tagName = tagName.Substring(1, tagName.Length-1);
                        string startTagName = tagStack.Pop();
                        Console.WriteLine($"End tag: {tagName}");
                    }
                    else // start block
                    {
                        tagStack.Push(tagName);
                        Console.WriteLine($"Start tag: {tagName}");
                    }
                    // Refresh content
                    currentString = string.Empty;
                }
                else // Get content
                {
                    currentString += currentChar;
                }
            }
        }
        //Please implement this method
        public static bool DetermineXml(string xml)
        {
            XmlParser(xml);

            return true;
        }
    }
}