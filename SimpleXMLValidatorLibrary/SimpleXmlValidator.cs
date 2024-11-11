namespace SimpleXMLValidatorLibrary
{
    // Customized exception: when a syntax error occurs while setting key and value of attributes
    class InvalidAttributeException: Exception {
        public InvalidAttributeException(string message) : base(message) {            
        }
    }

    public enum XmlBlockType {
        Open, // Represent an opening tag, e.g. <book>
        Close, // Represent a closing tag, e.g. </book>
        Xml, // Represent the XML header, e.g. <?xml version="1.0"?>
    };

    interface IParsable<T> {
        T Parse(string elements);
    }
    
    class XmlParser : IParsable<List<XmlBlock>> {
        // Return list of xml blocks in xml content
        public List<XmlBlock> Parse(string xml) {
            var xmlBlocks = new List<XmlBlock>();
            var currentTag = string.Empty; // Store the tag string in block.
            var currentContent = string.Empty; // Store the content string between blocks.
            var inTag = false; // Check if the character is in block or between blocks.

            // Read XML string by characters
            foreach (char ch in xml)
            {
                if (ch == '<') // start of block
                {
                    inTag = true;
                }
                else if (ch == '>') // end of block
                {
                    inTag = false;
                    // Initial XML block, including tag, attributes, and block type.
                    var currentBlock = XmlBlockFactory.CreateXmlBlock(currentTag);                    

                    if (currentBlock.BlockType == XmlBlockType.Close && !string.IsNullOrWhiteSpace(currentContent)) // close block
                    {
                        xmlBlocks[^1].SetContent(currentContent);
                        currentContent = string.Empty;
                    }
                    
                    xmlBlocks.Add(currentBlock);
                    currentTag = string.Empty; // Refresh tag string
                }
                else if (inTag) // Get Tag
                {
                    currentTag += ch;
                }
                else { // Get Content
                    currentContent += ch;
                }
            }

            return xmlBlocks;
        }
    }

    public static class XmlBlockFactory
    {
        public static XmlBlock CreateXmlBlock(string tagString)
        {
            tagString = tagString.Trim().Trim('?');
            var parts = tagString.Split(' ', 2);
            var tagName = parts[0];
            var blockType = XmlBlockType.Open;

            // Get Xml block type
            if (tagName == "xml")  blockType = XmlBlockType.Xml;
            else if (tagName.StartsWith("/"))
            {
                tagName = tagName[1..]; // remove first '/'
                blockType = XmlBlockType.Close;
            }

            // Get attributes
            var attributes = parts.Length > 1 ? XmlAttributeParser.ParseAttributes(parts[1]) : new Dictionary<string, string>();
            
            return new XmlBlock(tagName, attributes, blockType);
        }
    }

    public static class XmlAttributeParser
    {
        public static Dictionary<string, string> ParseAttributes(string attributeString) {
            var attributes = new Dictionary<string, string>();

            foreach(var element in attributeString.Split(' '))
            {
                // Get key and value
                int position = element.IndexOf("=");
                string key = element[..position];
                string value = element[(position+1)..];

                // Check if value is quoted
                if (value.StartsWith("\"") && value.EndsWith("\"")) {
                    value = value.Trim('\"'); // Remove quotes.
                } else {
                    throw new InvalidAttributeException("The value should be quoted by halfwidth colon");
                }

                attributes.Add(key, value);
            }

            return attributes;
        }
    }

    public class XmlBlock {
        // Properties to hold the tag name, attributes, content, and block type
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public XmlBlockType BlockType { get; set; }
        public string? Content { get; private set; } // Content should stored at close block.

        public XmlBlock (string tagName, Dictionary<string, string> attributes, XmlBlockType blockType) {
            TagName = tagName;
            Attributes = attributes;
            BlockType = blockType;
        }

        public void SetContent (string content) => Content = content;
    }

    public class SimpleXmlValidator
    {
        private static bool noAttribute = false; // default: has to compare attributes.
        
        public static void SetNoAttribute() {
            noAttribute = true;
        }

        public static bool GetNoAttribute() {
            return noAttribute;
        }

        public static bool BlockIsMatch (XmlBlock openBlock, XmlBlock closeBlock) {
            // Check if tag name is matchs.
            if (openBlock.TagName != closeBlock.TagName) {
                return false;
            }
            // Check if attributes are matched.
            if (!noAttribute) {
                if (openBlock.Attributes.Count != closeBlock.Attributes.Count) {
                    return false;
                }
                foreach(var kvp in closeBlock.Attributes) {
                    if (!openBlock.Attributes.TryGetValue(kvp.Key, out var value) || !value.Equals(kvp.Value)) {
                        return false;
                    }
                }
            }
            return true;
        }

        //Please implement this method
        public static bool DetermineXml(string xml)
        {
            XmlParser xmlParser = new XmlParser(); // Turn the input Xml string to list of Xml blocks.
            List<XmlBlock> xmlBlocks; // List of all Xml blocks
            Stack<XmlBlock> openBlockStack = new Stack<XmlBlock>(); // Stack of Xml open blocks.

            // Syntax error handling: value of attributes should be quoted by halfwidth colon.
            try {
                xmlBlocks = xmlParser.Parse(xml); // Get Xml blocks information.
            } catch (InvalidAttributeException ex) {
                // Console.WriteLine(ex.Message);
                return false;
            }


            // Read XML string by character
            foreach (XmlBlock xmlBlock in xmlBlocks)
            {
                if (xmlBlock.BlockType == XmlBlockType.Open) // open block
                {
                    openBlockStack.Push(xmlBlock);
                }
                else if (xmlBlock.BlockType == XmlBlockType.Close) // close block
                {
                    // If there is no open block, return false.
                    if (openBlockStack.Count == 0) {
                        return false;
                    }

                    // Invalid if open/close blocks are unmatched.
                    XmlBlock openBlock = openBlockStack.Pop();
                    if (!BlockIsMatch(openBlock, xmlBlock)) {
                        return false;
                    }
                }                
                // Show block info for test
                // currentBlock.ShowBlockInfo();
            }

            // Invalid if openBlockStack is not empty.
            if(openBlockStack.Count != 0) {
                return false;
            }

            return true;
        }
    }
}