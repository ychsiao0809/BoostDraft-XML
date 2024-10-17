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

    class TagParser : IParsable<Tuple<string, Dictionary<string, string>, XmlBlockType>> {
        // Retrun tag name, attributes, block type
        public Tuple<string, Dictionary<string, string>, XmlBlockType> Parse(string tagString) {
            // Initailize tag name, attributes, block type
            string tagName = string.Empty;
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            XmlBlockType blockType;

            // Remove space and '?' in both ends.
            // Have to remove '?' or it may occurs error when attribute parsing
            tagString = tagString.Trim().Trim('?');
            // Divide the tag part and attribute part.
            string[] parts = tagString.Split(' ', 2);

            tagName = parts[0]; // Get tag name

            // Get Xml block type
            if (tagName == "xml") {
                blockType = XmlBlockType.Xml;
            } else if (tagName.StartsWith("/")) {
                tagName = tagName.Trim('/');
                blockType = XmlBlockType.Close;
            } else {
                blockType = XmlBlockType.Open;
            }

            if (parts.Length > 1) {
                // Parse attribute part.
                foreach(var element in parts[1].Split(' ')) {
                    // Get key and value
                    int position = element.IndexOf("=");
                    string key = element.Substring(0, position);
                    string value = element.Substring(position+1);

                    // Check if value is quoted
                    if (value.StartsWith("\"") && value.EndsWith("\"")) {
                        value = value.Trim('\"'); // Remove quotes.
                    } // Todo: return syntax error

                    attributes.Add(key, value);
                }
            }

            return Tuple.Create(tagName, attributes, blockType);
        }
    }
    
    public class XmlBlock {
        // Properties to hold the tag name, attributes, content, and block type
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string? Content { get; set; } // Content should stored at close block.
        public XmlBlockType BlockType { get; set; }

        public static void AttributeParser (ref Dictionary<string, string> attributes, string[] elements) {
            for (int i = 1; i < elements.Length; i++) { // First element is tag name, skip it.
                int position = elements[i].IndexOf("=");
                string key = elements[i].Substring(0, position);
                string value = elements[i].Substring(position+1);

                // Check if value is quoted

                attributes.Add(key, value);
            }
            // Todo: throw error when there is syntax error in the attribute
            // throw new InvalidAttributeException("Syntax error occurs: Attribute values must always be quoted");
        }

        public XmlBlock (string tagString) {
            // Parse tag name, attributes, block type by tagParser
            TagParser tagParser = new TagParser();

            (string tagName, Dictionary<string, string> attributes, XmlBlockType blockType) = tagParser.Parse(tagString);

            // Set properties
            this.TagName = tagName;
            this.Attributes = attributes;
            this.BlockType = blockType;
            this.Content = null;
        }

        public void SetContent (string content) {
            this.Content = content;
        }

        public void ShowBlockInfo() {
            // Show tag name, block type
            Console.WriteLine($"Tag: {this.TagName}, Type: {this.BlockType}");

            // Show attributes
            if (this.Attributes.Count != 0) {
                Console.WriteLine("Attributes:");
                foreach(var kv in this.Attributes) {
                    Console.WriteLine($"  - {kv.Key}: {kv.Value}");
                }
            }

            // Show content.
            if (!string.IsNullOrWhiteSpace(this.Content)) {
                Console.WriteLine($"Content: {this.Content}");
            }
        }
    }

    public class SimpleXmlValidator
    {
        public static bool BlockIsMatch (XmlBlock openBlock, XmlBlock closeBlock, bool noAttribute) {
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

		public static bool XmlParser(string xml, bool noAttribute)
        {
            // Current string, whether block elements or content.
            string currentTagString = string.Empty;
            string currentContent = string.Empty;
            bool inTag = false;
            // Stack of Xml blocks.
            Stack<XmlBlock> blockStack = new Stack<XmlBlock>();

            // Read XML string by character
            foreach (char currentChar in xml)
            {
                if (currentChar == '<') // start of block
                {
                    inTag = true;
                }
                else if (currentChar == '>') // end of block
                {
                    inTag = false;
                    // Initial XML block, including tag, attributes, and block type.
                    // Remove space and '?' in both ends.
                    // Have to remove '?' or it may occurs error when attribute parsing
                    XmlBlock currentBlock = new XmlBlock(currentTagString);                    

                    if (currentBlock.BlockType == XmlBlockType.Close) // close block
                    {
                        // If there is no open block, return false.
                        if (blockStack.Count == 0) {
                            return false;
                        }

                        // Pop the latest open block.
                        XmlBlock openBlock = blockStack.Pop();

                        // Invalid if the open and close block is unmatched.
                        if (!BlockIsMatch(openBlock, currentBlock, noAttribute)) {
                            return false;
                        }

                        // Store content in close block
                        if (!string.IsNullOrWhiteSpace(currentContent))
                        {
                            currentBlock.SetContent(currentContent);
                            currentContent = string.Empty;
                        }
                    }
                    else if (currentBlock.BlockType == XmlBlockType.Open) // open block
                    {
                        // Push open block tag name to the tag stack.
                        blockStack.Push(currentBlock);
                    }
                    currentTagString = string.Empty; // Refresh tag string

                    // Show block info for test
                    // currentBlock.ShowBlockInfo();
                }
                else if (inTag) // Get Tag
                {
                    currentTagString += currentChar;
                }
                else { // Get Content
                    currentContent += currentChar;
                }
            }

            // Invalid if blockStack is not empty.
            if(blockStack.Count != 0) {
                return false;
            }

            return true;
        }
        //Please implement this method
        public static bool DetermineXml(string xml, bool noAttribute)
        {
            bool isValid = XmlParser(xml, noAttribute);

            return isValid;
        }
    }
}