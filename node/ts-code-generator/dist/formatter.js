"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ts = require("typescript");
var Formatter = (function () {
    function Formatter() {
        var _this = this;
        this.format = function (text) {
            var options = _this.getDefaultOptions();
            // Parse the source text
            var sourceFile = ts.createSourceFile('file.ts', text, ts.ScriptTarget.Latest, /*setParentPointers*/ true);
            // Get the formatting edits on the input sources
            var edits = ts.formatting.formatDocument(sourceFile, _this.getRuleProvider(options), options);
            // Apply the edits on the input code
            return _this.applyEdits(text, edits);
        };
        this.getRuleProvider = function (options) {
            // Share this between multiple formatters using the same options.
            // This represents the bulk of the space the formatter uses.
            var ruleProvider = new ts.formatting.RulesProvider();
            ruleProvider.ensureUpToDate(options);
            return ruleProvider;
        };
        this.applyEdits = function (text, edits) {
            // Apply edits in reverse on the existing text
            var result = text;
            for (var i = edits.length - 1; i >= 0; i--) {
                var change = edits[i];
                var head = result.slice(0, change.span.start);
                var tail = result.slice(change.span.start + change.span.length);
                result = head + change.newText + tail;
            }
            return result;
        };
        this.getDefaultOptions = function () {
            return {
                IndentSize: 4,
                TabSize: 4,
                NewLineCharacter: '\r\n',
                ConvertTabsToSpaces: true,
                InsertSpaceAfterCommaDelimiter: true,
                InsertSpaceAfterSemicolonInForStatements: true,
                InsertSpaceBeforeAndAfterBinaryOperators: true,
                InsertSpaceAfterKeywordsInControlFlowStatements: true,
                InsertSpaceAfterFunctionKeywordForAnonymousFunctions: false,
                InsertSpaceAfterOpeningAndBeforeClosingNonemptyParenthesis: false,
                InsertSpaceAfterOpeningAndBeforeClosingNonemptyBrackets: true,
                InsertSpaceAfterOpeningAndBeforeClosingTemplateStringBraces: true,
                IndentStyle: ts.IndentStyle.Smart,
                PlaceOpenBraceOnNewLineForFunctions: false,
                PlaceOpenBraceOnNewLineForControlBlocks: false,
            };
        };
    }
    return Formatter;
}());
exports.Formatter = Formatter;
//# sourceMappingURL=formatter.js.map