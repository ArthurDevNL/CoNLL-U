# CoNLL-U Parser in .NET Core
![.NET Core](https://github.com/ArthurDevNL/CoNLLU-Parser/workflows/.NET%20Core/badge.svg?branch=main)

This repository contains a lightweight, well-tested CoNLL-U parser written in C# .NET Core and parses according to the CoNLL-U format as specified by [Universal Dependencies](https://universaldependencies.org/format.html).

## Quick Start

CoNLL-U is available as a NuGet package. Once installed, you can start as follows:

```
var filePath = ...
var sentences = Conllu.ParseFile(filePath);
```

Each `Sentence` contains a list of `Token` which contain all the information as specified in the CoNLL-U format. Below is a short overview of some of the fields that are available in the `Token` class:

```
   public class Token
   {
        // CoNLL-U Properties
        int Id;
        string Form;
        string Lemma;
        string Upos;
        string Xpos;
        Dictionary<string, string> Feats;
        int? Head;
        string DepRel;
        Dictionary<TokenIdentifier, string> Deps;
        string Misc;
        
        // Other properties
        TokenIdentifier Identifier;
        string RawLine;
        bool IsMultiwordToken;
        bool IsEmptyNode;
   }
```

In addition, there is a `TokenIdentifier` class which wraps the different possibilities for word ID such as multiword tokens or empty nodes.

## To-do
- [x] Support empty nodes
- [ ] Add tree parsing helper functions
- [ ] Add serialization support to generate .conllu files

## License

Copyright (c) 2021 Arthur Hemmer

Distributed under the MIT License (MIT).




