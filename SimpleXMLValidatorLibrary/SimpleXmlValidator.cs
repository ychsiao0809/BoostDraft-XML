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
                    } else {
                        throw new InvalidAttributeException("The value should be quoted by halfwidth colon");
                    }

                    attributes.Add(key, value);
                }
            }

            return Tuple.Create(tagName, attributes, blockType);
        }
    }
    
    class XmlParser : IParsable<List<XmlBlock>> {
        // Return list of xml blocks in xml content
        public List<XmlBlock> Parse(string xml) {
            List<XmlBlock> xmlBlocks = new List<XmlBlock>();

            string currentTagString = string.Empty; // Store the tag string in block.
            string currentContent = string.Empty; // Store the content string between blocks.
            bool inTag = false; // Check if the character is in block or between blocks.

            // Read XML string by characters
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
                    XmlBlock currentBlock = new XmlBlock(currentTagString);                    

                    if (currentBlock.BlockType == XmlBlockType.Close) // close block
                    {
                        // Store content in open block
                        if (!string.IsNullOrWhiteSpace(currentContent))
                        {
                            xmlBlocks[xmlBlocks.Count-1].SetContent(currentContent);
                            currentContent = string.Empty;
                        }
                    }
                    
                    xmlBlocks.Add(currentBlock);
                    currentTagString = string.Empty; // Refresh tag string
                }
                else if (inTag) // Get Tag
                {
                    currentTagString += currentChar;
                }
                else { // Get Content
                    currentContent += currentChar;
                }
            }

            return xmlBlocks;
        }
    }

    public class XmlBlock {
        // Properties to hold the tag name, attributes, content, and block type
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string? Content { get; set; } // Content should stored at close block.
        public XmlBlockType BlockType { get; set; }

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