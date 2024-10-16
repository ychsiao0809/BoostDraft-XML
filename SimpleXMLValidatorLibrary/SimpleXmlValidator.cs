namespace SimpleXMLValidatorLibrary
{
	//feel free to add other classes/methods if you want
    public class XmlBlock {
        public enum XmlBlockType {
            Open, // Represent an opening tag, e.g. <book>
            Close, // Represent a closing tag, e.g. </book>
            Xml, // Represent the XML header, e.g. <?xml version="1.0"?>
        };

        // Properties to hold the tag name, attributes, content, and block type
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string? Content { get; set; }
        public XmlBlockType BlockType { get; set; }

        public XmlBlock (string[] elements) {
            string tagName = elements[0];
            XmlBlockType blockType;
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            if (tagName == "?xml") {
                blockType = XmlBlockType.Xml;
            } else if (tagName.StartsWith("/")) {
                tagName = tagName.Substring(1, tagName.Length-1);
                blockType = XmlBlockType.Close;
            } else {
                blockType = XmlBlockType.Open;
            }
            // Get attributes
            for (int i = 1; i < elements.Length; i++) {
                int position = elements[i].IndexOf("=");
                string key = elements[i].Substring(0, position);
                string value = elements[i].Substring(position+1);

                attributes.Add(key, value);
            }

            // Set properties
            this.TagName = tagName;
            this.BlockType = blockType;
            this.Attributes = attributes;
            this.Content = null;
        }

        public void setContent (string content) {
            this.Content = content;
        }
    }

    public class SimpleXmlValidator
    {
		public static bool XmlParser(string xml)
        {
            // Current string, whether block elements or content.
            string currentString = string.Empty;
            // Stack the start tags.
            Stack<string> tagStack = new Stack<string>();

            // Read XML string by character
            foreach (char currentChar in xml)
            {
                if (currentChar == '<') // start of block
                {
                    if (!string.IsNullOrWhiteSpace(currentString))
                    {
                        currentString = string.Empty;
                    }
                }
                else if (currentChar == '>') // end of block
                {
                    // Initial XML block, including tag, attributes, and block type
                    XmlBlock currentBlock = new XmlBlock(currentString.Split(' '));

                    if (currentBlock.BlockType == XmlBlock.XmlBlockType.Close) // close block
                    {
                        // Pop the latest open block.
                        string startTagName = tagStack.Pop();

                        // Invalid if the open and close block is unmatched.
                        if (startTagName != currentBlock.TagName) {
                            return false;
                        }
                    }
                    else if (currentBlock.BlockType == XmlBlock.XmlBlockType.Open) // open block
                    {
                        // Push open block tag name to the tag stack.
                        tagStack.Push(currentBlock.TagName);
                    }
                    // Refresh content
                    currentString = string.Empty;
                }
                else // Get content
                {
                    currentString += currentChar;
                }
            }

            // Invalid if tagStack is not empty.
            if(tagStack.Count != 0) {
                return false;
            }

            return true;
        }
        //Please implement this method
        public static bool DetermineXml(string xml)
        {
            bool isValid = XmlParser(xml);

            return isValid;
        }
    }
}