import * as ts from 'typescript';

export class Formatter {
    format = (text: string) => {
        const options = this.getDefaultOptions();

        // Parse the source text
        const sourceFile = ts.createSourceFile('file.ts', text, ts.ScriptTarget.Latest, /*setParentPointers*/ true);

        // Get the formatting edits on the input sources
        const edits = (<any>ts).formatting.formatDocument(sourceFile, this.getRuleProvider(options), options);

        // Apply the edits on the input code
        return this.applyEdits(text, edits);
    }

    private getRuleProvider = (options: ts.FormatCodeOptions) => {
        // Share this between multiple formatters using the same options.
        // This represents the bulk of the space the formatter uses.
        const ruleProvider = new (<any>ts).formatting.RulesProvider();
        ruleProvider.ensureUpToDate(options);
        return ruleProvider;
    }

    private applyEdits = (text: string, edits: ts.TextChange[]): string => {
        // Apply edits in reverse on the existing text
        let result = text;
        for (let i = edits.length - 1; i >= 0; i--) {
            const change = edits[i];
            const head = result.slice(0, change.span.start);
            const tail = result.slice(change.span.start + change.span.length)
            result = head + change.newText + tail;
        }
        return result;
    }

    private getDefaultOptions = (): ts.FormatCodeOptions => {
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
    }
}
