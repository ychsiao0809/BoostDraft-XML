# Take home coding test (XML)

The target of this task is to create a console application to check whether the given XML string is valid.

This document primarily references my [repo](https://github.com/ychsiao0809/BoostDraft-XML) on Github.

# Environment

- .NET SDK: v8.0.403
- Windows OS: v10.0.22631

# Developement Log

## Feature #1: Add .gitignore

## Feature #2**: Implement XML parser**

> https://github.com/ychsiao0809/BoostDraft-XML/pull/2/files
> 

### Summary

Implement an XMLparser capable of parsing XML string.

### Changes

- Implement `XmlParser` in `SimpleXmlValidator` for parsing xml blocks.
- Identifying start tags and end tags within the blocks.
- Handling and distinguishing the XML header from tag.

### Todo

### Summary

Implement an XMLparser capable of parsing XML string.

### Changes

- Implement `XmlParser` in `SimpleXmlValidator` for parsing xml blocks.
- Identifying start tags and end tags within the blocks.
- Handling and distinguishing the XML header from tag.

### Todo

- Create an XML block class to manage tags, attributes, and content.

## Feature #3: Implement XML block

> https://github.com/ychsiao0809/BoostDraft-XML/pull/3/files
> 

### Summary

Implemnet an XML block to manage tag, attributes, and block type.

### Changes

- Implement XML blocks class for XML elements parsing
    - initial XML block by content elements.
- Optimize the way of determing block type.

### Related PR

Complete Todo in [#2](https://github.com/ychsiao0809/BoostDraft-XML/pull/2).

### Todo

- Implement setContent to save content in open block.
- Divide the string of block and string of content.
- Validation
    - check if open block match every close block.

## Feature #5: V**alidation for unmatch open/close block**

> https://github.com/ychsiao0809/BoostDraft-XML/pull/5/files
> 

### Summary

Validation for unmatch open/close block, or remaining open block.

### Changes

- Change XmlParser to return boolean value (XML is valid or not).
- Validation
    - return invalid if their are remaining open block when xml string ends (Example2).
    - return invalid if the tag name of close block is not match with the latest open block (Example 3).

### Related PR

Complete Todo in [#3](https://github.com/ychsiao0809/BoostDraft-XML/pull/3).

### Todo

- Implement setContent to save content in open block.
- Divide the string of block and string of content.
- Validation
    - illegal character detection (Example 4).
    - check if close block remains (Optional).

## Test #6: Adding my test case

> https://github.com/ychsiao0809/BoostDraft-XML/pull/6
> 

### Summary

Adding new test case for different situation.

### Changes

- Adding new test case

```xml
<?xml version="1.0" encoding="UTF-8"?>
<books>
    <book id="1">
        <title>Hello C#</title>
        <author>David Hsiao</author>
        <price>199.9</price>
    </book>
    <book id="2">
        <title>Hello XML</title>
        <author>Hsiao David</author>
        <price>299.9</price>
    </book>
</books>
```

## Feature #7: A**dd block matching function**

> https://github.com/ychsiao0809/BoostDraft-XML/pull/7/files
> 

### Summary

Implement block matching function.

### Changes

- Build `AttributeParser` in `XmlBlock`. (May need syntax check)
- Implement `BlockIsMatch` function. (Example 4)
    - change tagStack to blockStack for block matching implementation.
    - compare if the close block is matched with open block.
- Implement `ShowBlockInfo` to display the block's information, including tag, attributes, block type, content.
- Save content in close block.

### Hotfix

- Fix the xml header problem.
    - occur errors when running on my own testcase
    - remove '?' character in the begin and end makes it works, idk why :(
- Fix naming rules for function in class. (`SetConent`).
- Divide `currentString` into `currentTagString` and `currentContent`.
    - `currentTagString` use to store string in block.
    - `currentContent` use to store the content between open and close blocks.
    - add `inTag` boolean to check if the character is for tag or content.

### Related PR

Complete the future work in #5.

### Todo

- Validation
    - option to compare open/close block with attributes or not (Optional)
    - illegal character detection (Optional).
    - check if close block remains (Optional).

## Refactor #8: M**ove `BlockIsMatch` from `XmlBlock` to `SimpleXmlValidator`**

> https://github.com/ychsiao0809/BoostDraft-XML/pull/8/files
> 

### Summary

Move function `BlockIsMatch` from class `XmlBlock` to class `SimpleXmlValidator` , which makes more sense.

## Feature #9: I**mplement new variable --no-attr**

> https://github.com/ychsiao0809/BoostDraft-XML/pull/9/files
> 

### Summary

Option to compare open/close block with attributes or not using `--no-attr` argument.

### Changes

- Detemine whether needs to compare attributes with `-no-attr` argument.
- Add `expectedResult_noAttr` element for `testCase` list.
- Skip attribute comparing in function `BlockIsMatch` of class `SimpleXmlValidator` if boolean `noAttribute` is `true`.

### Screenshot

![https://private-user-images.githubusercontent.com/34422210/377412376-814792df-aaeb-4112-bae2-5a767dbb79a9.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MjkxNjA0NjYsIm5iZiI6MTcyOTE2MDE2NiwicGF0aCI6Ii8zNDQyMjIxMC8zNzc0MTIzNzYtODE0NzkyZGYtYWFlYi00MTEyLWJhZTItNWE3NjdkYmI3OWE5LnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNDEwMTclMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjQxMDE3VDEwMTYwNlomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPWM1MmM2ODU4ZTZkZGQxOTZmYmM1MTgxZGUzYWVjYzEwN2VjNTc5ZjEyODMwZTZkMGMzMTdlMDg1YjA2NDIxNzkmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.RaYfQL0V-07hzZI0FET1SPx-aYAAG4_ZAC6-V6KB4so](https://private-user-images.githubusercontent.com/34422210/377412376-814792df-aaeb-4112-bae2-5a767dbb79a9.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MjkxNjA0NjYsIm5iZiI6MTcyOTE2MDE2NiwicGF0aCI6Ii8zNDQyMjIxMC8zNzc0MTIzNzYtODE0NzkyZGYtYWFlYi00MTEyLWJhZTItNWE3NjdkYmI3OWE5LnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNDEwMTclMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjQxMDE3VDEwMTYwNlomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPWM1MmM2ODU4ZTZkZGQxOTZmYmM1MTgxZGUzYWVjYzEwN2VjNTc5ZjEyODMwZTZkMGMzMTdlMDg1YjA2NDIxNzkmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.RaYfQL0V-07hzZI0FET1SPx-aYAAG4_ZAC6-V6KB4so)

## Feature #10: Check if close blocks remains

> https://github.com/ychsiao0809/BoostDraft-XML/pull/10/files
> 

### Summary

Check if close block remains (which means no open block).

### Changes

- Return false if no open block in `blockStack`.

### Related PR

Complete Todo in [#7](https://github.com/ychsiao0809/BoostDraft-XML/pull/7).

### Todo

- Optimize test process (change test from strings into files).

## Refactor #11: Optimize the program structure

> https://github.com/ychsiao0809/BoostDraft-XML/pull/11/files
> 

### Summary

Optimize the program structure by separating tag parser into individual class.

### Changes

- Seperate `XmlBlockType` from XmlBlock to an external enumerator.
- Design `IParsable` interface for `TagParser` and `XmlParser` (working on).
- Implement `TagParser`.

### Todo

- Implement `XmlParser`.

## Refactor #12: Optimize validator structure

> https://github.com/ychsiao0809/BoostDraft-XML/pull/12/files
> 

### Summary

Refactor `SimpleXmlValidator` structure to improve readability and maintainability.

### Changes

- Implement `XmlParser`.
- Move xmlParser function from `SimpleXmlValidator` to `XmlParser`.
    1. Merge functions `XmlParser` and `DetermineXml` in `SimpleXmlValidator`.
    2. Rebuild character parsing part of XmlParser to a new `XmlParser` class (output a list of xml blocks).
    3. `DetemineXml` function determine `isValid` by list of xml blocks.
- Remove function `AttributeParser` in class `XmlBlock`.
- Change `noAttribute` tag into external variable.

### Related PR

Complete Todo in [#11](https://github.com/ychsiao0809/BoostDraft-XML/pull/11).

## Feature #13: Add syntax error detection

> https://github.com/ychsiao0809/BoostDraft-XML/pull/13
> 

### Summary

Implement syntax error detection by customized exception `InValidAttributeException`.

### Change

- Test case
    - edit the result of origin test case to false, because of the fullwidth colons instead of halfwidth colons in value of attribute.
    - add new test case.
- throw `InValidAttributeException` when the value of attributes is not well quoted.
- return false when catch `InValidAttributeException`.

### Related PR

Complete Todo in [#7](https://github.com/ychsiao0809/BoostDraft-XML/pull/7).