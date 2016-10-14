# ObjectiveParser
c# program that parses academic syllabi trying to pull out course objective

Parses syllabli looking for learning objective. Uses a separate parser for Word Docs and PDF documents. The parser is chosen using the factory pattern to select the proper parser for the file type.

looks throug syllabi for common language and SLO verbs

```
usage:

Please enter a file to parse as an argument.
Usage: filename.[pfd,doc,docx]
Usage: directory
```
